using UnityEngine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System;
using UnityEditor.ShaderGraph.Internal;

namespace dnSR_Coding
{
    // Required components or public findable enum here.
    [RequireComponent( typeof( WeatherSystemManager ) )]
    [RequireComponent( typeof( EnvironmentLightingController ) )]

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/EnvironmentDaytimeManager" )]

    ///<summary> EnvironmentDaytimeManager description <summary>
    [Component("TimeController", "")]
    [DisallowMultipleComponent, ExecuteAlways]
    public class TimeController : MonoBehaviour, IDebuggable
    {
        [Header( "DEPENDENCIES" )]

        [SerializeField] private bool _updateTimeOfDay = true;
        [InfoBox( "The value is in seconds and does not exceeds 10 minutes realtime (600s)." )]
        [SerializeField, Range( 1, 600 )] private float _dayDuration = 480f;
        [SerializeField, Range( 0, 600 ), ShowIf( "IsDebuggable" )] private float _timeOfDay = 240f;
        [ShowNonSerializedField] private string _daytimeInMinutesAndSecondsFormat;
        [ShowNonSerializedField] private string _daytimeInHoursFormat;

        [ShowNonSerializedField] float _currentTimeOfDay;
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
                    OnFallingNight?.Invoke( false );                    
                }
                // Daytime block
                else if ( !_isDaytime && value )
                {
                    Debug.Log( "Is Daytime value changed to true" );

                    // Raising the event to subsribers.
                    OnRisingSun?.Invoke( true );
                }

                _isDaytime = value;
            }
        }

        private WeatherSystemManager _weatherSystemManager;
        private EnvironmentLightingController _environmentLightingManager;

        public static Action<bool> OnFallingNight;
        public static Action<bool> OnRisingSun;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        void Awake() => Init();
        
        // Set all datas that need it at the start of the game
        void Init()
        {
            GetLinkedComponents();
        }

        // Put all the get component here, it'll be easier to follow what we need and what we collect.
        // This method is called on Awake() + OnValidate() to set both in game mod and in editor what this script needs.
        void GetLinkedComponents()
        {
            if ( _weatherSystemManager.IsNull() ) { _weatherSystemManager = GetComponent<WeatherSystemManager>(); }
            if ( _environmentLightingManager.IsNull() ) { _environmentLightingManager = GetComponent<EnvironmentLightingController>(); }
        }

        private void Update() => UpdateTimeOfDay();

        /// <summary>
        /// Updates current time of day, tracking wether its night or daytime.
        /// </summary>
        private void UpdateTimeOfDay()
        {
            if ( !_updateTimeOfDay || Application.isPlaying && GameManager.Instance.IsGamePaused() ) { return; }

            _timeOfDay += Helper.RealDeltaTime( false );
            _timeOfDay %= _dayDuration; //Modulus to ensure always between 0-maxValue

            _currentTimeOfDay = _timeOfDay / _dayDuration;

            HandleCurrentTimeOfday();

            // Editor tests
            if ( !Application.isPlaying )
            {
                // Change active settings to the normal one matching the current weather.
                _environmentLightingManager.SetActiveSettings( IsDaytime );

                if ( !IsDaytime )
                {
                    _weatherSystemManager.ActiveWeather.HideSunShafts();
                    return;
                }
                _weatherSystemManager.ActiveWeather.DisplaySunShafts( IsDaytime );
            }
        }

        /// <summary>
        /// Assigns a value to "IsDaytime". When its value change to its previous value it executes instructions set in it declaration above.
        /// </summary>
        private void HandleCurrentTimeOfday()
        {
            IsDaytime = _currentTimeOfDay >= .2f && _currentTimeOfDay <= .8f;
            GetTime();
        }

        /// <summary>
        /// Transforms current time of day in hours format string and in a minutes/seconds format.
        /// This is order to track what time is it in game and how long a day lasts.
        /// </summary>
        private void GetTime()
        {
            float t = _currentTimeOfDay * 24;
            //Debug.Log( t );

            // Convert to Hours
            float hours = ExtMathfs.Floor( t );

            // Convert to Minutes
            t *= 60;
            float minutes = ExtMathfs.Floor( t % 60 );

            // Convert to Seconds
            t *= 60;
            float seconds = ExtMathfs.Floor( t % 60 );

            _daytimeInMinutesAndSecondsFormat = _timeOfDay.InMinutesAndSeconds();
            _daytimeInHoursFormat = hours + ":" + minutes + ":" + seconds;
        }

        // The argument is commented to enable the use of ButtonAttribute, remove it when tests are done !
        /// <summary>
        /// Add an amount of time to the current time value.
        /// </summary>
        /// /// <param name="timeToAdd"> Time value to add. </param>
        [Button]
        private void AddTime( /*int timeToAdd*/ )
        {
            float t = ( _dayDuration / 24 ) * 5/*timeToAdd*/;
            Debug.Log( t );

            float hourToAdd = t;
            _timeOfDay += hourToAdd;

            _currentTimeOfDay = _timeOfDay / _dayDuration;
            GetTime();
        }

        public float GetCurrentTimeOfDay() => _currentTimeOfDay;

        [Button]
        private void Reset()
        {
            _dayDuration = 480f;
            _timeOfDay = 0;
            _currentTimeOfDay = 0;

            GetTime();

            _updateTimeOfDay = false;
        }

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            GetLinkedComponents();
        }
#endif

        #endregion

        private void OnGUI()
        {
            if ( !Application.isEditor || !_updateTimeOfDay )
            {
                GUIContent nullContent = new( "Time Controller is not updated." );
                GUI.Label( new Rect( 5, Screen.height - 65, 350, 25 ), nullContent );
                return;
            }

            GUIContent content = new( "Time Controller - It is daytime : " + IsDaytime );

            GUI.Label( new Rect( 5, Screen.height - 65, 350, 25 ), content );
        }
    }
}