using UnityEngine;
using dnSR_Coding.Utilities;
using DG.Tweening;
using NaughtyAttributes;
using System;

namespace dnSR_Coding
{
    [RequireComponent( typeof( EnvironmentLightsReferencer ) )]

    ///<summary> NightDayCycle description <summary>
    [DisallowMultipleComponent]
    public class NightDayCycle : MonoBehaviour , IEnvironmentLightsUser, IDebuggable
    {
        [SerializeField] public AmbientLightingPreset _lightingPreset;

        [Header( "Night and day settings" )]
        [SerializeField] private bool _stopNightAndDayCycle = false;
        [SerializeField, Range( 1, 240 )] private float _dayDuration = 240f;
        [SerializeField, Range( 1, 240 )] private float _timeOfDay = 120f;
        private float _currentTimeOfDay = 0f;
        private float _firstPartOfDayLength, _secondPartOfDayLength;

        [Header( "Main light settings" )]
        [SerializeField] private float _mainLightIntensityShiftDurationOnDay = 1.25f;
        [SerializeField] private float _mainLightIntensityShiftDurationOnNight = 1.25f;
        private float _mainLightIntensityAtDay = 0f;

        public EnvironmentLightsReferencer EnvironmentLightsReferencer { get; set; }
        public LightController MainLightController { get; set; }
        public LightController AdditionalLightController { get; set; }

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

        private const float DAY_START_THRESHOLD = .3f;
        private const float DAY_END_THRESHOLD = .7f;
        private const float NIGHT_MAIN_LIGHT_INTENSITY = 0.2f;

        private WeatherPreset _activeWeatherPreset = null;
        private Color _initialFogColor;

        #region DEBUG

        //[Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region ENABLE, DISABLE

        void OnEnable() 
        {
            EventManager.OnApplyingWeatherPreset += SetActivePreset;
        }

        void OnDisable() 
        {
            EventManager.OnApplyingWeatherPreset -= SetActivePreset;
        }

        #endregion

        #region SETUP

        private void Awake() => Init();
        private void Init()
        {
            SetLightsReference();
        }
        public void SetLightsReference()
        {
            EnvironmentLightsReferencer = GetComponent<EnvironmentLightsReferencer>();

            MainLightController = EnvironmentLightsReferencer.MainLightController;
            AdditionalLightController = EnvironmentLightsReferencer.AdditionalLightController;
        }

        #endregion

        private void Update() => ProcessNightAndDayCycle();
        private void ProcessNightAndDayCycle()
        {
            if ( _stopNightAndDayCycle || _lightingPreset.IsNull<AmbientLightingPreset>() ) { return; }

            _timeOfDay += Time.deltaTime;
            _timeOfDay %= _dayDuration;

            _currentTimeOfDay = _timeOfDay / _dayDuration;

            float timePercent = _currentTimeOfDay;
            _isDaytime = timePercent >= DAY_START_THRESHOLD && timePercent <= DAY_END_THRESHOLD;

            SetAmbientElements( timePercent );

            RotateMainLight( timePercent );

            if ( _isDaytime ) {
                SetMainLightIntensity_WhenStartingTheDay();
                SetAdditionalLightIntensity_BasedOnDayTime();
            } 
            else {
                SetMainLightIntensity_WhenStartingTheNight();
            }
        }            

        #region COLOR SETTER FOR AMBIENT, DIRECTIONNAL LIGHT AND FOG COLORS

        private void SetAmbientElements( float timePercent )
        {
            // Ambient light color
            RenderSettings.ambientLight = _lightingPreset.AmbientColor.Evaluate( timePercent );

            // Main light color
            if ( !MainLightController.IsNull<LightController>() ) {
                MainLightController.SetLightColor( _lightingPreset.DirectionalColor.Evaluate( timePercent ) );
            }

            // Fog color
            if ( RenderSettings.fog ) {
                RenderSettings.fogColor = _initialFogColor.MultiplyRGB( _lightingPreset.FogColor.Evaluate( timePercent ) );
            }
        }

        #endregion

        #region MAIN LIGHT HANDLE

        private void RotateMainLight( float timePercent )
        {
            if ( MainLightController.IsNull<LightController>() ) { return; }

            MainLightController.transform.localRotation =
                Quaternion.Euler( new Vector3( ( timePercent * 360f ) - 90f, 170f, 0 ) );
        }

