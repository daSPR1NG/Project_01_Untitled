using UnityEngine;
using dnSR_Coding.Utilities;
using DG.Tweening;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [RequireComponent( typeof( EnvironmentLightsContainer ) )]

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

        public EnvironmentLightsContainer EnvironmentLightsContainer { get; set; }
        public Light MainLight { get; set; }
        public Light AdditionalLight { get; set; }

        private Tween _mainLightIntensityTween;

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

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            EventManager.OnApplyingWeatherPreset += ReferenceActivePreset;
        }

        void OnDisable() 
        {
            EventManager.OnApplyingWeatherPreset -= ReferenceActivePreset;
        }

        #endregion

        #region Setup

        private void Awake() => Init();
        private void Init()
        {
            SetLightsReference();
        }
        public void SetLightsReference()
        {
            EnvironmentLightsContainer = GetComponent<EnvironmentLightsContainer>();

            MainLight = EnvironmentLightsContainer.MainLight;
            AdditionalLight = EnvironmentLightsContainer.AdditionalLight;
        }

        #endregion

        private void Update() => ProcessNightAndDayCycle();

        private void ProcessNightAndDayCycle()
        {
            if ( _lightingPreset.IsNull() )
            {
                _lightingPreset.LogNullException();
                return; 
            }

            if ( _stopNightAndDayCycle ) { return; }

            //Debug.Log( IsDaytime );

            _timeOfDay += Time.deltaTime;
            _timeOfDay %= _dayDuration;

            _currentTimeOfDay = _timeOfDay / _dayDuration;

            float timePercent = _currentTimeOfDay;
            _isDaytime = timePercent >= DAY_START_THRESHOLD && timePercent <= DAY_END_THRESHOLD;

            SetAmbientColor( timePercent );
            SetMainLightColor( timePercent );
            SetFogColor( timePercent );

            RotateMainLight( timePercent );

            SetMainLightIntensityAtDay();
            SetMainLightIntensityAtNight();
            SetAdditionalLightIntensity_BasedOnDayTime( _isDaytime );            
        }            

        #region Color setter for ambient, directional light and fog colors

        private void SetAmbientColor( float timePercent )
        {
            RenderSettings.ambientLight = _lightingPreset.AmbientColor.Evaluate( timePercent );
        }

        private void SetMainLightColor( float timePercent )
        {
            if ( MainLight.IsNull() )
            {
                MainLight.LogNullException();
                return;
            }

            MainLight.color = _lightingPreset.DirectionalColor.Evaluate( timePercent );
        }

        private void SetFogColor( float timePercent )
        {
            if ( !RenderSettings.fog ) { return; }

            RenderSettings.fogColor = _lightingPreset.FogColor.Evaluate( timePercent );
        }

        #endregion

        private void RotateMainLight( float timePercent )
        {
            if ( MainLight.IsNull() )
            {
                MainLight.LogNullException();
                return;
            }

            MainLight.transform.localRotation =
                Quaternion.Euler( new Vector3( ( timePercent * 360f ) - 90f, 170f, 0 ) );
        }

        private void SetMainLightIntensityAtDay()
        {
            if ( !IsDaytime ) { return; }

            if ( MainLight.intensity == _mainLightIntensityAtDay || _mainLightIntensityTween.IsActive() ) { return; }

            _mainLightIntensityTween = DOTween.To( () => MainLight.intensity, _ => MainLight.intensity = _, _mainLightIntensityAtDay, 1.25f );
            _mainLightIntensityTween.OnComplete( () =>
            {
                MainLight.intensity = _mainLightIntensityAtDay;
                _mainLightIntensityTween.Kill();
            } );
        }
        private void SetMainLightIntensityAtNight()
        {
            if ( IsDaytime ) { return; }

            if ( MainLight.intensity == NIGHT_MAIN_LIGHT_INTENSITY || _mainLightIntensityTween.IsActive() ) { return; }

            _mainLightIntensityTween = DOTween.To( () => MainLight.intensity, _ => MainLight.intensity = _, NIGHT_MAIN_LIGHT_INTENSITY, 1.25f );
            _mainLightIntensityTween.OnComplete( () =>
            {
                MainLight.intensity = NIGHT_MAIN_LIGHT_INTENSITY;
                _mainLightIntensityTween.Kill();
            } );
        }

        private void SetAdditionalLightIntensity_BasedOnDayTime( bool isDayTime )
        {
            if ( AdditionalLight.IsNull() )
            {
                AdditionalLight.LogNullException();
                return;
            }

            if ( !isDayTime || _activeWeatherPreset.IsSunHidden )
            {
                HideSunLight();
                return;
            }

            DisplaySunLight();
            
            _firstPartOfDayLength = ( _dayDuration * GetMedianOfDay() ) - ( _dayDuration * DAY_START_THRESHOLD );            
            _secondPartOfDayLength = ( _dayDuration * DAY_END_THRESHOLD ) - ( _dayDuration * GetMedianOfDay() );
            
            // When we are still in the first part of the day...
            if ( _currentTimeOfDay < GetMedianOfDay() )
            {
                AdditionalLight.intensity = GetStartingDayValue() / _firstPartOfDayLength;
                return;
            }

            // When we are in the second part of the day...
            AdditionalLight.intensity = GetEndingDayValue() / _secondPartOfDayLength;
        }

        private void DisplaySunLight()
        {
            AdditionalLight.gameObject.TryToDisplay();
        }
        private void HideSunLight()
        {
            AdditionalLight.intensity = 0;
            AdditionalLight.gameObject.TryToHide();
        }

        private void ReferenceActivePreset( WeatherPreset sentWeatherPreset )
        {
            _activeWeatherPreset = sentWeatherPreset;

            EnvironmentLightModule module = _activeWeatherPreset.GetEnvironmentLightInfo().module;
            EnvironmentLightModule.EnvironmentLightSettings settings = module.GetSettingsByID( ( int ) _activeWeatherPreset.GetEnvironmentLightInfo().type );
            _mainLightIntensityAtDay = settings.Intensity;

            this.Log( "_mainLightIntensityAtDay " + _mainLightIntensityAtDay );
        }

        private float GetStartingDayValue()
        {
            return ( _timeOfDay - ( _dayDuration * DAY_START_THRESHOLD ) );
        }
        private float GetEndingDayValue()
        {
            return -( _timeOfDay - ( _dayDuration * DAY_END_THRESHOLD ) );
        }

        private float GetMedianOfDay()
        {
            return ( DAY_START_THRESHOLD + DAY_END_THRESHOLD ) / 2;
        }     

        #region On Editor

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetLightsReference();
        }

        float _dayTime = 0;
        void OnGUI()
        {
            GUI.Label( new Rect( 5, 5, 150, 30 ), "Time of Day - " + ( _currentTimeOfDay * 100 ).ToString("0.0") );
            _dayTime = GUI.HorizontalSlider( new Rect( 5, 25, 100, 30 ), _dayTime % _dayDuration, 0.0f, _dayDuration );
            _timeOfDay = _dayTime;
        }
#endif

        #endregion
    }
}