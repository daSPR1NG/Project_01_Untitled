using dnSR_Coding.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
public class ListedPropertyAutoNameAttribute : PropertyAttribute
{
    public readonly List<string> Names = new();
    public ListedPropertyAutoNameAttribute( Type enumType ) 
    {
        for ( int i = 0;  i < Enum.GetNames( enumType ).Length; i++ )
        {
            Names.AppendItem( Enum.GetNames( enumType ) [ i ] );
        }
    }
}