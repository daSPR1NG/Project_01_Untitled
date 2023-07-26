using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary> ReadOnlyAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = true )]
    public class ReadOnlyAttribute : PropertyAttribute
    {
        public ReadOnlyAttribute () { }
    }
}