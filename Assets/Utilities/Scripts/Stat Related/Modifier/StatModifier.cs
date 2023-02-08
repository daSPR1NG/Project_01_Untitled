using UnityEngine;
using System;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    [Serializable]
    public class StatModifier : Modifier
    {
        public override void Apply( object valueToModify )
        {
            Stat modifiedStat = ( Stat ) valueToModify;

            float value = modifiedStat.Value;
            float modifierValue = _modifierType == ModifierType.FLAT ? Value : Value / 100;

            switch ( _modifierType )
            {
                case ModifierType.FLAT:
                    switch ( _operand )
                    {
                        case Operand.PLUS:
                            value += modifierValue;
                            break;

                        case Operand.MINUS:
                            value -= modifierValue;
                            break;

                        case Operand.DIVIDE:
                            if ( modifierValue == 0 )
                            {
                                value = 0;
                                Debug.LogError( "Can't divide by zero." );
                            }

                            value /= modifierValue;
                            break;

                        case Operand.MULTIPLICATE:
                            value *= modifierValue;
                            break;
                    }
                    break;

                case ModifierType.PERCENTAGE:
                    value *= modifierValue;
                    break;

                case ModifierType.ADDITIVE_PERCENTAGE:
                    value += value * modifierValue;
                    break;
            }

            modifiedStat.SetValue( ExtMathfs.FloorToInt( value ) );
        }

        #region Constructors

        public StatModifier() : base() { }
        public StatModifier( Operand operand, ModifierType modifierType, int value ) : base()
        {
            _operand = operand;
            _modifierType = modifierType;
            Value = value;
        }

        #endregion
    }
}