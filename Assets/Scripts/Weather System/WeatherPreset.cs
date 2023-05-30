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

        public void Init()
        {
            IsActive = false;
        }

        public void Apply( MonoBehaviour monoBehaviour, ref GameObject rainGO, Light mainLightRef, Light thunderLight )
        {
            SetLightIntensity( _environmentLightIntensityType, mainLightRef );

            if ( HasActiveRainModule ) { ApplyRain( ref rainGO ); }
            else { StopRain( ref rainGO ); }

            if ( HasActiveThunderModule ) { ApplyThunder( monoBehaviour, thunderLight ); }
            if ( HasActiveFogModule ) { ApplyFog(); }

            IsActive = true;
        }
        public void Stop( MonoBehaviour monoBehaviour, ref GameObject rainGO, Light mainLightRef )
        {
            if ( HasActiveRainModule )      { StopRain( ref rainGO ); }
            if ( HasActiveThunderModule )   { StopThunder( monoBehaviour ); }
            if ( HasActiveFogModule )       { RemoveFog(); }

            SetLightIntensity( Enums.EnvironmentLightIntensityType.Max, mainLightRef );

            IsActive = false;
        }

        #region Rain

        private void ApplyRain( ref GameObject rainGO )
        {
            rainGO.TryToDisplay();
            _rainModule.ApplySettings( _rainType, ref rainGO );
        }

        private void StopRain( ref GameObject rainGO)
        {
            _rainModule.Stop( ref rainGO );
            rainGO.TryToHide();
        }

        #endregion

        #region Thunder

        // Remettre en privé après test !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public void ApplyThunder( MonoBehaviour monoBehaviour, Light thunderLight )
        {
            _thunderModule.ApplySettings( monoBehaviour, _thunderType, thunderLight );
        }

        // Remettre en privé après test !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public void StopThunder( MonoBehaviour monoBehaviour )
        {
            _thunderModule.Stop( monoBehaviour );
        }

        #endregion

        #region Fog

        private void ApplyFog()
        {
            _fogModule.ApplySettings( _fogType );
        }

        private void RemoveFog()
        {
            _fogModule.Stop();
        }

        #endregion

        #region Main Light Intensity

        private void SetLightIntensity( Enums.EnvironmentLightIntensityType lightIntensity, Light mainLightRef )
        {
            _environmentLightModule.ApplySettings( lightIntensity, mainLightRef );
        }

        #endregion

        public (RainModule module, Enums.RainType type) GetRainInfo()
        {
            return (_rainModule, _rainType);
        }
        public (ThunderModule module, Enums.ThunderType type) GetThunderInfo()
        {
            return (_thunderModule, _thunderType);
        }
        public (FogModule module, Enums.FogType type) GetFogInfo()
        {
            return (_fogModule, _fogType);
        }
        public (EnvironmentLightModule module, Enums.EnvironmentLightIntensityType type) GetEnvironmentLightInfo()
        {
            return (_environmentLightModule, _environmentLightIntensityType);
        }
    }
}