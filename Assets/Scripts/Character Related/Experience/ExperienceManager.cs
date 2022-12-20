using UnityEngine;
using ExternalPropertyAttributes;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> ExperienceManager description <summary>    
    public class ExperienceManager : ScriptableObject, IDebuggable
    {
        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion        

        /// <summary>
        /// Adds flat value to current EXP.
        /// </summary>
        /// <param name="amount"> The raw value </param>
        /// <returns> A floored absolute value. </returns>
        protected int GreaterFlatValue( int amount )
        {
            return ExtMathfs.Abs( amount );
        }

        /// <summary>
        /// Calculates an amount of EXP, affected by a multiplier and adds it to the current EXP.
        /// </summary>
        /// <param name="amount"> The raw value </param>
        /// <param name="multiplier"> The multiplier, less or equal to 1 </param>
        /// <returns> A floored absolute value. </returns>
        protected int MultipliedValue( int amount, float multiplier )
        {
            amount = ExtMathfs.Abs( ExtMathfs.FloorToInt( amount * multiplier ) );
            return ExtMathfs.Abs( amount );
        }

        /// <summary>
        /// Calculates the remaining amount of exp when an amount is added.
        /// </summary>
        /// <param name="current"></param>
        /// <param name="amountAdded"></param>
        /// <param name="maxValue"></param>
        /// <returns> The remaining amount of exp to add to current exp. </returns>
        protected int RemainingValue( int current, int amountAdded, int maxValue )
        {
            int value = ( current + amountAdded ) - maxValue;
            return value;
        }

        protected int PositiveValue( int amount )
        {
            return ExtMathfs.Abs( amount );
        }
    }
}