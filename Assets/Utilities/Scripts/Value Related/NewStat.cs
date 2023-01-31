using UnityEngine;
using System;
using System.Collections.Generic;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    [Serializable]
    public class NewStat : 
        IInitializedValue<float>, 
        IValue<float>, 
        IClampedValue<float>,
        IModifiableStatValue<float>
    {
        [SerializeField] private string _name;
        [SerializeField, ReadOnly] private float _value;

        [field: SerializeField] public float InitialValue { get; set; }

        [field: SerializeField] public bool HasMinValue { get; set; }
        [field: SerializeField] public bool HasMaxValue { get; set; }
        [field: SerializeField] public float MinValue { get; set; }
        [field: SerializeField] public float MaxValue { get; set; }

        public float Value { get => _value; set => _value = value; }

        [field: SerializeField] public List<StatModifier> StatModifiers { get; set; }
        public Action<float> OnStatValueModified { get; set; }

        public void InitializeValue( float initialValue )
        {
            _value = initialValue;
        }

        public float GetValue( float initialValue )
        {
            _value = initialValue;

            if ( StatModifiers.IsEmpty() )
            {
                _value = GetClampedValue( _value, HasMinValue, HasMaxValue );
                return _value;
            }

            for ( int i = 0; i < StatModifiers.Count; i++ )
            {
                StatModifiers [ i ].Apply( this );
            }

            _value = GetClampedValue( _value, HasMinValue, HasMaxValue );

            OnStatValueModified?.Invoke( _value );

            return _value;
        }

        public float GetClampedValue( float valueToClamp, bool hasMinValue, bool hasMaxValue )
        {
            valueToClamp = valueToClamp.Clamped(
                hasMinValue ? MinValue : valueToClamp,
                hasMaxValue ? MaxValue : valueToClamp );

            return valueToClamp;
        }

        public void SetNewMaxValue( float newMaxValue )
        {
            if ( MaxValue != newMaxValue ) { MaxValue = newMaxValue; }
        }
    }
}