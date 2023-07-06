using UnityEngine;
using dnSR_Coding.Utilities;
using System;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Presets/Create New Weather Preset" )]
    public class WeatherPreset : ScriptableObject
    {
        public int ID => GetInstanceID();
        public bool IsActive { get; private set; } = false;

        [Header( "Rain details" )]
        [SerializeField] private RainModule _rainModule;
        [SerializeField] private Enums.RainType _rainType;

        [Header( "Thunder details" )]
        [SerializeField] private ThunderModule _thunderModule;
        [SerializeField] private Enums.ThunderType _thunderType;

        [Header( "Fog details" )]
        [SerializeField] private FogModule _fogModule;
        [SerializeField] private Enums.FogType _fogType;

        [Header( "Environment Light details" )]
        [SerializeField] private EnvironmentLightModule _environmentLightModule;
        [SerializeField] private Enums.EnvironmentLightIntensityType _environmentLightIntensityType;
        [field: SerializeField] public bool IsSunHidden { get; private set; }

        private bool HasActiveRainModule => _rainType != Enums.RainType.None;
        private bool HasActiveThunderModule => _thunderType != Enums.ThunderType.None;
        private bool HasActiveFogModule => _fogType != Enums.FogType.None;

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

            SetLightIntensity( Enums.EnvironmentLightIntensityType.Max, mainLightController );

            IsActive = false;
        }

        #region Main Light Intensity

        private void SetLightIntensity( Enums.EnvironmentLightIntensityType lightIntensity, LightController mainLightController )
        {
            _environmentLightModule.ApplySettings( lightIntensity, mainLightController );
        }

        #endregion

        public (RainModule module, Enums.RainType type) GetRainInfo() {
            return (_rainModule, _rainType);
        }
        public (ThunderModule module, Enums.ThunderType type) GetThunderInfo() {
            return (_thunderModule, _thunderType);
        }
        public (FogModule module, Enums.FogType type) GetFogInfo() {
            return (_fogModule, _fogType);
        }
        public (EnvironmentLightModule module, Enums.EnvironmentLightIntensityType type) GetEnvironmentLightInfo() {
            return (_environmentLightModule, _environmentLightIntensityType);
        }
    }
}