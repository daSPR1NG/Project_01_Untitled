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

        public bool IsFocused { get; private set; }
        public CinemachineVirtualCamera VirtualCamera { get; private set; }

        [AllowNesting, ReadOnly] public Environment EnvironmentParent;
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

            _parentTrs = EnvironmentParent.transform;
            if ( _parentTrs.gameObject.IsActive() ) { EnvironmentParent.ToggleEnvironmentVisibility( false ); }
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

            _parentTrs = EnvironmentParent.transform;
            if ( !_parentTrs.gameObject.IsActive() ) { EnvironmentParent.ToggleEnvironmentVisibility( true ); }
        }

        public EnvironmentCameraData( Environment environmentParent, CinemachineVirtualCamera cameraTrs, int priority )
        {
            EnvironmentParent = environmentParent;
            VirtualCamera = cameraTrs;
            Priority = priority;
        }
        public EnvironmentCameraData() : this( null, null, 0 ) { }
    }
}