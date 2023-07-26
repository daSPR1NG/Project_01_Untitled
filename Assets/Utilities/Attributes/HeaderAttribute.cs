using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary>
    /// Overwrites the default Header attribute to redraw it as we want using the HeaderAttributeDrawer.
    ///<summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
    public class HeaderAttribute : PropertyAttribute
    {
        public readonly string Header;
        public readonly bool HasUnderline;

        public HeaderAttribute( string header ) {
            Header = header.ToUpper();
            HasUnderline = false;
        }

        public HeaderAttribute( string header, bool hasUnderline )
        {
            Header = header.ToUpper();
            HasUnderline = hasUnderline;
        }
    }
}