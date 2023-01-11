using UnityEngine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System;

namespace dnSR_Coding
{
    // Required components or public findable enum here.
    [RequireComponent( typeof( WeatherSystemManager ) )]
    [RequireComponent( typeof( TimeController ) )]

    ///<summary> NightDayCycleManager description <summary>
    [Component("EnvironmentLightingManager", "")]
    [DisallowMultipleComponent, ExecuteAlways]
    public class EnvironmentLightingManager : MonoBehaviour, IDebuggable
    {
        [Header( "DEPENDENCIES" )]

        [SerializeField] private Light _directionalLight;        

        [Header( "STORED LIGHTING SETTINGS" )]

        [SerializeField, Expandable] private EnvironmentLightingSettings _defaultLightingSettings;
        [SerializeField, Expandable] private EnvironmentLightingSettings _nightTimeLightingSettings;
        private EnvironmentLightingSettings _activeSettings;
        
        private float _currentLightIntensity = 0;        

        private WeatherSystemManager _weatherSystemManager;
        private TimeController _timeController;
        private CustomPostProcessVolume _mainCameraVolume;

        public static Action<EnvironmentLightingSettings> OnLightingSettingsChanged;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            WeatherSystemManager.OnWeatherChanged += SetEnvironmentLightingSettingsOnWeatherChanged;

            TimeController.OnFallingNight += SetActiveSettings;
            TimeController.OnRisingSun += SetActiveSettings;
        }

        void OnDisable() 
        {
            WeatherSystemManager.OnWeatherChanged -= SetEnvironmentLightingSettingsOnWeatherChanged;

            TimeController.OnFallingNight -= SetActiveSettings;
            TimeController.OnRisingSun -= SetActiveSettings;
        }

        #endregion

        void Awake() => Init();
        void Init()
        {
            GetLinkedComponents();
            SetLightingSettingsOnStart();
        }
        void GetLinkedComponents()
        {
            if ( _weatherSystemManager.IsNull() ) { _weatherSystemManager = GetComponent<WeatherSystemManager>(); }
            if ( _timeController.IsNull() ) { _timeController = GetComponent<TimeController>(); }

            if ( _mainCameraVolume.IsNull() ) 
            {
                _mainCameraVolume = Helper.GetMainCamera().GetComponent<CustomPostProcessVolume>(); 
            }
        }

        private void Update()
        {
            if ( Application.isPlaying ) { UpdateLighting( _timeController.GetCurrentTimeOfDay(), _activeSettings ); }
            else
            {
                UpdateLighting( _timeController.GetCurrentTimeOfDay(), _activeSettings );
            }
        }

        #region Lighting handle on Update

        /// <summary>
        /// Updates the main directional light, acting as a Sun, at runtime.
        /// Ambient, fog and intensity are modified.
        /// </summary>
        private void UpdateLighting( float timeOfDay, EnvironmentLightingSettings settings )
        {
            if ( _activeSettings.IsNull() ) { return; }

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

        #region Utils UpdateLighting method

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
        /// Set the correct lighting settings at start, wether it is daytime or not.
        /// </summary>
        private void SetLightingSettingsOnStart()
        {
            SetActiveSettings( _timeController.IsDaytime );
        }

        /// <summary>
        /// Sets the active volume profile settings, if its not already set.
        /// </summary>
        public void SetActiveSettings( bool isDaytime )
        {
            EnvironmentLightingSettings settings =
                isDaytime ? _weatherSystemManager.ActiveWeather.GetLightingSettings() : _nightTimeLightingSettings;

            if ( _activeSettings == settings ) { return; }
            Debug.Log( "Set active settings when its daytime : " + isDaytime );

            _activeSettings = settings;
            SetCameraVolumeManagerGammaValue( settings );
            ChangeSkybox();
        }

        private void ChangeSkybox()
        {
            if ( _activeSettings.RelatedSkybox.IsNull() ) { return; }
            RenderSettings.skybox = _activeSettings.RelatedSkybox;
        }

        /// <summary>
        /// Sets default lighting settings, it is used when no weather sequence are currently active,
        /// or when the active sequence doesn't have lighting settings filled.
        /// </summary>
        private void SetDefaultLightingSettings()
        {
            // If the sequence is null then we set the default setup.
            _activeSettings = _defaultLightingSettings;
            SetCameraVolumeManagerGammaValue( _defaultLightingSettings );

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
                SetActiveSettings( _timeController.IsDaytime );
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