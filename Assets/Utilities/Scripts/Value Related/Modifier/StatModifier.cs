using UnityEngine;
using System;

namespace dnSR_Coding
{
    [Serializable]
    public class StatModifier : Modifier
    {
        public override void Apply( object valueToModify )
        {
            NewStat modifiedStat = ( NewStat ) valueToModify;

            float value = modifiedStat.Value;
            float modifierValue = _modType == ModType.FLAT ? Value : Value / 100;

            switch ( _modType )
            {
                case ModType.FLAT:
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

                case ModType.PERCENTAGE:
                    value *= modifierValue;
                    break;

                case ModType.ADDITIVE_PERCENTAGE:
                    value += value * modifierValue;
                    break;
            }

            modifiedStat.Value = value;
        }
    }
}