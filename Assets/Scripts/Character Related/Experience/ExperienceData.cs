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
        [Tooltip( "This value is used to calculate the new max value on level up." )]
        [SerializeField] private float _scalingFactor = 1.25f;
        [Tooltip( "This value is the max exp required for the first level, it is used after on level up when calculating the new max value." )]
        [SerializeField] private int _initialMaxValue = 25;

        [Header( "Multiplier Settings" )]
        [SerializeField] private float _multiplier = 1;

        [field: SerializeField, ReadOnly, AllowNesting] public int Value { get; private set; }

        public bool HasMinValue { get => true; }
        public bool HasMaxValue { get => true; }

        public int MinValue => _initialMaxValue;
        [field: SerializeField, ReadOnly, AllowNesting] public int MaxValue { get; private set; }

        public float Multiplier => _multiplier;

        public void LevelUp()
        {
            _level++;            
            UpgradeMaxValue();

            //Debug.Log( "Level of " + LinkedStatType.ToString() + " : " + _level );
        }

        public void AddToCurrentValue( int valueToAdd )
        {
            for ( int i = Helper.MultipliedValue( valueToAdd, Multiplier ) - 1; i >= 0; i-- )
            {
                Value += 1;

                if ( Value >= MaxValue )
                {
                    SetCurrentValue( 0 );
                    LevelUp();
                    NotifyObservers( this );
                }

                SetCurrentValue( Value );                
            }
        }
        public void SetCurrentValue( int newValue )
        {
            Value = newValue;
            //Debug.Log( "Current experience value of " + LinkedStatType.ToString() + " : " + Value + " / " + MaxValue );
        }

        public void UpgradeMaxValue()
        {
            float raisedValue = Mathf.Pow( ( _initialMaxValue * _level ), _scalingFactor );

            SetNewMaxValue( ExtMathfs.FloorToInt( raisedValue ) );

            //Debug.Log( "Max experience value of " + LinkedStatType.ToString() + " : " + MaxValue );
        }

        public void SetNewMaxValue( int newMaxValue )
        {
            if ( MaxValue != newMaxValue ) { MaxValue = newMaxValue; }
        }

        public int GetLevel() => _level;

        public void ResetToDefault()
        {
            _level = 0;

            _scalingFactor = 1.25f;
            _initialMaxValue = 25;

            _multiplier = 1;

            MaxValue = MinValue;
            SetCurrentValue( 0 );

            //Debug.Log( "Reset " + LinkedStatType.ToString() + " experience data to default." );
        }       
    }
}