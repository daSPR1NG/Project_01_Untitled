using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu(fileName = "", menuName = "Scriptable Objects/Night Day Cycle/LightingPreset")]
    public class AmbientLightingPreset : ScriptableObject
    {
        [field: Header( "GRADIENTS SETTINGS" )]
        [field: SerializeField] public Gradient AmbientColor { get; private set;}
        [field: SerializeField] public Gradient DirectionalColor { get; private set; }
        [field: SerializeField] public Gradient FogColor { get; private set; }
    }
}