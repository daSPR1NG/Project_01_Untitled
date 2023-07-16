using UnityEngine;
using System;

namespace dnSR_Coding.Utilities
{
    ///<summary> LabeledArrayAttribute description <summary>
    public class LabeledArrayAttribute : PropertyAttribute
    {
        public readonly string [] names;
        public LabeledArrayAttribute( string [] names ) { this.names = names; }
        public LabeledArrayAttribute( Type enumType ) { names = Enum.GetNames( enumType ); }
    }
}