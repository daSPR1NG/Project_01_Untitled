using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary> LimitArraySizeAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
    public class LimitArraySizeAttribute : PropertyAttribute
    {
        public readonly string [] Names;
        public readonly bool IsSizeOverriden;
        public LimitArraySizeAttribute( Type type, bool isSizeOverriden = false )
        {
            Names = Enum.GetNames( type );
            IsSizeOverriden = isSizeOverriden;
        }
    }
}