using UnityEngine;
using ExternalPropertyAttributes;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using System;

namespace dnSR_Coding
{
    public enum WeatherType { None, Rainy, Sunny, }

    [RequireComponent( typeof( EnvironmentLightingManager ) )]
    [RequireComponent( typeof( TimeController ) )]

    ///<summary> WeatherSystemManager description <summary>
    [Component("WeatherSystemManager", "")]
    [DisallowMultipleComponent]
    public class WeatherSystemManager : MonoBehaviour, IDebuggable
    {
        [Header( "DEPENDENCIES" )]

        [SerializeField] WeatherType _weatherType = WeatherType.None;   

        [Space( 10f )]

        [Header( "DEPENDENCIES" )]

        [SerializeField] private List<WeatherSequence> _weatherSequences = new();

        private WeatherSequence _activeWeatherSequence;
        private EnvironmentLightingManager _environmentLightingManager;
        private TimeController _timeController;
        private int _weatherTypeIndex;

        public static Action<WeatherSequence> OnWeatherChanged;

        #region Debug

        [ Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() 
        {
            TimeController.OnFallingNight += ActiveWeather.HideSunShafts;
            TimeController.OnRisingSun += ActiveWeather.DisplaySunShafts;
        }

        void OnDisable() 
        {
            TimeController.OnFallingNight -= ActiveWeather.HideSunShafts;
            TimeController.OnRisingSun -= ActiveWeather.DisplaySunShafts;
        }

        #endregion

        void Awake() => Init();
        void Init()
        {
            GetLinkedComponents();
            PopulateWeatherSequences();
            SetWeather();
        }

        void GetLinkedComponents()
        {
            if ( _environmentLightingManager.IsNull() ) { _environmentLightingManager = GetComponent<EnvironmentLightingManager>(); }
            if ( _timeController.IsNull() ) { _timeController = GetComponent<TimeController>(); }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if ( Application.isEditor && KeyCode.W.IsPressed() )
            {
                //_weatherType = ( WeatherType ) ( _weatherTypeIndex++ % Enum.GetValues( typeof( WeatherType ) ).Length );
                if ( _weatherType == WeatherType.Rainy ) _weatherType = WeatherType.Sunny;
                else if ( _weatherType == WeatherType.Sunny ) _weatherType = WeatherType.Rainy;

                SetWeather();
            }
        }
#endif

        private void SetWeather()
        {
            if ( !_activeWeatherSequence.IsNull() ) { _activeWeatherSequence.RemoveSequence(); }

            switch ( _weatherType )
            {
                case WeatherType.None:

                    _activeWeatherSequence = null;
                    if ( !GetActiveWeather().IsNull() ) { GetActiveWeather().RemoveSequence(); }
                    break;

                case WeatherType.Rainy:

                    _activeWeatherSequence = GetWeatherSequenceByType( WeatherType.Rainy );
                    _activeWeatherSequence.ApplySequence( _timeController.IsDaytime );
                    break;

                case WeatherType.Sunny:

                    _activeWeatherSequence = GetWeatherSequenceByType( WeatherType.Sunny );
                    _activeWeatherSequence.ApplySequence( _timeController.IsDaytime );
                    break;
            }            

            OnWeatherChanged?.Invoke( _activeWeatherSequence );
        }

        public WeatherSequence ActiveWeather => _activeWeatherSequence;

        #region Utils - Get Specific Weather Sequence

        private WeatherSequence GetWeatherSequenceByType( WeatherType type )
        {
            if ( _weatherSequences.IsEmpty() ) { return null; }

            for ( int i = _weatherSequences.Count - 1; i >= 0; i-- )
            {
                if ( _weatherSequences [ i ].IsNull() || _weatherSequences [ i ].GetWeatherType() != type ) 
                {
                    continue; 
                }

                return _weatherSequences [ i ];
            }

            return null;
        }
        private WeatherSequence GetActiveWeather()
        {
            if ( _weatherSequences.IsEmpty() ) { return null; }

            for ( int i = _weatherSequences.Count - 1; i >= 0; i-- )
            {
                if ( _weatherSequences [ i ].IsNull() || !_weatherSequences [ i ].IsApplied ) 
                {
                    continue; 
                }

                return _weatherSequences [ i ];
            }

            return null;
        }

        #endregion

        #region OnValidate

#if UNITY_EDITOR

        private void PopulateWeatherSequences()
        {
            if ( transform.HasNoChild() ) { return; }

            for ( int i = 0; i < transform.childCount; i++ )
            {
                var wS = transform.GetChild( i ).GetComponent<WeatherSequence>();

                if ( wS.IsNull() ) { continue; }

                _weatherSequences.AppendItem( wS, false );
            }
        }

        private void OnValidate()
        {
            GetLinkedComponents();

            if ( Application.isPlaying ) { return; }
            
            PopulateWeatherSequences();
            SetWeather();
        }
#endif

        #endregion

        private void OnGUI()
        {
            if ( !Application.isEditor || _activeWeatherSequence.IsNull() )
            {
                GUIContent nullContent = new( "WeatherSystem Manager - No active weather found." );
                GUI.Label( new Rect( 5, Screen.height - 45, 350, 25 ), nullContent );
                return; 
            }

            GUIContent content = new(
                "WeatherSystem Manager - "
                + _activeWeatherSequence.GetWeatherType()
                + " Weather is active." );

            GUI.Label( new Rect( 5, Screen.height - 45, 350, 25 ), content );
        }
    }
}