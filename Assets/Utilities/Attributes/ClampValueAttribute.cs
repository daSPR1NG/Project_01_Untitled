using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary> ClampValueAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    public class ClampValueAttribute : PropertyAttribute
    {
        public ClampValueAttribute () {}
    }
}