using UnityEngine;
using System;
using System.Collections.Generic;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    public enum StatType { Unassigned, Strength, Endurance, Dexterity, }

    [Serializable]
    public class Stat :
        IInitializedValue<float>,
        IValue<float>,
        IClampedValue<float>,
        IModifiableStatValue<float>
    {
        [HideInInspector] public string Name;

        [SerializeField] private StatType _type;

        [field: SerializeField] public float InitialValue { get; set; }

        [field: SerializeField] public bool HasMinValue { get; set; }
        [field: SerializeField] public bool HasMaxValue { get; set; }
        [field: SerializeField] public float MinValue { get; set; }
        [field: SerializeField] public float MaxValue { get; set; }

        public float Value { get; set; }

        [field: SerializeField] public List<StatModifier> StatModifiers { get; set; }

        public void InitializeValue( float initialValue )
        {
            Value = initialValue;
        }

        public float GetValue( float initialValue )
        {
            Value = initialValue;

            if ( StatModifiers.IsEmpty() )
            {
                Value = GetClampedValue( Value, HasMinValue, HasMaxValue );
                return Value;
            }

            for ( int i = 0; i < StatModifiers.Count; i++ )
            {
                StatModifiers [ i ].Apply( this );
            }

            Value = GetClampedValue( Value, HasMinValue, HasMaxValue );

            return Value;
        }

        public float GetClampedValue( float valueToClamp, bool hasMinValue, bool hasMaxValue )
        {
            valueToClamp = valueToClamp.Clamped(
                hasMinValue ? MinValue : valueToClamp,
                hasMaxValue ? MaxValue : valueToClamp );

            return valueToClamp;
        }
        public StatType GetStatType() => _type;

        public void SetNewMaxValue( float newMaxValue ) => MaxValue = newMaxValue;

        #region Constructors

        public Stat() : base() { }
        public Stat( StatType type, int value ) : base()
        {
            Name = type.ToString();
            _type = type;
            Value = value;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        public void SetName()
        {
            string name = _type.ToString() + " - " + Value.ToString();
            if ( !Name.Equals( name ) ) { Name = name; }
        }
#endif
        #endregion
    }
}