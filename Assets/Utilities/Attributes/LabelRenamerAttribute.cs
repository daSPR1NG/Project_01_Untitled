using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary> LabelRenamerAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = false, AllowMultiple = false )]
    public class LabelRenamerAttribute : PropertyAttribute
    {
        public readonly string Abbreviation;

        public LabelRenamerAttribute( string abbreviation ) 
        {
            Abbreviation = abbreviation;
        }
    }
}