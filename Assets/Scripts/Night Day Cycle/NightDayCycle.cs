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
        [Header( "Ambient light settings" )]
        [SerializeField] private AmbientLightingPreset _lightingPreset;        

        [Header( "Night and day settings" )]
        [SerializeField] private bool _stopNightAndDayCycle = false;
        [SerializeField, Range( 1, 240 )] private float _dayDuration = 120f;
        [SerializeField, Range( 0, 240 )] private float _timeOfDay;
        private float _currentTimeOfDay = 0;
        private float _mainLightIntensityAtDay = 0;

        private float _firstPartOfDayLength;
        private float _secondPartOfDayLength;

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

        #region DEBUG

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
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
            if ( _stopNightAndDayCycle || _lightingPreset.IsNull() ) { return; }

            _timeOfDay += Time.deltaTime;
            _timeOfDay %= _dayDuration;

            _currentTimeOfDay = _timeOfDay / _dayDuration;

            float timePercent = _currentTimeOfDay;
            _isDaytime = timePercent >= DAY_START_THRESHOLD && timePercent <= DAY_END_THRESHOLD;

            SetAmbientElements( timePercent );

            RotateMainLight( timePercent );

            if ( _isDaytime ) {
                TryToSetMainLightIntensity_WhenStartingTheDay();
                TryToSetAdditionalLightIntensity_BasedOnDayTime();
            } 
            else {
                TryToSetMainLightIntensity_WhenStartingTheNight();
            }
        }            

        #region COLOR SETTER FOR AMBIENT, DIRECTIONNAL LIGHT AND FOG COLORS

        private void SetAmbientElements( float timePercent )
        {
            RenderSettings.ambientLight = _lightingPreset.AmbientColor.Evaluate( timePercent );

            if ( !MainLightController.IsNull() ) {
                MainLightController.SetLightColor( _lightingPreset.DirectionalColor.Evaluate( timePercent ) );            
            }

            if ( RenderSettings.fog ) {
                RenderSettings.fogColor = _lightingPreset.FogColor.Evaluate( timePercent );
            }
        }

        #endregion

        #region MAIN LIGHT HANDLE

        private void RotateMainLight( float timePercent )
        {
            if ( MainLightController.IsNull() ) { return; }

            MainLightController.transform.localRotation =
                Quaternion.Euler( new Vector3( ( timePercent * 360f ) - 90f, 170f, 0 ) );
        }

        private void TryToSetMainLightIntensity_WhenStartingTheDay()
        {
            if ( MainLightController.IsNull() ) { return; }

            bool isMainLightIntensitySetOrBeingSet = MainLightController.DoesLightIntensityEquals( _mainLightIntensityAtDay ) 
                || _mainLightIntensityTween.IsActive();
            if ( isMainLightIntensitySetOrBeingSet ) { return; }

            TweenMainLightIntensity( 
                _mainLightIntensityTween,
                MainLightController,
                _mainLightIntensityAtDay, 
                1.25f, 
                () => MainLightController.SetLightIntensity( _mainLightIntensityAtDay ) );
        }

        private void TryToSetMainLightIntensity_WhenStartingTheNight()
        {
            if ( MainLightController.IsNull() ) { return; }

            bool isMainLightIntensitySetOrBeingSet = MainLightController.DoesLightIntensityEquals( NIGHT_MAIN_LIGHT_INTENSITY ) 
                || _mainLightIntensityTween.IsActive();
            if ( isMainLightIntensitySetOrBeingSet ) { return; }

            TweenMainLightIntensity(
                _mainLightIntensityTween,
                MainLightController,
                NIGHT_MAIN_LIGHT_INTENSITY,
                1.25f,
                () => MainLightController.SetLightIntensity( NIGHT_MAIN_LIGHT_INTENSITY ) );
        }

        private void TweenMainLightIntensity( Tween tween, LightController mainLightController, float valueToReach, float duration, Action onComplete )
        {
            tween = DOTween.To( () => mainLightController.GetControllerLight().intensity, _ => mainLightController.GetControllerLight().intensity = _, valueToReach, duration );

            tween.OnComplete( () =>
            {
                onComplete?.Invoke();                
                tween.Kill();
            } );
        }

        #endregion

        #region ADDITIONAL LIGHT HANDLE

        private void TryToSetAdditionalLightIntensity_BasedOnDayTime()
        {
            if ( AdditionalLightController.IsNull() ) { return; }

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
            EnvironmentLightModule.EnvironmentLightSettings settings = 
                module.GetSettingsByID( ( int ) _activeWeatherPreset.GetEnvironmentLightInfo().type );
            _mainLightIntensityAtDay = settings.Intensity;

            this.Debugger( "_mainLightIntensityAtDay " + _mainLightIntensityAtDay );
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