        private void SetMainLightIntensity_WhenStartingTheDay()
        {
            if ( MainLightController.IsNull<LightController>() ) { return; }

            bool isMainLightIntensitySetOrBeingSet = MainLightController.DoesLightIntensityEquals( _mainLightIntensityAtDay ) 
                || _mainLightIntensityTween.IsActive();
            if ( isMainLightIntensitySetOrBeingSet ) { return; }

            TweenMainLightIntensity( 
                _mainLightIntensityTween,
                MainLightController,
                _mainLightIntensityAtDay,
                _mainLightIntensityShiftDurationOnDay, 
                () => MainLightController.SetLightIntensity( _mainLightIntensityAtDay ) );
        }

        private void SetMainLightIntensity_WhenStartingTheNight()
        {
            if ( MainLightController.IsNull<LightController>() ) { return; }

            bool isMainLightIntensitySetOrBeingSet = MainLightController.DoesLightIntensityEquals( NIGHT_MAIN_LIGHT_INTENSITY ) 
                || _mainLightIntensityTween.IsActive();
            if ( isMainLightIntensitySetOrBeingSet ) { return; }

            TweenMainLightIntensity(
                _mainLightIntensityTween,
                MainLightController,
                NIGHT_MAIN_LIGHT_INTENSITY,
                _mainLightIntensityShiftDurationOnNight,
                () => MainLightController.SetLightIntensity( NIGHT_MAIN_LIGHT_INTENSITY ) );
        }

        private void TweenMainLightIntensity( Tween tween, LightController mainLightController, float valueToReach, float duration, Action onComplete )
        {
            this.Debugger( "Tweening main light intensity !" );

            tween = DOTween.To( () => mainLightController.GetControllerLight().intensity, _ => mainLightController.GetControllerLight().intensity = _, valueToReach, duration );

            tween.OnComplete( () =>
            {
                onComplete?.Invoke();                
                tween.Kill();
            } );
        }

        #endregion

        #region ADDITIONAL LIGHT HANDLE

        private void SetAdditionalLightIntensity_BasedOnDayTime()
        {
            if ( AdditionalLightController.IsNull<LightController>() ) { return; }

            if ( _activeWeatherPreset.IsSunHidden )
            {
                AdditionalLightController.DisableLight();
                return;
            }

            AdditionalLightController.EnableLight();

            // When we are still in the first part of the day...
            if ( _currentTimeOfDay < GetMedianOfDay() )
            {
                _firstPartOfDayLength = ( _dayDuration * GetMedianOfDay() ) - ( _dayDuration * DAY_START_THRESHOLD );
                AdditionalLightController.SetLightIntensity( GetStartingDayValue() / _firstPartOfDayLength );
                return;
            }

            _secondPartOfDayLength = ( _dayDuration * DAY_END_THRESHOLD ) - ( _dayDuration * GetMedianOfDay() );
            // When we are in the second part of the day...
            AdditionalLightController.SetLightIntensity( GetEndingDayValue() / _secondPartOfDayLength );
        }
        
        #endregion

        private void SetActivePreset( WeatherPreset sentWeatherPreset )
        {
            _activeWeatherPreset = sentWeatherPreset;

            EnvironmentLightModule module = _activeWeatherPreset.GetEnvironmentLightInfo().module;
            EnvironmentLightModule.EnvironmentLightSettings lightSettings = 
                module.GetSettingsByID( ( int ) _activeWeatherPreset.GetEnvironmentLightInfo().type );
            _mainLightIntensityAtDay = lightSettings.Intensity;

            this.Debugger( "_mainLightIntensityAtDay " + _mainLightIntensityAtDay );

            FogModule fogModule = _activeWeatherPreset.GetFogInfo().module;
            FogModule.FogSettings fogSettings = fogModule.GetSettingsByID( ( int ) _activeWeatherPreset.GetFogInfo().type );
            _initialFogColor = fogSettings.FogColor;
        }

        #region DAY VALUES | START - MEDIAN - END

        private float GetStartingDayValue() {
            return ( _timeOfDay - ( _dayDuration * DAY_START_THRESHOLD ) );
        }
        private float GetEndingDayValue() {
            return -( _timeOfDay - ( _dayDuration * DAY_END_THRESHOLD ) );
        }
        private float GetMedianOfDay()
        {
            float totalDayDuration = DAY_START_THRESHOLD + DAY_END_THRESHOLD;
            return totalDayDuration / 2;
        }     

        #endregion

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