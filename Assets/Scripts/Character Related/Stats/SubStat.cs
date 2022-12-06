using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
//using NaughtyAttributes;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    [System.Serializable]
    public class SubStat
    {
        private const int DAMAGE_REDUCTION_MAX_VALUE = 50;
        private const int RESISTANCE_MAX_VALUE = 100;
        private const int COUNTER_ATTACK_CHANCE_MAX_VALUE = 50;
        private const int PRECISION_MAX_VALUE = 100;
        private const int DODGE_MAX_VALUE = 50;

        [Header( "Details" )]

        [HideInInspector] public string Name;
        [SerializeField] private SubType _type = SubType.Unassigned;

        [Header( "Base Value Settings" )]

        private bool _hasBaseValue = true;
        [SerializeField, AllowNesting, ShowIf( "_hasBaseValue" )]
        private int _baseValue = 1;

        [Header( "Max Value Settings" )]

        private bool _hasMaxValue = true;
        [SerializeField, AllowNesting, ShowIf( "_hasMaxValue" ), Range( 0, 100 )]
        private int _maxValue = 0;

        public int Value /*{ get; private set; }*/; // in public only to debug !
        public SubType Type => _type;

        public void CalculateValue( int strength, int endurance, int dexterity )
        {
            switch ( Type )
            {
                #region Others

                case SubType.Unassigned:

                    Value = 0;
                    break;

                case SubType.Initiative_INI:
                    // Initiative – INI : (END + FOR + DEX) * 1.
                    SetBaseValue( 5 );

                    Value = _baseValue + ( strength + endurance + dexterity );
                    break;

                #endregion

                #region Endurance - END

                case SubType.HealthPoints_HP:
                    // Points de vie - HP = baseHP + ((baseHP * .1f) * END). 
                    SetBaseValue( 25 );

                    Value = endurance > 0
                        ? ExtMathfs.FloorToInt( _baseValue + ( ( _baseValue * .1f ) * endurance ) )
                        : _baseValue;

                    SetMaxValue( Value );
                    break;
                case SubType.DamageReduction_DMGR:
                    // Réduction de dégâts - RDMG = (END * 5) / 4.
                    ResetBaseValue();
                    SetMaxValue( DAMAGE_REDUCTION_MAX_VALUE );

                    Value = ExtMathfs.FloorToInt( ( endurance * 5 ) / 4 );
                    break;
                case SubType.Resistance_RES:
                    // Résistance aux effets - RES = ( END * .25f ) * 10.
                    ResetBaseValue();
                    SetMaxValue( RESISTANCE_MAX_VALUE );

                    Value = ExtMathfs.FloorToInt( ( endurance * .25f ) * 10 );
                    break;

                #endregion
                #region Strength - STR

                case SubType.Damage_DMG:
                    // Dégâts - DMG = baseDMG + ((baseDMG * .4f) * FOR). 
                    SetBaseValue( 5 );
                    ResetMaxValue();

                    Value = strength > 0 
                        ? ExtMathfs.FloorToInt( _baseValue + ( ( _baseValue * .4f ) * strength ) )
                        : _baseValue;
                    break;
                case SubType.CounterAttackChance_CA:
                    // Chance de contre-attaquer – CATT = (FOR * .2f) * 10.
                    ResetBaseValue();
                    SetMaxValue( COUNTER_ATTACK_CHANCE_MAX_VALUE );

                    Value = ExtMathfs.FloorToInt( ( strength * .2f ) * 10 );
                    break;

                #endregion
                #region Dexterity - DEX

                case SubType.Precision_PRE:
                    // Précision – PRE = (DEX * .25f) * 10.
                    ResetBaseValue();
                    SetMaxValue( PRECISION_MAX_VALUE );

                    Value = ExtMathfs.FloorToInt( ( dexterity * .25f ) * 10 );
                    break;
                case SubType.Dodge_DOD:
                    // Esquive – ESQ = (DEX * .2f) * 10.
                    ResetBaseValue();
                    SetMaxValue( DODGE_MAX_VALUE );

                    Value = ExtMathfs.FloorToInt( ( dexterity * .2f ) * 10 );
                    break;

                    #endregion
            }
        }

        #region Base value related

        private void SetBaseValue( int value )
        {
            _hasBaseValue = true;
            _baseValue = value;
        }
        private void ResetBaseValue()
        {
            _hasBaseValue = false;
            _baseValue = 0;
        }

        #endregion

        #region Max value related

        private void SetMaxValue( int value )
        {
            _hasMaxValue = true;
            _maxValue = value;
        }
        private void ResetMaxValue()
        {
            _hasMaxValue = false;
            _maxValue = 0;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR

        public void SetName()
        {
            string typeName = Type.ToString();
            if ( !Name.Equals( typeName ) ) { Name = typeName; }
        }
#endif

        #endregion
    }
}