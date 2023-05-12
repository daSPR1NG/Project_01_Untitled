using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

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
        [SerializeField] private bool _isSunDisplayed;
        [SerializeField] private Enums.EnvironmentLightIntensity _environmentLightIntensity;

        private bool HasActiveRainModule => _rainType != Enums.RainType.None;
        private bool HasActiveThunderModule => _thunderType != Enums.ThunderType.None;
        private bool HasActiveFogModule => _fogType != Enums.FogType.None;

        public void Init()
        {
            IsActive = false;
        }

        public void Apply( ref GameObject rainGO )
        {
            if ( HasActiveRainModule )      { ApplyRain( ref rainGO ); }
            if ( HasActiveThunderModule )   { ApplyThunder(); }
            if ( HasActiveFogModule )       { ApplyFog(); }

            IsActive = true;
        }

        public void Stop( ref GameObject rainGO )
        {
            if ( HasActiveRainModule )      { StopRain( ref rainGO ); }
            if ( HasActiveThunderModule )   { StopThunder(); }
            if ( HasActiveFogModule )       { RemoveFog(); }

            IsActive = false;
        }

        #region Rain

        private void ApplyRain( ref GameObject rainGO )
        {
            if ( _rainType == Enums.RainType.None )
            {
                StopRain( ref rainGO );
                return;
            }

            rainGO.TryToDisplay();
            _rainModule.ApplySettings( _rainType, ref rainGO );
        }

        private void StopRain( ref GameObject rainGO)
        {
            _rainType = Enums.RainType.None;
            _rainModule.Stop( ref rainGO );
            rainGO.TryToHide();
        }

        #endregion

        #region Thunder

        private void ApplyThunder()
        {
            // Mettre ce qui se passe quand il y a de l'orage
        }

        private void StopThunder()
        {
            // L'inverse de ce qui est fait dans la méthode ApplyThunder
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
    }
}