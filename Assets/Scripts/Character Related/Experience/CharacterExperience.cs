using UnityEngine;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using dnSR_Coding.Utilities;
using System;

namespace dnSR_Coding
{
    // TODO : 
    // - The boolean used by the method "ResetExperienceDatasToDefault", might need to be replaced in the future.

    [RequireComponent( typeof( PlayerCharacterProperties ) )]

    ///<summary> CharacterExperience description <summary>
    [Component("CharacterExperience", "")]
    [DisallowMultipleComponent]
    public class CharacterExperience : MonoBehaviour, IDebuggable
    {
        [SerializeField] private List<ExperienceData> _experienceData = new();

        private PlayerCharacterProperties _pcPropertiesManager;        

        public static Action<StatType> OnStatLevelUp;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            ExperienceSource.OnGeneratingExperience += AddExperienceToAStat;
        }

        void OnDisable() 
        {
            ExperienceSource.OnGeneratingExperience -= AddExperienceToAStat;
        }

        #endregion

        void Awake() => Init();
        
        // Set all datas that need it at the start of the game
        void Init()
        {
            GetLinkedComponents();

            // Change the bool value to match the current state of the game, when resuming we don't want to reset the EXP
            ResetExperienceDatasToDefault( true );
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            if ( _pcPropertiesManager.IsNull() )
            { _pcPropertiesManager = GetComponent<PlayerCharacterProperties>(); }
        }

        /// <summary>
        /// Adds a certain amount of experience to the given experience data.
        /// </summary>
        /// <param name="data"> the data you're looking for </param>
        /// <param name="amount"> the amount to push to the current pool </param>
        public void AddExperienceToAStat( StatType type, int amount )
        {
            ExperienceData data = GetExperienceDataByType( type );

            if ( data.IsNull() )
            {
                Debug.LogError( "There is no stat corresponding, this means something is wrong." );
                return;
            }

            // We recalculate the correct amount of exp to add, taking into account the current value of "Multiplier".
            amount = ExtMathfs.Abs( ExtMathfs.FloorToInt( amount * data.Multiplier ) );

            int remainingExperience = data.MaxValue - data.CurrentValue;
            bool itsALevelUp = amount >= remainingExperience;

            // Here we check if by adding the amount given, the current value exceeds the max value or not...
            // ... if so, we substract the possible remaining amount before re-adding it to the CurrentValue pool.
            if ( itsALevelUp )
            {
                remainingExperience = ( data.CurrentValue + amount ) - data.MaxValue;
                data.SetCurrentValue( 0 );

                data.LevelUp();
                data.UpgradeMaxValue();

                if ( !Application.isPlaying )
                {
                    _pcPropertiesManager.CharacterStats.AddPointToStat( type );
                }
                else { OnStatLevelUp?.Invoke( type ); }
            }

            // We define the exact amount to add based on wether its a levelup or not...
            amount = itsALevelUp ? remainingExperience : amount;
            // ... then we add it.
            data.AddToCurrentValue( ExtMathfs.Abs( amount ) );
        }

        /// <summary>
        /// Return an experience data, matching the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ExperienceData GetExperienceDataByType( StatType type )
        {
            if ( _experienceData.IsEmpty() )
            {
                Debug.LogError( "There is no experience data set, this means something is wrong." );
                return null;
            }

            for ( int i = _experienceData.Count - 1; i >= 0; i-- )
            {
                if ( _experienceData [ i ].LinkedStatType != type ) { continue; }

                return _experienceData [ i ];
            }

            Debug.LogError( "There is no experience data of this Type, this means something is wrong." );
            return null;
        }

        /// <summary>
        /// Resets each valid experience data to default.
        /// </summary>
        public void ResetExperienceDatasToDefault( bool needsToBeReset )
        {
            if ( !needsToBeReset || _experienceData.IsEmpty() ) { return; }

            for ( int i = _experienceData.Count - 1; i >= 0; i-- )
            {
                if ( i.IsNull() )
                {
                    _experienceData.RemoveAt( i );
                    Debug.Log( "Experience data index " + i + " was null, it has been removed." );
                    continue;
                }

                _experienceData [ i ].ResetToDefault();
            }
        }

        #region OnValidate

#if UNITY_EDITOR

        [Button]
        private void AddExperienceToStrength()
        {
            AddExperienceToAStat( StatType.Strength, 10 );
        }
        [Button]
        private void AddExperienceToEndurance()
        {
            AddExperienceToAStat( StatType.Endurance, 10 );
        }
        [Button]
        private void AddExperienceToDexterity()
        {
            AddExperienceToAStat( StatType.Dexterity, 10 );
        }

        [Button]
        private void ResetExperienceDatasButton()
        {
            ResetExperienceDatasToDefault( true );
        }

        private void SetExperienceDatasName()
        {
            if ( _experienceData.IsEmpty() ) { return; }

            for ( int i = _experienceData.Count - 1; i >= 0; i-- )
            {
                _experienceData [ i ].SetName();
            }
        }

        private void OnValidate()
        {
            GetLinkedComponents();
            SetExperienceDatasName();
        }

#endif

        #endregion
    }
}