using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/Camera Volume/VolumeProfileSettings")]
    public class VolumeProfileSettings : ScriptableObject
    {
        [SerializeField] private List<VolumeProfileData> _volumeProfileDatas = new();
        public List<VolumeProfileData> VolumeProfileDatas => _volumeProfileDatas;
    }
}