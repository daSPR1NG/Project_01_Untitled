using Cinemachine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System;
using UnityEngine;

namespace dnSR_Coding
{
    [Serializable]
    public class EnvironmentCameraData
    {
        [HideInInspector] public string Name;

        [AllowNesting, ReadOnly] public bool IsFocused;
        [AllowNesting, ReadOnly] public CinemachineVirtualCamera VirtualCamera;
        [AllowNesting, ReadOnly] public Environment EnvironmentComponent;
        [AllowNesting, ReadOnly] public int Priority;

        private Transform _parentTrs;

        public void ResetFocus()
        {
            if ( VirtualCamera.IsNull() )
            {
                Helper.Log( this, "No Virtual camera set for this environment camera data !", Helper.LogType.Error );
                return;
            }

            Priority = 10;
            VirtualCamera.Priority = Priority;

            IsFocused = false;
            VirtualCamera.enabled = IsFocused;

            _parentTrs = EnvironmentComponent.transform;
            if ( _parentTrs.gameObject.IsActive() ) { EnvironmentComponent.ToggleEnvironmentVisibility( false ); }
        }
        public void Focus()
        {
            if ( VirtualCamera.IsNull() )
            {
                Helper.Log( this, "No Virtual camera set for this environment camera data !", Helper.LogType.Error );
                return;
            }

            Priority = 20;
            VirtualCamera.Priority = Priority;

            IsFocused = true;
            VirtualCamera.enabled = IsFocused;

            _parentTrs = EnvironmentComponent.transform;
            if ( !_parentTrs.gameObject.IsActive() ) { EnvironmentComponent.ToggleEnvironmentVisibility( true ); }
        }

        public EnvironmentCameraData( Environment environmentParent, CinemachineVirtualCamera cameraTrs, int priority )
        {
            EnvironmentComponent = environmentParent;
            VirtualCamera = cameraTrs;
            Priority = priority;
        }
        public EnvironmentCameraData() : this( null, null, 0 ) { }
    }
}