using UnityEngine;
using System;

namespace dnSR_Coding.Utilities
{
    ///<summary> CenteredHeaderAttribute description <summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
    public class CenteredHeaderAttribute : PropertyAttribute
    {
        private const float DEFAULT_HEIGHT = 18;

        public readonly string Header;
        public float Height { get; private set; }
        public bool IsIndented { get; private set; } = false;

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