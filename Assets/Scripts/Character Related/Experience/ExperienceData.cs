using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    public class ExperienceData : Subject, IValue<int>, IClampedValue<int>
    {
        [HideInInspector] public string Name = "[TYPE HERE]";

        [SerializeField, ReadOnly, AllowNesting] private int _level;

        [Header( "Scaling Settings" )]
        [Tooltip( "This value is the max exp required for the first level, it is used after on level up when calculating the new max value." )]
        [SerializeField] private int _initialMaxValue = 25;
        [Tooltip( "This value is used to calculate the new max value on level up." )]
        [SerializeField] private float _scalingFactorOnLevelUp = 1.25f;

        [Header( "EarningMultiplier Settings" )]
        [SerializeField] private float _earningMultiplier = 1;

        [field: SerializeField, ReadOnly, AllowNesting] public int Value { get; private set; }

        public bool HasMinValue { get => true; }
        public bool HasMaxValue { get => true; }

        public int MinValue => _initialMaxValue;
        [field: SerializeField, ReadOnly, AllowNesting] public int MaxValue { get; private set; }

        public float EarningMultiplier => _earningMultiplier;

        /// <summary>
        /// Adds a level and upgrade max value.
        /// </summary>
        public void LevelUp()
        {
            _level++;            
            UpgradeMaxValue();
        }

        /// <summary>
        /// Add experience to the current value, by accounting the earning multiplier.
        /// Able to handle multiple level ups.
        /// </summary>
        /// <param name="amount">The amount to add to current value.</param>
        public void AddExperience( int amount )
        {
            int properAmount = Helper.MultipliedValue( amount, EarningMultiplier );
            SetValue( Value + properAmount );

            while ( Value >= MaxValue )
            {
                Value -= MaxValue;
                LevelUp();                
            }

            NotifyObservers( this );
        }

        /// <summary>
        /// Upgrades max value, raising ( current value * level ) by the scaling factor.
        /// </summary>
        public void UpgradeMaxValue()
        {
            float raisedValue = Mathf.Pow( ( _initialMaxValue * _level ), _scalingFactorOnLevelUp );

            SetNewMaxValue( ExtMathfs.FloorToInt( raisedValue ) );

            //Debug.Log( "Max experience value of " + LinkedStatType.ToString() + " : " + MaxValue );
        }

        public void SetValue( int newValue ) => Value = newValue;
        public void SetNewMaxValue( int newMaxValue ) => MaxValue = newMaxValue;

        public int GetLevel() => _level;

        public void ResetToDefault()
        {
            _level = 0;

            _scalingFactorOnLevelUp = 1.25f;
            _initialMaxValue = 25;

            _earningMultiplier = 1;

            MaxValue = MinValue;
            SetValue( 0 );
        }       
    }
}