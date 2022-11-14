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
        [SerializeField] private bool _isDaytime = false;
        [SerializeField, Range( 0, 240 )] private float _dayDuration = 120f;
        [SerializeField, Range( 0, 240 )] private float _timeOfDay;
        [SerializeField, ExposedScriptableObject] private EnvironmentLightingSettings _defaultLightingSettings;

        [field: SerializeField] public EnvironmentLightingSettings ActiveSettings { get; private set; }

        private WeatherSystemManager _weatherSystemManager;
        private CustomPostProcessVolume _mainCameraVolume;

        private float _currentLightIntensity = 0;

        public static Action OnLightingSettingsChanged;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            WeatherSystemManager.OnWeatherChanged += SetActiveLightingSettings;
        }

        void OnDisable() 
        {
            WeatherSystemManager.OnWeatherChanged -= SetActiveLightingSettings;
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

            if ( Application.isPlaying )
            {
                //(Replace with a reference to the game time)
                _timeOfDay += Time.deltaTime;
                _timeOfDay %= _dayDuration; //Modulus to ensure always between 0-maxValue
                UpdateLighting( _timeOfDay / _dayDuration, ActiveSettings );
            }
            else
            {
                SetActiveLightingSettings( _weatherSystemManager.ActiveWeather );
                UpdateLighting( _timeOfDay / _dayDuration, ActiveSettings );
            }
        }

        private void UpdateLighting( float timePercent, EnvironmentLightingSettings settings )
        {
            if ( ActiveSettings.IsNull() ) { return; }

            _isDaytime = timePercent >= .3f && timePercent <= .7f;

            //Set ambient and fog
            RenderSettings.ambientLight = settings.AmbientColor.Evaluate( timePercent );
            RenderSettings.fogColor = settings.FogColor.Evaluate( timePercent );

            if ( _directionalLight.IsNull() ) { return; }

            // This block here allows us to make the intensity oscillating from these values to :
            // 0% daytime == 0% of intensity a.k.a _settings.LowerLightIntensity,
            // TO 50% daytime == 100% of intensity,
            // TO 100% of daytime == 0% of intensity a.k.a _settings.LowerLightIntensity.
            // This system mimics how the sun behaves in IRL.
            _currentLightIntensity = timePercent <= .5f
                ? settings.GreaterLightIntensity * timePercent
                : Mathf.Abs( ( ( settings.GreaterLightIntensity * timePercent ) - settings.GreaterLightIntensity ) );

            SetLightIntensity( _currentLightIntensity * 2,
                                         settings.LowerLightIntensity,
                                         settings.GreaterLightIntensity );

            SetLightColor( settings.DirectionalColor.Evaluate( timePercent ) );

            _directionalLight.transform.localRotation =
                Quaternion.Euler( new Vector3( ( timePercent * 360f ) - 90f, 170f, 0 ) );
        }

        private void SetLightIntensity( float intensity, float clampMin, float clampMax)
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

        private void SetActiveLightingSettings( WeatherSequence weatherSequence )
        {
            if ( weatherSequence.IsNull()
                || !weatherSequence.IsNull() && weatherSequence.GetLightingSettings().IsNull()
                || weatherSequence.GetWeatherType() == WeatherType.None )
            {
                ActiveSettings = _defaultLightingSettings;

                SetLightIntensity( 2f, 0f, 2f );
                SetLightColor( Color.red );

                SetCameraVolumeManagerGammaValue();
                return;
            }

            ActiveSettings = weatherSequence.GetLightingSettings();

            SetCameraVolumeManagerGammaValue();
        }

        #region OnValidate
#if UNITY_EDITOR
        private void SetCameraVolumeManagerGammaValue()
        {
            if ( !Application.isPlaying )
            {
                CameraVolumeManager cvm = ( CameraVolumeManager ) FindObjectOfType( typeof( CameraVolumeManager ) );
                cvm.NeedToBeUpdated();
            }
            else
            {
                OnLightingSettingsChanged?.Invoke();
            }
        }

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