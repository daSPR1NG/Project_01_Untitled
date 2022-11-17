using UnityEngine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;
using System;

namespace dnSR_Coding
{
    // Required components or public findable enum here.
    [RequireComponent( typeof( WeatherSystemManager ) )]

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/EnvironmentLightingManager" )]

    ///<summary> NightDayCycleManager description <summary>
    [Component("EnvironmentLightingManager", "")]
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public class EnvironmentLightingManager : MonoBehaviour, IDebuggable
    {
        [Title( "DEPENDENCIES", 12, "white" )]

        [SerializeField] private Light _directionalLight;        
        [SerializeField, Range( 1, 240 )] private float _dayDuration = 120f;
        [SerializeField, Range( 0, 240 )] private float _timeOfDay;

        [Title( "STORED LIGHTING SETTINGS", 12, "white" )]

        [SerializeField, ExposedScriptableObject] private EnvironmentLightingSettings _defaultLightingSettings;
        [SerializeField, ExposedScriptableObject] private EnvironmentLightingSettings _nightTimeLightingSettings;

        [field: SerializeField] public EnvironmentLightingSettings ActiveSettings { get; private set; }

        private WeatherSystemManager _weatherSystemManager;
        private CustomPostProcessVolume _mainCameraVolume;

        float _currentTimeOfDay;
        private float _currentLightIntensity = 0;

        private bool _isDaytime = false;
        public bool IsDaytime 
        { 
            get => _isDaytime; 
            set
            {
                // Nighttime block
                if ( _isDaytime && !value )
                {
                    Debug.Log( "Is Daytime value changed to false" );

                    // Raising the event to subsribers.
                    OnFallingNight?.Invoke();
                    // Change active settings to the normal one matching the current weather.
                    SetActiveSettings( _nightTimeLightingSettings );

                    _weatherSystemManager.ActiveWeather.HideSunShafts();
                }
                // Daytime block
                else if ( !_isDaytime && value )
                {
                    Debug.Log( "Is Daytime value changed to true" );                    

                    // Raising the event to subsribers.
                    OnRisingSun?.Invoke();
                    // Change active settings to the normal one matching the current weather.
                    SetActiveSettings( _weatherSystemManager.ActiveWeather.GetLightingSettings() );

                    _weatherSystemManager.ActiveWeather.DisplaySunShafts( true );
                }

                _isDaytime = value;
            }
        }


        public static Action<EnvironmentLightingSettings> OnLightingSettingsChanged;
        public static Action OnFallingNight;
        public static Action OnRisingSun;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            WeatherSystemManager.OnWeatherChanged += SetEnvironmentLightingSettingsOnWeatherChanged;
        }

        void OnDisable() 
        {
            WeatherSystemManager.OnWeatherChanged -= SetEnvironmentLightingSettingsOnWeatherChanged;
        }

        #endregion

        void Awake() => Init();
        void Init()
        {
            GetLinkedComponents();
        }
        void GetLinkedComponents()
        {
            if ( _weatherSystemManager.IsNull() ) { _weatherSystemManager = GetComponent<WeatherSystemManager>(); }

            if ( _mainCameraVolume.IsNull() ) 
            {
                _mainCameraVolume = Helper.GetMainCamera().GetComponent<CustomPostProcessVolume>(); 
            }
        }

        private void Update()
        {
            if ( ActiveSettings.IsNull() ) { return; }

            if ( Application.isPlaying ) { UpdateTimeOfDay(); }
            else
            {
                _timeOfDay %= _dayDuration;
                _currentTimeOfDay = _timeOfDay / _dayDuration;

                HandleCurrentTimeOfday();

                SetEnvironmentLightingSettingsOnWeatherChanged( _weatherSystemManager.ActiveWeather );
                UpdateLighting( _currentTimeOfDay, ActiveSettings );
            }
        }

        #region Lighting handle on Update

        /// <summary>
        /// Updates current time of day, tracking wether its night or daytime.
        /// </summary>
        private void UpdateTimeOfDay()
        {
            _timeOfDay += Time.deltaTime;
            _timeOfDay %= _dayDuration; //Modulus to ensure always between 0-maxValue

            _currentTimeOfDay = _timeOfDay / _dayDuration;

            HandleCurrentTimeOfday();

            // Then we update the current active lighting settings parameters/setup.
            UpdateLighting( _currentTimeOfDay, ActiveSettings );
        }

