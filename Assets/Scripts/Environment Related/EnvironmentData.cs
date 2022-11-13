using Cinemachine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System;
using UnityEngine;

namespace dnSR_Coding
{
    [Serializable]
    public class EnvironmentData
    {
        [HideInInspector] public string Name;

        [AllowNesting, ReadOnly] public Environment EnvironmentComponent;

        public void SetEnvironmentComponent( Environment environment )
        {
            if ( EnvironmentComponent != environment
                || EnvironmentComponent.IsNull() )
            {
                EnvironmentComponent = environment;
            }
        }

        public EnvironmentData( Environment EnvironmentComponent )
        {
            this.EnvironmentComponent = EnvironmentComponent;
        }
        public EnvironmentData() : this( null ) { }
    }
}