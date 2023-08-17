using UnityEngine;
using System;

namespace dnSR_Coding
{
    ///<summary> TestFoldAttribute description <summary>
    [AttributeUsage( AttributeTargets.Class, Inherited = true, AllowMultiple = false )]
    public class TestFoldAttribute : PropertyAttribute
    {
        public TestFoldAttribute ()
        {
        
        }
    }
}