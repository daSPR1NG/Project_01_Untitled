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

        public Environment EnvironmentComponent { get; private set; }
        public Transform CameraTrs { get; private set; }

        [AllowNesting, ReadOnly] public Transform EnvironmentTrs;

        public void SetEnvironmentComponent( Environment environment )
        {
            if ( EnvironmentComponent != environment
                || EnvironmentComponent.IsNull() )
            {
                EnvironmentComponent = environment;
            }
        }

        public void SetCameraTransform( Transform trs )
        {
            if ( trs.IsNull() || CameraTrs == trs ) { return; }

            CameraTrs = trs;
        }
        public void SetVirtualCameraTransformName( string name )
        {
            if ( CameraTrs == null ) { return; }

            if ( CameraTrs.GetChild( 0 ).name != name ) { CameraTrs.GetChild( 0 ).name = name; }
        }

        public EnvironmentData( Transform EnvironmentTrs, Environment EnvironmentComponent, Transform CameraTrs )
        {
            this.EnvironmentTrs = EnvironmentTrs;
            this.EnvironmentComponent = EnvironmentComponent;
            this.CameraTrs = CameraTrs;
        }
        public EnvironmentData( Transform EnvironmentTrs ) : this( EnvironmentTrs, null, null ) { }
        public EnvironmentData() : this( null, null, null ) { }
    }
}