using UnityEngine;
using ExternalPropertyAttributes;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using System;

namespace dnSR_Coding
{
    public enum WeatherType { None, Rainy, Sunny, }

    [RequireComponent( typeof( EnvironmentLightingManager ) )]

    ///<summary> WeatherSystemManager description <summary>
    [Component("WeatherSystemManager", "")]
    [DisallowMultipleComponent]
    public class WeatherSystemManager : MonoBehaviour, IDebuggable
    {
        [Title( "DEPENDENCIES", 12, "white" )]

        [SerializeField] WeatherType _weatherType = WeatherType.None;   

        [Space( 10f )]

        [Title( "DEPENDENCIES", 12, "white" )]

        [SerializeField] private List<WeatherSequence> _weatherSequences = new();

        private WeatherSequence _activeWeatherSequence;
        private EnvironmentLightingManager _environmentLightingManager;
        
        private int _weatherTypeIndex;

        public static Action<WeatherSequence> OnWeatherChanged;

        #region Debug

        [ Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

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
        }

        private void Update()
        {
            if ( KeyCode.W.IsPressed() )
            {
                _weatherType = ( WeatherType ) ( _weatherTypeIndex++ % Enum.GetValues( typeof( WeatherType ) ).Length );
                SetWeather();
            }
        }

        private void SetWeather()
        {
            if ( !_activeWeatherSequence.IsNull() )
            {
                _activeWeatherSequence.RemoveWeatherSequence();
            }

            switch ( _weatherType )
            {
                case WeatherType.None:

                    _activeWeatherSequence = null;
                    if ( !GetActiveWeather().IsNull() ) { GetActiveWeather().RemoveWeatherSequence(); }
                    break;

                case WeatherType.Rainy:

                    _activeWeatherSequence = GetWeatherSequenceByType( WeatherType.Rainy );
                    _activeWeatherSequence.ApplyWeatherSequence();
                    break;

                case WeatherType.Sunny:

                    _activeWeatherSequence = GetWeatherSequenceByType( WeatherType.Sunny );
                    _activeWeatherSequence.ApplyWeatherSequence();
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

                _weatherSequences.AddItem( wS, false );
            }
        }

        private void OnValidate()
        {
            GetLinkedComponents();
            SetWeather();

            if ( Application.isPlaying ) { return; }

            PopulateWeatherSequences();
        }
#endif

        #endregion

        private void OnGUI()
        {
            if ( !Application.isEditor || _activeWeatherSequence.IsNull() )
            {
                GUIContent nullContent = new("No active weather found." );
                GUI.Label( new Rect( 5, Screen.height - 45, 350, 25 ), nullContent );
                return; 
            }

            GUIContent content = new(
                _activeWeatherSequence.GetWeatherType()
                + " Weather is active." );

            GUI.Label( new Rect( 5, Screen.height - 45, 350, 25 ), content );
        }
    }
}