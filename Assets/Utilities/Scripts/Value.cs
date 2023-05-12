using dnSR_Coding;
using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class Value<T> : IMinimizedValue<T>, IMaxedValue<T>
{
    [field: SerializeField] 
    public T Current { get; set; } = default;

    [field: SerializeField]
    public bool IsClamped { get; set; } = false;

    [field: SerializeField, AllowNesting, ShowIf( "IsClamped" )]
    public bool HasMinValue { get; set; } = false;

    [field: SerializeField, AllowNesting, ShowIf( EConditionOperator.And, "IsClamped", "HasMin" )]
    public T MinValue { get; set; } = default;

    [field: SerializeField, AllowNesting, ShowIf( "IsClamped" )]
    public bool HasMaxValue { get; set; } = false;

    [field: SerializeField, AllowNesting, ShowIf( EConditionOperator.And, "IsClamped", "HasMax" )] 
    public T MaxValue { get; set; } = default;

    public void SetMaxValue( T value )
    {
        MaxValue = value;
    }

    public void SetMinValue( T value )
    {
        MinValue = value;
    }
}
