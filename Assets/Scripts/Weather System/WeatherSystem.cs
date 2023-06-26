using UnityEngine;
using NaughtyAttributes;
using dnSR_Coding.Utilities;
using DG.Tweening;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [RequireComponent( typeof( EnvironmentLightsReferencer ) )]

    ///<summary> WeatherSystem description <summary>
    [DisallowMultipleComponent]
    public class WeatherSystem : MonoBehaviour, IEnvironmentLightsUser, IDebuggable
    {
        [Header( "Weather Presets" )]
        [SerializeField] private List<WeatherPreset> _weatherPresets = new();
        private WeatherPreset _activePreset = null;
        private GameObject _rainGO;

        public EnvironmentLightsReferencer EnvironmentLightsReferencer { get; set; }
        public LightController MainLightController { get; set; }
        public LightController AdditionalLightController { get; set; }
        private LightController _thunderLightController;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() 
        {
            DOTween.Kill( this );
        }

        #endregion

        private void Awake() => Init();
        private void Init()
        {
            SetDependencies();

            ToggleSunDisplay_OnApplyingPreset( false );
            ApplyWeatherPreset( index: 0 );
        }
        private void SetDependencies()
        {
            _rainGO = GameObject.FindGameObjectWithTag( "Environment/Rain" );
            SetLightsReference();
        }

        private void Update()
        {
            if ( KeyCode.T.IsPressed() )
            {
                _activePreset.StopThunder( this );
                _activePreset.ApplyThunder( this, _thunderLightController, MainLightController.GetControllerLight().color );
            }
        }

        /// <summary>
        /// Activates a weather preset, chosen by index.
        /// </summary>
        /// <param name="index"></param>
        public void ApplyWeatherPreset( int index )
        {
            // If the index is higher than the chosen index we throw an error and kill the method...
            if ( index > _weatherPresets.Count )
            {
                Debug.LogError( 
                    "The index you choose is higher than the preset amount, it's impossible to activate it", this );
                return;
            }

            // If no active preset is set, we set it...
            if ( _activePreset.IsNull() ) 
            {
                _activePreset = _weatherPresets [ index ];
                _activePreset.Init();

                this.Debugger( "Assign active preset" );
            }

            bool aPresetIsActive = !_activePreset.IsNull() && _activePreset.IsActive;

            // If a preset is already active and we still trying to aply it again, we kill the method...
            if ( aPresetIsActive && _activePreset == _weatherPresets [ index ] ) 
            {
                this.Debugger( "Killing the method a preset is active and is the same as the one already passed" );
                return; 
            }

            // If a preset is active but a new one is applied (different from the current one), we stop the current preset to properly activate the new one...
            if ( aPresetIsActive && _activePreset != _weatherPresets [ index ] )
            {
                _activePreset.Stop( monoBehaviour: this, ref _rainGO, MainLightController );
                // Active preset is reassigned and we apply the preset we want.
                _activePreset = _weatherPresets [ index ];
                _activePreset.Init();
            }

            _activePreset.Apply(
                monoBehaviour: this,
                ref _rainGO,
                MainLightController,
                _thunderLightController );

            EventManager.OnApplyingWeatherPreset?.Invoke( _activePreset );

            ToggleSunDisplay_OnApplyingPreset( _activePreset.IsSunHidden );
            this.Debugger( $"Assign active preset {_activePreset.name}" );
        }
        public void StopActiveWeatherPreset()
        {
            if ( _activePreset.IsNull() || !_activePreset.IsActive ) { return; }

            _activePreset.Stop( monoBehaviour: this, ref _rainGO, MainLightController );
            _activePreset = null;
        }

        private void ToggleSunDisplay_OnApplyingPreset( bool sunIsHidden )
        {
            AdditionalLightController.ToggleEnabledLightState( !sunIsHidden );
        }

        public void SetLightsReference()
        {
            EnvironmentLightsReferencer = GetComponent<EnvironmentLightsReferencer>();

            MainLightController = EnvironmentLightsReferencer.MainLightController;
            AdditionalLightController = EnvironmentLightsReferencer.AdditionalLightController;
            _thunderLightController = EnvironmentLightsReferencer.ThunderLightController;
        }

        #region On Editor

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetLightsReference();
        }        
#endif

        #endregion
    }
}