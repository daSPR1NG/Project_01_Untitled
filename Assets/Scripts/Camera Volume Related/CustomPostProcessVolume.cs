using UnityEngine;
using UnityEngine.Rendering;

namespace dnSR_Coding
{
    [RequireComponent( typeof( CameraVolumeManager ) )]

    ///<summary> CustomPostProcessVolume description <summary>
    [Component("CustomPostProcessVolume", "")]
    [DisallowMultipleComponent]
    public class CustomPostProcessVolume : Volume {}
}