        /// <summary>
        /// Updates the main directional light, acting as a Sun, at runtime.
        /// Ambient, fog and intensity are modified.
        /// </summary>
        private void UpdateLighting( float timeOfDay, EnvironmentLightingSettings settings )
        {
            if ( ActiveSettings.IsNull() ) { return; }

            //Set ambient and fog values at runtime.
            RenderSettings.ambientLight = settings.AmbientColor.Evaluate( timeOfDay );
            RenderSettings.fogColor = settings.FogColor.Evaluate( timeOfDay );

            if ( _directionalLight.IsNull() ) { return; }

            // This block here allows us to make the intensity oscillating from these values to :
            // 0% daytime == 0% of intensity a.k.a _settings.LowerLightIntensity,
            // TO 50% daytime == 100% of intensity,
            // TO 100% of daytime == 0% of intensity a.k.a _settings.LowerLightIntensity.
            // This system mimics how the sun behaves in IRL.
            _currentLightIntensity = timeOfDay <= .5f
                ? settings.GreaterLightIntensity * timeOfDay
                : Mathf.Abs( ( ( settings.GreaterLightIntensity * timeOfDay ) - settings.GreaterLightIntensity ) );

            SetLightIntensity( _currentLightIntensity * 2,
                                         settings.LowerLightIntensity,
                                         settings.GreaterLightIntensity );

            SetLightColor( settings.DirectionalColor.Evaluate( timeOfDay ) );

            _directionalLight.transform.localRotation =
                Quaternion.Euler( new Vector3( ( timeOfDay * 360f ) - 90f, 170f, 0 ) );
        }

        #region Utils UpdateLighting

        private void SetLightIntensity( float intensity, float clampMin, float clampMax )
        {
            if ( _directionalLight.IsNull() || _directionalLight.intensity == intensity ) { return; }

            // We multiply intensity by 2 to match the exact value of _settings.GreaterLightIntensity
            _directionalLight.intensity = intensity;
            _directionalLight.intensity = _directionalLight.intensity.Clamped( clampMin, clampMax );
        }

        private void SetLightColor( Color color )
        {
            if ( _directionalLight.IsNull() || _directionalLight.color == color ) { return; }

            //If the directional light is set then rotate and set it's color, I actually rarely use the rotation because it casts tall shadows unless you clamp the value
            _directionalLight.color = color;
        }


        #endregion

        #endregion

        /// <summary>
        /// Sets the active volume profile settings, if its not already set.
        /// </summary>
        private void SetActiveSettings( EnvironmentLightingSettings settings )
        {
            if ( ActiveSettings == settings ) { return; }

            ActiveSettings = settings;
            SetCameraVolumeManagerGammaValue( settings );
        }

        /// <summary>
        /// Sets default lighting settings, it is used when no weather sequence are currently active,
        /// or when the active sequence doesn't have lighting settings filled.
        /// </summary>
        private void SetDefaultLightingSettings()
        {
            // If the sequence is null then we set the default setup.
            SetActiveSettings( _defaultLightingSettings );

            SetLightIntensity( 2f, 0f, 2f );
            SetLightColor( Color.red );
        }

        #region On weather changed handle

        /// <summary>
        /// Sets the correct lighting settings according to the new weather.
        /// </summary>
        /// <param name="sequence"></param>
        private void SetEnvironmentLightingSettingsOnWeatherChanged( WeatherSequence sequence )
        {
            if ( !sequence.IsNull()
                || !sequence.IsNull() && sequence.GetLightingSettings().IsNull() )
            {
                var settings = IsDaytime ? sequence.GetLightingSettings() : _nightTimeLightingSettings;
                SetActiveSettings( settings );

                return;
            }

            SetDefaultLightingSettings();
        }

        #region Utils SetEnvironmentLightingSettingsOnWeatherChanged       

        /// <summary>
        /// Sets the gamma volume of the current settings volume profile.
        /// </summary>
        /// <param name="settings"></param>
        private void SetCameraVolumeManagerGammaValue( EnvironmentLightingSettings settings )
        {
            if ( Application.isPlaying ) { OnLightingSettingsChanged?.Invoke( settings ); }
            else
            {
                CameraVolumeManager cvm = ( CameraVolumeManager ) FindObjectOfType( typeof( CameraVolumeManager ) );
                cvm.NeedToBeUpdated( settings );
            }
        }

        #endregion

        #endregion

        #region On specific time of day reached

        /// <summary>
        /// Assigns a value to "IsDaytime".
        /// When its value change to its previous value it executes instructions set in it declaration above.
        /// </summary>
        private void HandleCurrentTimeOfday()
        {
            IsDaytime = _currentTimeOfDay >= .3f && _currentTimeOfDay <= .7f;
            Debug.Log( "HandleCurrentTimeOfday() : " + IsDaytime, transform );
        }

        #endregion

        #region OnValidate

#if UNITY_EDITOR

        private void FindPossibleDirectionalLightInEditor()
        {
            if ( !_directionalLight.IsNull() ) { return; }

            //Search for lighting tab sun
            if ( !RenderSettings.sun.IsNull() ) { _directionalLight = RenderSettings.sun; }
            //Search scene for light that fits criteria (directional)
            else
            {
                Light [] lights = FindObjectsOfType<Light>();

                foreach ( Light light in lights )
                {
                    if ( light.type != LightType.Directional ) { continue; }

                    _directionalLight = light;
                    return;
                }
            }
        }

        //Try to find a directional light to use if we haven't set one
        private void OnValidate()
        {
            GetLinkedComponents();

            FindPossibleDirectionalLightInEditor();
        }
#endif

        #endregion
    }
}