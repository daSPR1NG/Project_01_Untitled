using UnityEngine;
using DG.Tweening;
using System;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Interfaces;
using dnSR_Coding.Utilities.Attributes;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor.Expressions;
using System.Runtime.CompilerServices;

namespace dnSR_Coding
{
    [RequireComponent( typeof( EnvironmentLightsReferencer ) )]

    // All this script has to handle is :
    // Setting and handling the time of day
    // AND Consume services for handling the ambience look (mainlight, ambient color, ...)

    ///<summary> NightDayCycle description <summary>
    [DisallowMultipleComponent]
    public class NightDayCycle : MonoBehaviour, IEnvironmentLightsUser, IDebuggable
    {
        private const float DEFAULT_DAY_DURATION = 360;
        private const float DAY_START_THRESHOLD = .25f;
        private const float DAY_END_THRESHOLD = .75f;
        private const float NIGHT_MAIN_LIGHT_INTENSITY = 0.2f;

        [ToggleLeft]
        [PropertySpace( 5, 5 )]
        [InfoBox( "Stops the cycle, meaning that the time of day is not updated anymore." )]
        [SerializeField] private bool _stopNightAndDayCycle = false;

        [SerializeField, PropertySpace( 5, 5 )]
        private AmbientLightingPreset _lightingPreset;

        [CenteredHeader( "Night and day settings" )]

        [SerializeField, PropertyRange( 1, "DEFAULT_DAY_DURATION" )]
        private float _dayDuration = DEFAULT_DAY_DURATION;
        [SerializeField, PropertyRange( 1, "DEFAULT_DAY_DURATION" )]
        private float _timeOfDay = DEFAULT_DAY_DURATION / 2;
        private float _currentTimeOfDay = 0f;

        [CenteredHeader( "Main Light Intensity Settings" ), PropertySpace( 5, 10 )]
        [SerializeField] private MainLightIntensitySettings _mainLightIntensitySettings;

        public EnvironmentLightsReferencer EnvironmentLightsReferencer { get; set; }
        public LightController MainLightController { get; set; }
        public LightController AdditionalLightController { get; set; }

        private readonly LightIntensityTweener _lightIntentityTweener = null;
        private readonly Tween _mainLightIntensityTween;

        private static bool _isDaytime = false;
        public static bool IsDaytime
        {
            get => _isDaytime;
            set
            {
                // Nighttime block
                if ( _isDaytime && !value ) { Debug.Log( "It is Night time" ); }
                // Daytime block
                else if ( !_isDaytime && value ) { Debug.Log( "It is Daytime" ); }

                _isDaytime = value;
            }
        }

        private WeatherPreset _activeWeatherPreset = null;
        private Color _initialFogColor = default;

        private readonly AmbientColorer _ambientColorer = null;
        private readonly LightRotator _lightRotator = null;

        [System.Serializable]
        private struct MainLightIntensitySettings
        {
            [SerializeField] private float _shiftDurationOnDay;
            [SerializeField] private float _shiftDurationOnNight;

            private float _mainLightIntensityAtDay;

            public float ShiftDurationOnDay { readonly get => _shiftDurationOnDay; set => _shiftDurationOnDay = value; }
            public float ShiftDurationOnNight { readonly get => _shiftDurationOnNight; set => _shiftDurationOnNight = value; }
            public float MainLightIntensityAtDay { readonly get => _mainLightIntensityAtDay; set => _mainLightIntensityAtDay = value; }
        }

        #region DEBUG

        [field: SerializeField, FoldoutGroup( "Debug Section", Order = -1 )]
        public bool IsDebuggable { get; set; } = true;

        #endregion

        #region ENABLE, DISABLE

        void OnEnable()
        {
            EventManager.WeatherSystem_OnApplyingWeatherPreset.Subscribe( SetActivePreset_WhenOneIsApplied );
        }

        void OnDisable()
        {
            EventManager.WeatherSystem_OnApplyingWeatherPreset.Unsubscribe( SetActivePreset_WhenOneIsApplied );
        }

        #endregion

        #region CONSTRUCTOR

        private NightDayCycle()
        {
            _ambientColorer = new AmbientColorer();
            _lightRotator = new LightRotator();
            _lightIntentityTweener = new LightIntensityTweener();
        }

        #endregion

        #region SETUP

        private void Awake() => Init();
        private void Init()
        {
            SetLightReferences();
        }

        [Button( ButtonStyle.CompactBox )]
        public void SetLightReferences()
        {
            EnvironmentLightsReferencer = GetComponent<EnvironmentLightsReferencer>();

            MainLightController = EnvironmentLightsReferencer.MainLightController;
            AdditionalLightController = EnvironmentLightsReferencer.AdditionalLightController;

            this.Debugger( "NightDayCycle - Light references have been set." );
        }

