using UnityEngine;
using System.Collections;
using System;

namespace dnSR_Coding
{
    ///<summary> ComponentAttribute description <summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public string Name { get; private set; } = null;
        public string Description { get; private set; } = null;

        public ComponentAttribute( string name )
        {
            Name = name.ToUpper();
        }

        public ComponentAttribute( string name, string description ) : this ( name.ToUpper() )
        {
            Description = description;
        }
    }
}