using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    [System.Serializable]
    public class ExperienceData
    {
        [HideInInspector] public string Name = "[TYPE HERE]";

        [Header( "Details" )]
        [SerializeField] private bool _isDebuggable = true;
        [SerializeField] private StatType _linkedStatType = StatType.Unassigned;

        [Header( "Scaling Settings" )]
        [Tooltip( "This value is used to calculate the new max value on level up." )]
        [SerializeField] private float _scalingFactor = 1.25f;
        [Tooltip( "This value is the max exp required for the first level, it is used after on level up when calculating the new max value." )]
        [SerializeField] private int _initialMaxValue = 25;

        [Header( "Multiplier Settings" )]
        [SerializeField] private float _multiplier = 1;

        private int _level;
        private int _currentValue;
        private int _maxValue;

        public int Level => _level;
        public int CurrentValue { get => _currentValue; private set => _currentValue = value; }
        public int MaxValue { get => _maxValue; private set => _maxValue = value; }
        public StatType LinkedStatType => _linkedStatType;
        public float Multiplier => _multiplier;

        public void LevelUp()
        {
            _level++;
            Debug.Log( "Level of " + LinkedStatType.ToString() + " : " + _level );
        }

        public void AddToCurrentValue( int valueToAdd )
        {
            SetCurrentValue( CurrentValue + valueToAdd );
            Debug.Log( "Earned value for " + LinkedStatType.ToString() + " : " + valueToAdd );
        }
        public void SetCurrentValue( int newValue )
        {
            CurrentValue = newValue;
            Debug.Log( "Current experience value of " + LinkedStatType.ToString() +  " : " + CurrentValue + " / " + MaxValue );
        }

        public void UpgradeMaxValue()
        {
            float raisedValue = Mathf.Pow( ( _initialMaxValue * _level ), _scalingFactor );

            MaxValue = ExtMathfs.FloorToInt( raisedValue );

            Debug.Log( "Max experience value of " + LinkedStatType.ToString() + " : " + MaxValue );
        }

        public void ResetToDefault()
        {
            _level = 0;

            _scalingFactor = 1.25f;
            _initialMaxValue = 25;

            _multiplier = 1;

            MaxValue = _initialMaxValue;
            SetCurrentValue( 0 );

            Debug.Log( "Reset " + LinkedStatType.ToString() + " experience data to default." );
        }

#if UNITY_EDITOR
        public void SetName()
        {
            string newName = LinkedStatType.ToString();
            if ( !Name.Equals( newName ) ) { Name = newName; }
        }
#endif
    }
}