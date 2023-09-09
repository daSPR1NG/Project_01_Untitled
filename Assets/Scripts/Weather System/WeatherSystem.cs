using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Interfaces;
using dnSR_Coding.Utilities.Attributes;
using Sirenix.OdinInspector;
using System;

namespace dnSR_Coding
{
    [RequireComponent( typeof( EnvironmentLightsReferencer ) )]

    ///<summary> WeatherSystem description <summary>
    [DisallowMultipleComponent]
    public class WeatherSystem : MonoBehaviour, IEnvironmentLightsUser, IDebuggable
    {
        [CenteredHeader( "Weather Presets", LeftDecorationColor = EditorColor.Red )]

        [ListDrawerSettings( 
            ShowItemCount = true, DraggableItems = false, ShowIndexLabels = false, HideRemoveButton = true )]
        [SerializeField]
        private List<WeatherPreset> _weatherPresets = new();

        private WeatherPreset _currentPreset = null;
        private GameObject _rainGO;

        #region Lights fields

        public EnvironmentLightsReferencer EnvironmentLightsReferencer { get; set; }
        public LightController MainLightController { get; set; }
        public LightController AdditionalLightController { get; set; }
        private LightController _thunderLightController;

        #endregion

        #region DEBUG

        [field: SerializeField, FoldoutGroup( "Debug Section", Order = -1 )]
        public bool IsDebuggable { get; set; } = true;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable()
        {
            DOTween.Kill( this );
        }

        #endregion

        #region Setup

        private void Start() => Init();
        private void Init()
        {
            GetDependencies();

            ToggleSunDisplay_OnApplyingPreset( false );
            SetWeatherPreset( index: 0 );
        }
        private void GetDependencies()
        {
            _rainGO = GameObject.FindGameObjectWithTag( "Environment/Rain" );
            SetLightReferences();
        }

        #endregion

        // AS A DEBUG PURPOSE, PLS REMOVE AFTER
        private void Update()
        {
            if ( KeyCode.D.IsPressed() )
            {
                SetWeatherPreset( 0 );
            }

            if ( KeyCode.S.IsPressed() )
            {
                StopCurrentWeatherPreset( false );
            }
        }

        /// <summary>
        /// Activates a weather preset, chosen by index.
        /// </summary>
        /// <param name="index"></param>
        public void SetWeatherPreset( int index )
        {
            // We need to check if a preset is set and active...
            // and if it is the same as the one we want to activate.
            bool aPresetIsActive = !_currentPreset.IsNull() && _currentPreset.IsActive;
            if ( aPresetIsActive 
                && _currentPreset == _weatherPresets [ index ] )
            {
                this.Debugger( "Killing the method a preset is active and is the same as the one already passed" );
                return;
            }

            // We need to check if the index is higher than the list count.
            if ( index > _weatherPresets.Count )
            {
                this.Debugger(
                    "The index you choose is higher than the preset amount, it's impossible to activate it",
                    DebugType.Error );
                return;
            }

            WeatherPreset newWeatherPreset = _weatherPresets [ index ];

            // We need to check if a preset is set.
            if ( _currentPreset.IsNull() )
            {
                // No preset is set so we need to set it here.
                _currentPreset = newWeatherPreset;

                this.Debugger(
                    $"Weather preset was null so another one corresponding to " +
                    $"{index} has been assigned" );
            }

            // If a preset is active but a new one is applied (different from the current one), ...
            // we stop the current preset to properly activate the new one.
            if ( aPresetIsActive && _currentPreset != newWeatherPreset )
            {
                StopCurrentWeatherPreset( true, index );
            }

            // Then actually apply the preset.
            ApplyCurrentWeatherPreset();
        }

        #region Apply / Stop current preset

        /// <summary>
        /// Applies the weather preset.
        /// </summary>
        private void ApplyCurrentWeatherPreset()
        {
            // First we need to check if all the needed elements are here.
            if ( _currentPreset.IsNull<WeatherPreset>()
                 || this.IsNull<MonoBehaviour>()
                 || _rainGO.IsNull<GameObject>()
                 || MainLightController.IsNull<LightController>()
                 || _thunderLightController.IsNull<LightController>() )
            {
                return;
            }

            // Then we are actually applying the preset, giving all the element that are needed.
            _currentPreset.Apply( 
                this, 
                _rainGO, 
                MainLightController, 
                _thunderLightController );

            EventManager.WeatherSystem_OnApplyingWeatherPreset.Call( _currentPreset );

            ToggleSunDisplay_OnApplyingPreset( _currentPreset.IsSunHidden );
            this.Debugger( $"Assign active preset {_currentPreset.name}" );
        }

        /// <summary>
        /// Stops the active weather preset.
        /// It is used when just stopping it or when a new preset become active and another one was already used.
        /// </summary>
        /// <param name="isReassignationNeeded"> 
        /// Means that we are in the case where a new preset has become active. 
        /// </param>
        /// <param name="index"> Corresponds to the active preset -> weatherPresets[index] </param>
        public void StopCurrentWeatherPreset( bool isReassignationNeeded, int index = 0 )
        {
            // First we need to check is a weather preset is defined and active.
            if ( _currentPreset.IsNull<WeatherPreset>()
                || !_currentPreset.IsActive )
            {
                this.Debugger(
                    "The preset you're trying to stop is null",
                    DebugType.Error );
                return;
            }

            // Then we need to actually stop it
            _currentPreset.Stop();
            this.Debugger( $"{_currentPreset.name} has been stopped." );

            // Then as a security layer we reset the active preset to null.
            // But in case we need to reassign the active preset, we assign it to _weatherPresets [ index ].
            _currentPreset = isReassignationNeeded
                ? _currentPreset = _weatherPresets [ index ]
                : null;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Toggles the sun display based on the light setting used by the Light Module...
        /// in the weather preset.
        /// </summary>
        /// <param name="sunIsHidden"></param>
        private void ToggleSunDisplay_OnApplyingPreset( bool sunIsHidden )
        {
            AdditionalLightController.ToggleEnabledLightState( !sunIsHidden );
        }

        /// <summary>
        /// Gras all the reference comming from Environment Lights Referencer to use...
        /// all the light controllers that are neeeded.
        /// </summary>
        public void SetLightReferences()
        {
            EnvironmentLightsReferencer = GetComponent<EnvironmentLightsReferencer>();

            MainLightController = EnvironmentLightsReferencer.MainLightController;
            AdditionalLightController = EnvironmentLightsReferencer.AdditionalLightController;
            _thunderLightController = EnvironmentLightsReferencer.ThunderLightController;
        }

        #endregion

        #region On Editor

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetLightReferences();
        }
#endif

        #endregion
    }
}