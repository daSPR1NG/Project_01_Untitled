using UnityEngine;
using System;

namespace dnSR_Coding.Utilities.Attributes
{
    ///<summary>
    /// Overwrites the default Header attribute to redraw it as we want using the HeaderAttributeDrawer.
    ///<summary>
    [AttributeUsage( AttributeTargets.Field, Inherited = true, AllowMultiple = true )]
    public class CstmHeaderAttribute : PropertyAttribute
    {
        public readonly string Header;
        public readonly bool HasUnderline;

        public CstmHeaderAttribute( string header ) {
            Header = header.ToUpper();
            HasUnderline = false;
        }

        public CstmHeaderAttribute( string header, bool hasUnderline )
        {
            Header = header.ToUpper();
            HasUnderline = hasUnderline;
        }
    }
}