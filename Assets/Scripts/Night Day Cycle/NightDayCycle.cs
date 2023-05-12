using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    [RequireComponent( typeof( EnvironmentLightManager ) )]

    ///<summary> NightDayCycle description <summary>
    [DisallowMultipleComponent]
    public class NightDayCycle : MonoBehaviour
    {
        [Header( "Ambien light settings" )]
        [SerializeField] private AmbientLightingPreset _lightingPreset;        

        [Header( "Night and day settings" )]
        [SerializeField] private bool _stopNightAndDayCycle = false;
        [SerializeField, Range( 1, 240 )] private float _dayDuration = 120f;
        [SerializeField, Range( 0, 240 )] private float _timeOfDay;

        private EnvironmentLightManager _environmentLightManager;
        private Light _ambientLight;
        private Light _sunLight;
        private float _currentTimeOfDay = 0;

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

        private void Awake() => Init();
        private void Init()
        {
            SetLightsReference();
        }

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
            SetDirectionalLightColor( timePercent );
            SetFogColor( timePercent );

            RotateMainLight( timePercent );
            SetSunIntensity_BasedOnDayTime();            
        }            

        #region Color setter for ambient, directional light and fog colors

        private void SetAmbientColor( float timePercent )
        {
            RenderSettings.ambientLight = _lightingPreset.AmbientColor.Evaluate( timePercent );
        }

        private void SetDirectionalLightColor( float timePercent )
        {
            if ( _ambientLight.IsNull() )
            {
                _ambientLight.LogNullException();
                return;
            }

            _ambientLight.color = _lightingPreset.DirectionalColor.Evaluate( timePercent );
        }

        private void SetFogColor( float timePercent )
        {
            if ( !RenderSettings.fog ) { return; }

            RenderSettings.fogColor = _lightingPreset.FogColor.Evaluate( timePercent );
        }


        #endregion

        private void RotateMainLight( float timePercent )
        {
            if ( _ambientLight.IsNull() )
            {
                _ambientLight.LogNullException();
                return;
            }

            _ambientLight.transform.localRotation =
                Quaternion.Euler( new Vector3( ( timePercent * 360f ) - 90f, 170f, 0 ) );
        }

        private void SetSunIntensity_BasedOnDayTime()
        {
            if ( _sunLight.IsNull() ) 
            {
                _sunLight.LogNullException();
                return; 
            }

            //  Calcul de la première partie de la journée - début -> médian
            float firstPartOfDayLength = ( _dayDuration * GetMedianOfDay() ) - ( _dayDuration * DAY_START_THRESHOLD );
            //  Calcul de la deuxième partie de la journée - médian -> fin
            float secondPartOfDayLength = ( _dayDuration * DAY_END_THRESHOLD ) - ( _dayDuration * GetMedianOfDay() );

            if ( IsDaytime && _currentTimeOfDay < GetMedianOfDay() )
            {
                _sunLight.intensity = ( _timeOfDay - ( _dayDuration * DAY_START_THRESHOLD ) ) / firstPartOfDayLength;
                return;
            }

            if ( _sunLight.intensity <= 0 ) { return; }

            _sunLight.intensity = -( _timeOfDay - ( _dayDuration * DAY_END_THRESHOLD ) ) / secondPartOfDayLength;
        }

        private float GetMedianOfDay()
        {
            return ( DAY_START_THRESHOLD + DAY_END_THRESHOLD ) / 2;
        }

        private void SetLightsReference()
        {
            _environmentLightManager = GetComponent<EnvironmentLightManager>();

            _ambientLight = _environmentLightManager.GetMainLight();
            _sunLight = _environmentLightManager.GetAdditionalLight();
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