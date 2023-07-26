using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary> NotNullAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = true )]
    public class NotNullAttribute : PropertyAttribute
    {
        public NotNullAttribute () { }
    }
}