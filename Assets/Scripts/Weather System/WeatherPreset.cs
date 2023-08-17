using UnityEngine;
using dnSR_Coding.Utilities.Helpers;
using Sirenix.OdinInspector;
using dnSR_Coding.Utilities.Attributes;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Presets/Create New Weather Preset" )]
    [InlineEditor( InlineEditorObjectFieldModes.Foldout )]
    public class WeatherPreset : ScriptableObject
    {
        public int ID => GetInstanceID();
        public bool IsActive { get; private set; } = false;

        #region Rain module

        [CenteredHeader( Header = "Rain details", TextAnchor = TextAnchor.MiddleLeft )]
        [SerializeField] private RainModule _rainModule;
        [SerializeField] private Enums.Rain_Type _rainType;

        #endregion

        #region Thunder module

        [CenteredHeader( Header = "Thunder details", TextAnchor = TextAnchor.MiddleLeft )]
        [SerializeField] private ThunderModule _thunderModule;
        [SerializeField] private Enums.Thunder_Type _thunderType;

        #endregion

        #region Fog module

        [CenteredHeader( Header = "Fog details" )]
        [SerializeField] private FogModule _fogModule;
        [SerializeField] private Enums.Fog_Type _fogType;

        #endregion

        #region Light module

        [CenteredHeader( Header = "Lighting details" )]
        [SerializeField] private EnvironmentLightModule _environmentLightModule;
        [SerializeField] private Enums.Environment_LightIntensity_Type _environmentLightIntensityType;
        [field: SerializeField] public bool IsSunHidden { get; private set; }

        #endregion        

        private bool HasActiveRainModule => _rainType != Enums.Rain_Type.None;
        private bool HasActiveThunderModule => _thunderType != Enums.Thunder_Type.None;
        private bool HasActiveFogModule => _fogType != Enums.Fog_Type.None;

        public void Init() {
            IsActive = false;
        }

        public void Apply( MonoBehaviour monoBehaviour, GameObject rainGO, LightController mainLightController, LightController thunderLightController )
        {
            SetLightIntensity( _environmentLightIntensityType, mainLightController );

            if ( HasActiveRainModule ) { _rainModule.ApplySettings( _rainType, rainGO ); }
            else { _rainModule.Stop( rainGO ); }

            if ( HasActiveThunderModule ) { _thunderModule.ApplySettings( monoBehaviour, _thunderType, thunderLightController, mainLightController.GetControllerLight().color ); }
            else { _thunderModule.Stop( monoBehaviour ); }

            if ( HasActiveFogModule ) { _fogModule.ApplySettings( _fogType ); }

            IsActive = true;
        }
        public void Stop( MonoBehaviour monoBehaviour, GameObject rainGO, LightController mainLightController )
        {
            if ( HasActiveRainModule )      { _rainModule.Stop( rainGO ); }
            if ( HasActiveThunderModule )   { _thunderModule.Stop( monoBehaviour ); }
            if ( HasActiveFogModule )       { _fogModule.Stop(); }

            SetLightIntensity( Enums.Environment_LightIntensity_Type.Max, mainLightController );

            IsActive = false;
        }

        #region Main Light Intensity

        private void SetLightIntensity( Enums.Environment_LightIntensity_Type lightIntensity, LightController mainLightController )
        {
            _environmentLightModule.ApplySettings( lightIntensity, mainLightController );
        }

        #endregion

        public (RainModule module, Enums.Rain_Type type) GetRainInfo() {
            return (_rainModule, _rainType);
        }
        public (ThunderModule module, Enums.Thunder_Type type) GetThunderInfo() {
            return (_thunderModule, _thunderType);
        }
        public (FogModule module, Enums.Fog_Type type) GetFogInfo() {
            return (_fogModule, _fogType);
        }
        public (EnvironmentLightModule module, Enums.Environment_LightIntensity_Type type) GetEnvironmentLightInfo() {
            return (_environmentLightModule, _environmentLightIntensityType);
        }
    }
}