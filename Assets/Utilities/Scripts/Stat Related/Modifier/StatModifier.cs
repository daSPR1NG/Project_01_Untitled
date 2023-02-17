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

            switch ( _operand )
            {
                case Enums.Operand.PLUS:
                    value += Value;
                    break;

                case Enums.Operand.MINUS:
                    value -= Value;
                    break;
            }

            modifiedStat.SetValue( ExtMathfs.FloorToInt( value ) );
        }

        #region Constructors

        public StatModifier() : base() { }
        public StatModifier( Enums.Operand operand, int value ) : base()
        {
            _operand = operand;
            Value = value;
        }

        #endregion
    }
}