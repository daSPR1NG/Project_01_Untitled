using UnityEngine;

namespace dnSR_Coding
{
    ///<summary> ValidationAttribute description <summary>
    public sealed class ValidationAttribute : PropertyAttribute
    {
        public readonly string Text = string.Empty;

        public ValidationAttribute( string text )
        {
            Text = text;
        }
    }
}