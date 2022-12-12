using UnityEngine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    [System.Serializable]
    public class SubStat
    {
        // Max for defensive subStats
        private const int DAMAGE_REDUCTION_MAX_VALUE =          15;
        private const int COUNTER_ATTACK_CHANCE_MAX_VALUE =     15;
        private const int DODGE_MAX_VALUE =                     15;

        // Max up to 100%
        private const int RESISTANCE_MAX_VALUE =                100;
        private const int PRECISION_MAX_VALUE =                 100;

        [Header( "Details" )]

        [HideInInspector] public string Name;
        [SerializeField] private SubType _type = SubType.Unassigned;

        [Header( "Base TotalValue Settings" )]

        private bool _hasBaseValue = true;
        [SerializeField, AllowNesting, ShowIf( "_hasBaseValue" )]
        private int _baseValue = 1;

        private bool _hasMaxValue = false;
        private int _maxValue = 0;

        public int CurrentValue /*{ get; private set; }*/; // in public only to debug !
        public int TotalValue /*{ get; private set; }*/; // in public only to debug !
        public SubType Type => _type;

        /// <summary>
        /// Calculate the correct value of the substat, based on its type.
        /// </summary>
        /// <param name="strength"> Equals to the current amount of point in strength. </param>
        /// <param name="endurance"> Equals to the current amount of point in endurance.</param>
        /// <param name="dexterity"> Equals to the current amount of point in dexterity.</param>
        public void CalculateValue( int strength, int endurance, int dexterity, bool initCurrentValue )
        {            
            int calculatedValue = 0;

            // By default we reset both base and max values.
            SetBaseValue();
            SetMaxValue();

            switch ( Type )
            {
                #region Others

                // TotalValue equals 0.
                case SubType.Unassigned:
                    calculatedValue = 0;
                    break;

                // Initiative – INI : (END + FOR + DEX) * 1.
                case SubType.Initiative_INI: 
                    calculatedValue = _baseValue + ( strength + endurance + dexterity );
                    break;

                #endregion

                #region Endurance - END

                // Points de vie - HP = baseHP + ((baseHP * .1f) * END).
                case SubType.HealthPoints_HP: 
                    calculatedValue = endurance > 0
                        ? ExtMathfs.FloorToInt( _baseValue + ( ( _baseValue * .1f ) * endurance ) )
                        : _baseValue;
                    break;

                // Réduction de dégâts - RDMG = (END * 5) / 4.
                case SubType.DamageReduction_DMGR: 
                    calculatedValue = ExtMathfs.FloorToInt( ( endurance * 5 ) / 4 );
                    break;

                // Résistance aux effets - RES = ( END * .25f ) * 10.
                case SubType.Resistance_RES: 
                    calculatedValue = ExtMathfs.FloorToInt( ( endurance * .25f ) * 10 );
                    break;

                #endregion

                #region Strength - STR

                // Dégâts - DMG = baseDMG + ((baseDMG * .4f) * FOR).
                case SubType.Damage_DMG:                     
                    calculatedValue = strength > 0
                        ? ExtMathfs.FloorToInt( _baseValue + ( ( _baseValue * .4f ) * strength ) )
                        : _baseValue;
                    break;

                // Chance de contre-attaquer – CATT = (FOR * .2f) * 2.5f.
                case SubType.CounterAttackChance_CA: 
                    calculatedValue = ExtMathfs.FloorToInt( ( strength * .2f ) * 2.5f );
                    break;

                #endregion

                #region Dexterity - DEX

                // Précision – PRE = (DEX * .25f) *  2.5f .
                case SubType.Precision_PRE: 
                    calculatedValue = ExtMathfs.FloorToInt( ( dexterity * .25f ) * 2.5f );
                    break;

                // Esquive – ESQ = (DEX * .2f) *  2.5f .
                case SubType.Dodge_DOD: 
                    calculatedValue = ExtMathfs.FloorToInt( ( dexterity * .2f ) * 2.5f );
                    break;

                #endregion
            }

            TotalValue = _hasMaxValue && calculatedValue >= _maxValue 
                ? _maxValue : calculatedValue;

            if ( initCurrentValue ) { CurrentValue = TotalValue; }
        }

        /// <summary>
        /// Sets the subStat base value.
        /// </summary>
        /// <param name="value"></param>
        private void SetBaseValue()
        {
            _hasBaseValue = false;
            _baseValue = 0;

            // Contains all the types that have base value, refers yourself to the GDD.
            switch ( Type )
            {
                // Base value is 5 - 07/12/2022
                case SubType.Initiative_INI:
                    _hasBaseValue = true;
                    _baseValue = 5;
                    break;

                // Base value is 25 - 07/12/2022
                case SubType.HealthPoints_HP:
                    _hasBaseValue = true;
                    _baseValue = 25;
                    break;

                // Base value is 5 - 07/12/2022
                case SubType.Damage_DMG:
                    _hasBaseValue = true;
                    _baseValue = 5;
                    break;
            }           
        }

        /// <summary>
        /// Sets the subStat max value.
        /// </summary>
        /// <param name="value"></param>
        private void SetMaxValue()
        {
            _hasMaxValue = false;
            _maxValue = 0;

            switch ( Type )
            {
                case SubType.DamageReduction_DMGR:
                    _hasMaxValue = true;
                    _maxValue = DAMAGE_REDUCTION_MAX_VALUE;
                    break;

                case SubType.Resistance_RES:
                    _hasMaxValue = true;
                    _maxValue = RESISTANCE_MAX_VALUE;
                    break;

                case SubType.CounterAttackChance_CA:
                    _hasMaxValue = true;
                    _maxValue = COUNTER_ATTACK_CHANCE_MAX_VALUE;
                    break;

                case SubType.Precision_PRE:
                    _hasMaxValue = true;
                    _maxValue = PRECISION_MAX_VALUE;
                    break;

                case SubType.Dodge_DOD:
                    _hasMaxValue = true;
                    _maxValue = DODGE_MAX_VALUE;
                    break;
            }
        }

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