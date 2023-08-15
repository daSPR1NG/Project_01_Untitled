using UnityEngine;
using System;

namespace dnSR_Coding.Utilities.Attributes
{
    ///<summary> ReadOnlyAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = true )]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public ReadOnlyAttribute () { }
    }
}