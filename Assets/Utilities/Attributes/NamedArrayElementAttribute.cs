using UnityEngine;
using System;

namespace dnSR_Coding.Attributes
{
    ///<summary>
    /// .
    ///<summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
    public class NamedArrayElementAttribute : PropertyAttribute
    {
        public readonly string [] Names;
        public readonly bool ExcludeFirstIndex;

        public NamedArrayElementAttribute( Type type, bool excludeFirstIndex = false )
        {
            Names = Enum.GetNames( type );
            ExcludeFirstIndex = excludeFirstIndex;
        }
    }
}