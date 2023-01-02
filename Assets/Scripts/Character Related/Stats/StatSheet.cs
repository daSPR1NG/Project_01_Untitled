using UnityEngine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    // TODO : 
    // - The boolean used in the method "SetSubStatsValues" as a parameter, might need to be replaced in the future.
    // - The boolean used by the method "ResetStatsPointsToDefault" as a parameter, might need to be replaced in the future.

    public enum StatType { Unassigned, Strength, Endurance, Dexterity, }
    public enum SubType
    {
        Unassigned, Initiative_INI, HealthPoints_HP, DamageReduction_DMR, Resistance_RES, Damage_DMG, CounterAttackChance_CA,
        Precision_PRE, Dodge_DOD
    }

    ///<summary> StatSheet description <summary>
    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Character/New Stat Sheet" )]
    public class StatSheet : ScriptableObject, IDebuggable
    {
        [Header( "Stats" )]
        [SerializeField] private List<Stat> _stats = new();

        [Header( "Sub stats" )]
        [SerializeField] private List<SubStat> _subStats = new( System.Enum.GetValues( typeof( SubType ) ).Length );

        [Header( "Attached stats experience" )]
        [SerializeField] private bool _usesAnExperienceManager = true;
        [ShowIf( "_usesAnExperienceManager" )]
        [SerializeField] private StatsExperienceManager _attachedExperienceManager;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            StatsExperienceManager.OnStatLevelUp += AddPointToStat;
        }

        void OnDisable() 
        {
            StatsExperienceManager.OnStatLevelUp += AddPointToStat;
        }

        #endregion

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
        public void ResetStatsPointsToDefault( bool needsToBeReset )
        {
            if ( !needsToBeReset || _stats.IsEmpty() ) { return; }

            for ( int i = _stats.Count - 1; i >= 0; i-- )
            {
                _stats [ i ].ResetPoints();
            }

            SetSubStatsValues();
        }

        #endregion

        #region SubStats related

        /// <summary>
        /// Sends main stats point to sub stats to update their value.
        /// </summary>
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
                // Might need to replace the boolean used here to match the fact that when resuming the game we don't want the player character...
                // ...to regenerate health fully while he had lost hp in the last game.
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

        private const int STAT_AMOUNT = 3;
        private const int SUB_STAT_AMOUNT = 8;

        [Button( "Reset sheet's stat and sub stats value" )]
        private void ResetSheet()
        {
            bool needToBeReset = true;

            ResetStatsPointsToDefault( needToBeReset );
            SetStatsAndSubStatsNames();
            _attachedExperienceManager.ResetExperienceDatasToDefault( needToBeReset );
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

        private void SetRelatedStatSheetOfAttachedExperienceManager()
        {
            if ( _attachedExperienceManager.IsNull() ) { return; }
            _attachedExperienceManager.SetRelatedStatSheet( this );
        }

        private void InitSheet()
        {
            if ( _attachedExperienceManager.IsNull() ) { return; }

            int statTypeIndex = 1;
            int subStatTypeIndex = 1;

            for ( int i = 0; i < STAT_AMOUNT; i++ )
            {
                Stat createdStat = new()
                {
                    Type = ( StatType ) statTypeIndex,
                    Points = 0,
                    Name = ( ( StatType ) statTypeIndex ).ToString(),
                };
                
                if ( _stats.Count < STAT_AMOUNT )
                {
                    _stats.AppendItem( createdStat );
                    statTypeIndex++;
                }
            }

            for ( int i = 0; i < SUB_STAT_AMOUNT; i++ )
            {
                SubStat createdSubStat = new()
                {
                    Type = ( SubType ) subStatTypeIndex,
                    Name = ( ( SubType ) subStatTypeIndex ).ToString(),
                };

                if ( _subStats.Count < SUB_STAT_AMOUNT )
                {
                    _subStats.AppendItem( createdSubStat );
                    subStatTypeIndex++;
                }
            }

            SetRelatedStatSheetOfAttachedExperienceManager();
            SetSubStatsValues();
        }

        private void OnValidate()
        {
            InitSheet();
            SetStatsAndSubStatsNames();
        }
#endif

        #endregion
    }
}