using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using static dnSR_Coding.CharacterExperience;
using static dnSR_Coding.Stat;

namespace dnSR_Coding
{
    public enum StatType { Unassigned, Strength, Endurance, Dexterity, }
    public enum SubType
    {
        Unassigned, Initiative_INI, HealthPoints_HP, DamageReduction_DMGR, Resistance_RES, Damage_DMG, CounterAttackChance_CA,
        Precision_PRE, Dodge_DOD
    }

    [RequireComponent( typeof( PlayerCharacterProperties ) )]

    ///<summary> CharacterStats description <summary>
    [Component("CharacterStats", "")]
    [DisallowMultipleComponent]
    public class CharacterStats : MonoBehaviour, IDebuggable
    {
        [Header( "Details" )]

        [SerializeField] private List<Stat> _stats = new();
        [SerializeField] private List<SubStat> _subStats = new( System.Enum.GetValues( typeof( SubType ) ).Length );

        private PlayerCharacterProperties _playerCharacterPropertiesManager;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            OnStatLevelUp += AddPointToStat;
        }

        void OnDisable() 
        {
            OnStatLevelUp += AddPointToStat;
        }

        #endregion

        void Awake() => Init();
        
        // Set all datas that need it at the start of the game
        void Init()
        {
            GetLinkedComponents();
            SetSubStatsValues();
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            if ( _playerCharacterPropertiesManager.IsNull() ) 
            { _playerCharacterPropertiesManager = GetComponent<PlayerCharacterProperties>(); }
        }

        #region Stats related

        /// <summary>
        /// Looks for a stat of the given type and adds a point to it.
        /// </summary>
        /// <param name="type"></param>
        public void AddPointToStat( StatType type )
        {
            Stat stat = GetStatByType( type );
            stat.AddPoint( 1 );

            SetSubStatsValues();
        }

        /// <summary>
        /// Returns a stat matching the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Stat GetStatByType( StatType type )
        {
            if ( _stats.IsEmpty() )
            {
                Debug.LogError( "There is no stat set, this means something is wrong." );
                return null;
            }

            for ( int i = _stats.Count - 1; i >= 0; i-- )
            {
                if ( _stats [ i ].Type != type ) { continue; }

                return _stats [ i ];
            }

            Debug.LogError( "There is no stat of this Type, this means something is wrong." );
            return null;
        }        

        /// <summary>
        /// Resets the points of each stats.
        /// </summary>
        public void ResetStatsPointsToDefault()
        {
            if ( _stats.IsEmpty() ) { return; }

            for ( int i = _stats.Count - 1; i >= 0; i-- )
            {
                _stats [ i ].ResetPoints();
            }

            SetSubStatsValues();
        }

        #endregion

        #region SubStats related

        private void SetSubStatsValues()
        {
            if ( _subStats.IsEmpty() )
            {
                Debug.LogError( "There is no subStat set, this means something is wrong." );
                return;
            }

            int strengthPts = GetStatByType( StatType.Strength ).Points;
            int endurancePts = GetStatByType( StatType.Endurance ).Points;
            int dexterityPts = GetStatByType( StatType.Dexterity ).Points;

            for ( int i = _subStats.Count - 1; i >= 0; i-- )
            {
                _subStats [ i ].CalculateValue( strengthPts, endurancePts, dexterityPts );
            }
        }

        /// <summary>
        /// Returns a subStat matching the given type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public SubStat GetSubStatByType( SubType type )
        {
            if ( _subStats.IsEmpty() )
            {
                Debug.LogError( "There is no subStat set, this means something is wrong." );
                return null;
            }

            for ( int i = _subStats.Count - 1; i >= 0; i-- )
            {
                if ( _subStats [ i ].Type != type ) { continue; }

                return _subStats [ i ];
            }

            Debug.LogError( "There is no subStat of this Type, this means something is wrong." );
            return null;
        }

        #endregion

        #region OnValidate

#if UNITY_EDITOR

        [Button]
        private void ResetStatsPointsButton()
        {
            ResetStatsPointsToDefault();
        }

        private void SetStatsAndSubStatsNames()
        {
            if ( !_stats.IsEmpty() )
            {
                for ( int i = 0; i < _stats.Count; i++ )
                {
                    _stats [ i ].SetName();
                }
            }

            if ( !_subStats.IsEmpty() )
            {
                for ( int i = 0; i < _subStats.Count; i++ ) 
                {
                    _subStats [ i ].SetName();
                }
            }
        }

        private void OnValidate()
        {
            GetLinkedComponents();

            SetStatsAndSubStatsNames();
            SetSubStatsValues();
        }
#endif

        #endregion
    }
}