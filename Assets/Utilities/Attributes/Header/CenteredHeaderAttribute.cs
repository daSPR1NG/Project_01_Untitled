using UnityEngine;
using System;

namespace dnSR_Coding.Utilities.Attributes
{
    ///<summary> CenteredHeaderAttribute description <summary>
    [AttributeUsage( AttributeTargets.All, Inherited = true, AllowMultiple = true )]
    public class CenteredHeaderAttribute : PropertyAttribute
    {
        internal const float DEFAULT_HEIGHT = 20;
        internal const float Y_MIN_POS_VALUE = 5;

        public string Header { get; set; }
        public float Height { get; set; } = DEFAULT_HEIGHT;
        public float YMinPos { get; set; }  = Y_MIN_POS_VALUE;
        public TextAnchor TextAnchor { get; set; } = TextAnchor.MiddleLeft;

        public CenteredHeaderAttribute( 
            string header = "", 
            float height = DEFAULT_HEIGHT, 
            float yMinPos = Y_MIN_POS_VALUE, 
            TextAnchor textAnchor = TextAnchor.MiddleLeft )
        {
            Header = header.ToUpper();
            Height = height;
            YMinPos = yMinPos;
            TextAnchor = textAnchor;
        }
    }
}