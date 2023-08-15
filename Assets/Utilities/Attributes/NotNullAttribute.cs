using UnityEngine;
using System;

namespace dnSR_Coding.Utilities.Attributes
{
    ///<summary> NotNullAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = true )]
    public class NotNullAttribute : PropertyAttribute
    {
        public NotNullAttribute () { }
    }
}