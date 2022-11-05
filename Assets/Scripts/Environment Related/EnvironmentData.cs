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

        public bool Displayed = true;
        public Transform EnvironmentTrs;

        [AllowNesting, ReadOnly] public Environment EnvironmentComponent;
        [AllowNesting, ReadOnly] public Transform CameraTrs;
        public void SetCameraDatas( string name, Transform trs )
        {
            SetCameraPivotChildName( name );
            SetCameraTransform( trs );
        }
        public void SetEnvironmentComponentReference( Environment environment )
        {
            if ( EnvironmentComponent != environment
                || EnvironmentComponent.IsNull() )
            {
                EnvironmentComponent = environment;
            }
        }

        public void SetName( string name )
        {
            if ( Name != name ) { Name = name; }
        }

        private void SetCameraPivotChildName( string name )
        {
            if ( CameraTrs == null ) { return; }

            if ( CameraTrs.GetChild( 0 ).name != name ) { CameraTrs.GetChild( 0 ).name = name; }
        }
        private void SetCameraTransform( Transform trs )
        {
            if ( trs.IsNull() || CameraTrs == trs ) { return; }

            CameraTrs = trs;
        }
    }
}