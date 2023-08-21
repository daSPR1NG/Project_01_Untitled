using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary> HiddenFieldAttribute description <summary>
    [AttributeUsage( AttributeTargets.All, Inherited = false, AllowMultiple = false )]
    public class HiddenFieldAttribute : PropertyAttribute
    {
        public HiddenFieldAttribute ()
        {
        
        }
    }
}