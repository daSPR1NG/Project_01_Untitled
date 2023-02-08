using UnityEngine;

namespace dnSR_Coding
{
    public enum Operand { PLUS, MINUS, DIVIDE, MULTIPLICATE }
    public enum ModifierType { FLAT, PERCENTAGE, ADDITIVE_PERCENTAGE }

    public abstract class Modifier
    {
        [SerializeField] protected Operand _operand = Operand.PLUS;
        [SerializeField] protected ModifierType _modifierType = ModifierType.FLAT;

        [field: SerializeField] public float Value { get; set; }
        public abstract void Apply( object valueToModify );
    }
}