using Cinemachine;
using ExternalPropertyAttributes;
using System;
using UnityEngine;

namespace dnSR_Coding
{
    [Serializable]
    public class EnvironmentCameraData
    {
        [HideInInspector] public string Name;
        [HideInInspector] public bool IsFocused = false;

        [AllowNesting, ReadOnly] public Environment EnvironmentParent;
        [AllowNesting, ReadOnly] public int Priority;
        [AllowNesting, ReadOnly] public CinemachineVirtualCamera VirtualCamera;

        public void ResetFocus()
        {
            Priority = 10;
            VirtualCamera.Priority = Priority;

            IsFocused = false;
            VirtualCamera.enabled = IsFocused;
        }

        public void Focus()
        {
            Priority = 20;
            VirtualCamera.Priority = Priority;

            IsFocused = true;
            VirtualCamera.enabled = IsFocused;
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