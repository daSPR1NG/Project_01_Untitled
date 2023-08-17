using dnSR_Coding.Utilities.Attributes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace dnSR_Coding
{
    [CreateAssetMenu( fileName = "", menuName = "Scriptable Objects/Night Day Cycle/LightingPreset" )]
    [InlineEditor( InlineEditorObjectFieldModes.Foldout )]
    public class AmbientLightingPreset : ScriptableObject
    {
        [field: SerializeField, BoxGroup( "Settings" )] 
        public Gradient AmbientColor { get; private set; }

        [field: SerializeField, BoxGroup( "Settings" )] 
        public Gradient DirectionalColor { get; private set; }

        [field: SerializeField, BoxGroup( "Settings" )] 
        public Gradient FogColor { get; private set; }
    }
}