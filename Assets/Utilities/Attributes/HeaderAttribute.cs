using UnityEngine;
using System;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> HeaderAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
    public class HeaderAttribute : PropertyAttribute
    {
        public readonly string Header;

        public HeaderAttribute( string header )
        {
            Header = header.ToUpper();
        }
    }
}