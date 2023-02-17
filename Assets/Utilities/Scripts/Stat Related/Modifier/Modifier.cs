using UnityEngine;

namespace dnSR_Coding
{
    public abstract class Modifier
    {
        [SerializeField] protected Enums.Operand _operand = Enums.Operand.PLUS;

        [field: SerializeField] public float Value { get; set; }
        public abstract void Apply( object valueToModify );
    }
}