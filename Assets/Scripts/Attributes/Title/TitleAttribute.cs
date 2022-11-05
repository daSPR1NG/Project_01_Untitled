using UnityEngine;
using System;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> HeaderAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
    sealed class TitleAttribute : PropertyAttribute
    {
        public readonly string Header;
        public readonly string ColorString;

        public readonly Color Color;
        public readonly float TextHeightIncrease;
        public readonly float FontSize;

        public TitleAttribute( string header, float fontsize, string colorString ) : this( header, fontsize, 1, colorString ) {}

        public TitleAttribute( string header, float fontsize, float textHeightIncrease = 1, string colorString = "white" )
        {
            Header = header.ToUpper();
            ColorString = colorString;

            FontSize = fontsize;

            TextHeightIncrease = TextHeightIncrease.Max( 1, textHeightIncrease );

            if ( string.IsNullOrEmpty( header ) ) TextHeightIncrease = 1f;

            if ( ColorUtility.TryParseHtmlString( colorString, out Color ) ) return;

            Color = new Color (0, 0, 0);
            ColorString = "white";
        }
    }
}