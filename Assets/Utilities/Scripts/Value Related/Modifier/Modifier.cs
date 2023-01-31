using UnityEngine;

namespace dnSR_Coding
{
    public enum Operand { PLUS, MINUS, DIVIDE, MULTIPLICATE }
    public enum ModType { FLAT, PERCENTAGE, ADDITIVE_PERCENTAGE }

    public abstract class Modifier : IValue<float>
    {
        [SerializeField] protected Operand _operand = Operand.PLUS;
        [SerializeField] protected ModType _modType = ModType.FLAT;

        [field: SerializeField] public float Value { get; set; }
        public abstract void Apply( object valueToModify );
    }
}