        #endregion

        private void Update() => ProcessNightAndDayCycle();
        private void ProcessNightAndDayCycle()
        {
            if ( _stopNightAndDayCycle || _lightingPreset.IsNull<AmbientLightingPreset>() ) { return; }

            _timeOfDay += Time.deltaTime;
            _timeOfDay %= _dayDuration;

            _currentTimeOfDay = _timeOfDay / _dayDuration;
            _isDaytime = _currentTimeOfDay >= DAY_START_THRESHOLD && _currentTimeOfDay <= DAY_END_THRESHOLD;

            _ambientColorer.SetAmbientElementsColor(
                MainLightController,
                _lightingPreset,
                _initialFogColor,
                _currentTimeOfDay );

            _lightRotator.RotateLight_BasedOnCurrentTimeOfDay( MainLightController, _currentTimeOfDay );

            if ( _isDaytime )
            {
                _lightIntentityTweener.TweenLightIntensity(
                    _mainLightIntensityTween,
                    MainLightController,
                    _mainLightIntensitySettings.MainLightIntensityAtDay,
                    _mainLightIntensitySettings.ShiftDurationOnDay );
                SetAdditionalLightIntensity_BasedOnDayTime( AdditionalLightController, _activeWeatherPreset );
            }
            else
            {
                _lightIntentityTweener.TweenLightIntensity(
                    _mainLightIntensityTween,
                    MainLightController,
                    NIGHT_MAIN_LIGHT_INTENSITY,
                    _mainLightIntensitySettings.ShiftDurationOnNight );
                AdditionalLightController.SetLightIntensity( 0 );
            }
        }

        #region ADDITIONAL LIGHT HANDLE

        private void SetAdditionalLightIntensity_BasedOnDayTime(
            LightController lightController,
            WeatherPreset weatherPreset )
        {
            bool isSunHidden = weatherPreset.IsNull<WeatherPreset>()
                || !weatherPreset.IsNull() && weatherPreset.IsSunHidden;

            if ( isSunHidden )
            {
                this.Debugger( "Sun is hidden" );
                lightController.DisableLight();
                return;
            }

            lightController.EnableLight();

            bool isFirstPartOfDay = _currentTimeOfDay < GetTotalDayPercentage();

            float currentPartOfDayValue = isFirstPartOfDay
                ? ( _dayDuration * GetTotalDayPercentage() ) - ( _dayDuration * DAY_START_THRESHOLD )
                : ExtMathfs.Abs( ( _dayDuration * GetTotalDayPercentage() ) - ( _dayDuration * DAY_END_THRESHOLD ) );
            float maxPartOfDayValue = isFirstPartOfDay ? GetStartingDayValue() : GetEndingDayValue();

            this.Debugger( GetTotalDayPercentage() );
            this.Debugger( _dayDuration * GetTotalDayPercentage() );
            this.Debugger( maxPartOfDayValue + "/" + currentPartOfDayValue );
            this.Debugger( maxPartOfDayValue / currentPartOfDayValue );

            lightController.SetLightIntensity( maxPartOfDayValue / currentPartOfDayValue );
        }

        #endregion

        #region PRESET HANDLE

        private void SetActivePreset_WhenOneIsApplied( WeatherPreset sentWeatherPreset )
        {
            _activeWeatherPreset = sentWeatherPreset;

            EnvironmentLightModule module = _activeWeatherPreset.GetEnvironmentLightInfo().module;
            EnvironmentLightModule.EnvironmentLightSettings lightSettings =
                module.GetSettingsByID( ( int ) _activeWeatherPreset.GetEnvironmentLightInfo().type );
            _mainLightIntensitySettings.MainLightIntensityAtDay = lightSettings.Intensity;

            FogModule fogModule = _activeWeatherPreset.GetFogInfo().module;
            FogModule.FogSettings fogSettings = fogModule.GetSettingsByID( ( int ) _activeWeatherPreset.GetFogInfo().type );
            _initialFogColor = fogSettings.FogColor;
        }

        #endregion

        #region DAY VALUES | START - MEDIAN - END

        private float GetStartingDayValue()
        {
            return _timeOfDay - ( _dayDuration * DAY_START_THRESHOLD );
        }
        private float GetEndingDayValue()
        {
            return ExtMathfs.Abs( _timeOfDay - ( _dayDuration * DAY_END_THRESHOLD ) );
        }
        private float GetTotalDayPercentage()
        {
            float totalDayDuration = DAY_END_THRESHOLD - DAY_START_THRESHOLD;
            return totalDayDuration;
        }

        #endregion
    }
}