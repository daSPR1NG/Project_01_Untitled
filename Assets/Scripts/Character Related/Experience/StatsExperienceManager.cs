using UnityEngine;
using System;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Character/New StatBackUp Experience Manager" )]
    public class StatsExperienceManager : ScriptableObject
    {
        [SerializeField] private List<StatExperienceData> _experienceData = new();

        public static Action<StatType> OnStatLevelUp;

#if UNITY_EDITOR
        private StatSheetBackUp _relatedStatSheet = null;
#endif

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

        /// <summary>
        /// Adds a certain addedAmount of experience to the given experience data.
        /// </summary>
        /// <param name="data"> the data you're looking for </param>
        /// <param name="addedAmount"> the addedAmount to push to the current pool </param>
        public void AddExperienceToAStat( StatType type, int addedAmount )
        {
            StatExperienceData data = GetExperienceDataByType( type );

            if ( data.IsNull() )
            {
                Debug.LogError( "There is no stat corresponding, this means something is wrong." );
                return;
            }

            data.AddExperience( Helper.PositiveValue( addedAmount ) );
        }

        /// <summary>
        /// Return an experience data, matching the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private StatExperienceData GetExperienceDataByType( StatType type )
        {
            if ( _experienceData.IsEmpty() )
            {
                Debug.LogError( "There is no experience data set, this means something is wrong." );
                return null;
            }

            for ( int i = _experienceData.Count - 1; i >= 0; i-- )
            {
                if ( _experienceData [ i ].GetAssociatedStat() != type ) { continue; }

                return _experienceData [ i ];
            }

            Debug.LogError( "There is no experience data of this _type, this means something is wrong." );
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

        [Button]
        public void SetupStatExperienceDatas()
        {
            ResetExperienceDatasToDefault( true );
        }
        [Button]
        public void AddExperienceToStrength()
        {
            AddExperienceToAStat( StatType.Strength, 500 );
        }
        [Button]
        public void AddExperienceToEndurance()
        {
            AddExperienceToAStat( StatType.Endurance, 250 );
        }
        [Button]
        public void AddExperienceToDexterity()
        {
            AddExperienceToAStat( StatType.Dexterity, 50 );
        }

        #region OnValidate

#if UNITY_EDITOR        

        private void SetExperienceDatasName()
        {
            if ( _experienceData.IsEmpty() ) { return; }

            for ( int i = _experienceData.Count - 1; i >= 0; i-- )
            {
                _experienceData [ i ].SetName();
            }
        }

        public void SetRelatedStatSheet( StatSheetBackUp sheet )
        {
            if ( _relatedStatSheet != sheet ) { _relatedStatSheet = sheet; }
        }

        private void OnValidate()
        {
            SetExperienceDatasName();
        }

#endif

        #endregion
    }
}