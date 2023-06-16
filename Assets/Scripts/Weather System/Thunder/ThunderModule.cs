using UnityEngine;
using dnSR_Coding.Utilities;
using System;
using dnSR_Coding.Attributes;
using NaughtyAttributes;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace dnSR_Coding
{
    // NOTES : 
    // Flickering Rate => Range entre deux valeurs -> random choisi
    // Curve => tableau de différentes intensités


    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Thunder Module Settings" )]
    public class ThunderModule : WeatherSystemModule<ThunderModule.ThunderSettings>, IDebuggable
    {
        [Serializable]
        public struct ThunderSettings
        {
            public int ID
            {
                get
                {
                    return ( int ) _associatedThunderType;
                }
            }

            [SerializeField, NamedArrayElement( typeof( Enums.ThunderType ) )] private string _name;

            [field: Header( "Main" )]
            [SerializeField] private Enums.ThunderType _associatedThunderType;
            [SerializeField, MinMaxSlider( 0, 1 )] private Vector2 _flickeringRate;
            [SerializeField, MinMaxSlider( 0, 5 )] private Vector2 _timerBetweenConsecutiveApplications;
            [SerializeField] private List<AnimationCurve> _flickeringCurves;

            [field: Header( "Audio" )]
            [SerializeField] private bool _hasAudio;
            [field: SerializeField, AllowNesting, ShowIf( "_hasAudio" )]
            public SimpleAudioEvent AudioEvent { get; private set; }

            public readonly Vector2 GetFlickeringRate() => _flickeringRate;
            public readonly Vector2 GetTimerBetweenConsecutiveApplications() => _timerBetweenConsecutiveApplications;
            public readonly List<AnimationCurve> GetFlickeringCurves() => _flickeringCurves;
        }
        public ThunderSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        private Coroutine thunderCoroutine;
        private int _currentKey = 0;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        public void ApplySettings( MonoBehaviour monoBehaviour, Enums.ThunderType thunderType, Light thunderLight )
        {
            if ( !thunderCoroutine.IsNull() ) { return; }

            ThunderSettings settings = GetSettingsByID( ( int ) thunderType );
            if ( settings.IsNull() )
            {
                Debug.LogError( "Thunder Module - ApplySettings - Thunder settings reference is null" );
                return;
            }
            this.Debugger( $"Thunder setting has been applied with ID : {settings.ID}." );

            if ( thunderLight.IsNull() ) {
                Debug.LogError( "Thunder Module - ApplySettings - Thunder light reference is null" );
                return;
            }

            thunderCoroutine = monoBehaviour.StartCoroutine( StartLightFlickering( settings, thunderLight ) );
        }
        public void Stop( MonoBehaviour monoBehaviour )
        {
            if ( thunderCoroutine.IsNull() ) { return; }

            this.Debugger( "Thunder setting has been stopped." );

            monoBehaviour.StopCoroutine( thunderCoroutine );
            thunderCoroutine = null;
        }

        private IEnumerator StartLightFlickering( ThunderSettings settings, Light thunderLight )
        {
            AnimationCurve currentCurve = null;
            float randomFlickerRate = 0;
            float randomTimerBetweenConsecutiveApplications = 0;

            do
            {
                bool hasLastKeyReached = !currentCurve.IsNull() && _currentKey >= ( currentCurve.length - 1 );

                // BLOC THAT NEEDS TO ITERATE ONCE (When no curve has been chosen or when reaching the end of a curve) --
                if ( currentCurve.IsNull()
                    || !currentCurve.IsNull() && hasLastKeyReached )
                {
                    this.Debugger( "Curve has been picked, a thunder will be shot" );

                    thunderLight.intensity = 0;

                    // Get a random curve...
                    int randomCurveIndex = UnityEngine.Random.Range( 0, settings.GetFlickeringCurves().Count );
                    currentCurve = settings.GetFlickeringCurves() [ randomCurveIndex ];
                    this.Debugger( "Random curve picked : " + randomCurveIndex );

                    // Pick a random flickering rate value...
                    randomFlickerRate = UnityEngine.Random.Range(
                        settings.GetFlickeringRate().x,
                        settings.GetFlickeringRate().y );
                    this.Debugger( "Random flickering rate picked : " + randomFlickerRate );
                    // Pick a random timer between two consecutive applications...
                    randomTimerBetweenConsecutiveApplications = UnityEngine.Random.Range(
                        settings.GetTimerBetweenConsecutiveApplications().x,
                        settings.GetTimerBetweenConsecutiveApplications().y );
                    this.Debugger( "Random timer picked : " + randomTimerBetweenConsecutiveApplications );
                }
                // BLOC THAT NEEDS TO ITERATE ONCE (When no curve has been chosen or when reaching the end of a curve) --

                // BLOC THAT NEEDS TO ITERATE CONTINUOUSLY --
                _currentKey = ( _currentKey + 1 ) % currentCurve.length;
                this.Debugger( "Current Key : " + _currentKey );

                // Adjust flickering rate when the sequence is over...
                float rate = hasLastKeyReached ? randomTimerBetweenConsecutiveApplications : randomFlickerRate;
                this.Debugger( "Rate value : " + rate + " ON | Length : " + currentCurve.length );

                thunderLight.intensity = hasLastKeyReached ? 0f : currentCurve.keys [ _currentKey ].value;
                this.Debugger( "Current thunder intensity : " + thunderLight.intensity );
                // BLOC THAT NEEDS TO ITERATE CONTINUOUSLY --

                yield return new WaitForSeconds( rate );

            } while ( true );
        }
    }
}