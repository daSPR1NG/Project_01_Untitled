using UnityEngine;
using System;

namespace dnSR_Coding.Utilities.Attributes
{
    ///<summary> CenteredHeaderAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
    public class CenteredHeaderAttribute : PropertyAttribute
    {
        private const float DEFAULT_HEIGHT = 20;

        public readonly string Header;
        public readonly float Height = DEFAULT_HEIGHT;
        public readonly bool IsIndented  = false;

        public CenteredHeaderAttribute( string header )
        {
            Header = header.ToUpper();
            Height = DEFAULT_HEIGHT;
            IsIndented = false;
        }

        public CenteredHeaderAttribute( string header, float height )
        {
            Header = header.ToUpper();
            Height = height;
            IsIndented = false;
        }

        public CenteredHeaderAttribute( string header, bool isIndented )
        {
            Header = header.ToUpper();
            Height = DEFAULT_HEIGHT;
            IsIndented = isIndented;
        }

        public CenteredHeaderAttribute( string header, float height, bool isIndented )
        {
            Header = header.ToUpper();
            Height = height;
            IsIndented = isIndented;
        }
    }
}