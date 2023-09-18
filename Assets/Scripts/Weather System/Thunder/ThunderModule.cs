using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using dnSR_Coding.Utilities.Interfaces;
using dnSR_Coding.Utilities.Runtime;
using Sirenix.OdinInspector;

namespace dnSR_Coding
{
    // NOTES : 
    // Flickering Rate => Range entre deux valeurs -> random choisi
    // Curve => tableau de différentes intensités


    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Thunder Module Settings" )]
    public class ThunderModule : WeatherSystemModule<ThunderModule.ThunderSettings>
    {
        #region Thunder settings struct

        [Serializable]
        public struct ThunderSettings
        {
            public readonly int ID
            {
                get
                {
                    return ( int ) _associatedThunderType;
                }
            }

            [field: CenteredHeader( "Main settings" )]
            [SerializeField] private Enums.Thunder_Type _associatedThunderType;

            [field: CenteredHeader( "Thunder application settings" )]
            [SerializeField, MinMaxSlider( 0, 5 )] private Vector2 _timerBetweenTwoStrikes;
            [SerializeField, MinMaxSlider( 0, 1 )] private Vector2 _flickeringRate;
            [SerializeField] private List<AnimationCurve> _flickeringCurves;

            [field: CenteredHeader( "Audio settings" )]
            [SerializeField] private bool _hasAudio;
            [field: SerializeField, ShowIf( "_hasAudio" )]
            public SimpleAudioEvent AudioEvent { get; private set; }

            public readonly Vector2 GetFlickeringRate() => _flickeringRate;
            public readonly Vector2 GetTimerBetweenConsecutiveApplications() => _timerBetweenTwoStrikes;
            public readonly List<AnimationCurve> GetFlickeringCurves() => _flickeringCurves;
        }
        public ThunderSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        #endregion

        private MonoBehaviour _monoBehaviour = null;
        private LightController _thunderLightController = null;

        private Coroutine thunderCoroutine = null;
        private int _currentKey = 0;

        #region Init

        public void Init( MonoBehaviour monoBehaviour, LightController thunderlightController )
        {
            SetMonoBehaviour( monoBehaviour );
            SetThunderLightController( thunderlightController );
        }

        private void SetMonoBehaviour( MonoBehaviour monoBehaviour )
        {
            if ( monoBehaviour.IsNull<GameObject>() )
            {
                this.Debugger(
                    "The monoBehaviour sent to this object is null.",
                    DebugType.Error );
                return;
            }

            _monoBehaviour = monoBehaviour;
        }
        private void SetThunderLightController( LightController lightController )
        {
            if ( lightController.IsNull<LightController>() )
            {
                this.Debugger(
                    "The lightController sent to this object is null.",
                    DebugType.Error );
                return;
            }

            _thunderLightController = lightController;
        }

        #endregion

        #region Apply / Stop

        /// <summary>
        /// Applies the settings.
        /// </summary>
        /// <param name="monoBehaviour"> The user needed to use coroutine. </param>
        /// <param name="thunderType"> The type of thunder applied. </param>
        /// <param name="thunderLightController"> 
        /// The light controller used for thunder, can be found in Lights referencer.
        /// </param>
        /// <param name="mainLightColor">
        /// The current main light color (especially useful for the daytime light influence).
        /// </param>
        public void Apply(
            Enums.Thunder_Type thunderType,
            Color mainLightColor )
        {
            if ( !thunderCoroutine.IsNull<Coroutine>() ) { return; }

            ThunderSettings settings = GetSettingsByID( ( int ) thunderType );
            if ( settings.IsNull<ThunderSettings>() )
            {
                this.Debugger( 
                    "Thunder Module - Apply - Thunder settings reference is null",
                    DebugType.Error );
                return;
            }
            this.Debugger( $"Thunder setting has been applied with ID : {settings.ID}." );

            if ( _thunderLightController.IsNull<LightController>() )
            {
                this.Debugger(
                    "Thunder Light Module - Apply - Thunder light controller reference is null",
                    DebugType.Error );
                return;
            }

            thunderCoroutine = _monoBehaviour.StartCoroutine( 
                StartLightFlickering( settings, mainLightColor ) );

            this.Debugger( $"Thunder setting has been applied with a flickering rate of : {settings.GetFlickeringRate()}." );
        }

        /// <summary>
        /// Stops the thunder.
        /// </summary>
        /// <param name="monoBehaviour"> The user needed to use coroutine. </param>
        public void Stop()
        {
            // We need to check if a thunder coroutine is defined.
            if ( thunderCoroutine.IsNull<Coroutine>() ) { return; }

            // Then we stop this coroutine.
            _monoBehaviour.StopCoroutine( thunderCoroutine );
            this.Debugger( "Thunder setting has been stopped." );

            // As a security layer, we set it to null
            thunderCoroutine = null;
        }

        #endregion

        #region Utils

        /// <summary>
        /// Flickers the thunder light to imitate the effect or IRL thunder.
        /// </summary>
        /// <param name="settings"> The settings that hold all the parameter to use. </param>
        /// The light controller used for thunder, can be found in Lights referencer.
        /// <param name="mainLightColor">
        /// The current main light color (especially useful for the daytime light influence).
        /// </param>
        /// <returns></returns>
        private IEnumerator StartLightFlickering( 
            ThunderSettings settings, 
            Color mainLightColor )
        {
            AnimationCurve currentCurve = null;
            float randomFlickerRate = 0;
            float randomTimerBetweenConsecutiveApplications = 0;

            _thunderLightController.SetLightColor( mainLightColor );

            do
            {
                bool hasLastKeyReached = !currentCurve.IsNull<AnimationCurve>() && _currentKey >= ( currentCurve.length - 1 );

                // BLOC THAT NEEDS TO ITERATE ONCE (When no curve has been chosen or when reaching the end of a curve) --
                if ( currentCurve.IsNull<AnimationCurve>()
                    || !currentCurve.IsNull<AnimationCurve>() && hasLastKeyReached )
                {
                    this.Debugger( "Curve has been picked, a thunder will be shot" );

                    _thunderLightController.SetLightIntensity( 0 );

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

                _thunderLightController.SetLightIntensity( hasLastKeyReached ? 0f : currentCurve.keys [ _currentKey ].value );
                this.Debugger( "Current thunder intensity : " + _thunderLightController.GetControllerLight().intensity );
                // BLOC THAT NEEDS TO ITERATE CONTINUOUSLY --

                yield return new WaitForSeconds( rate );

            } while ( true );
        }

        #endregion
    }
}