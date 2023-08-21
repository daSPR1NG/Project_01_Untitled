using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using dnSR_Coding.Utilities.Runtime;
using static UnityEngine.ParticleSystem;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Rain Module Settings" )]
    public class RainModule : WeatherSystemModule<RainModule.RainSettings>
    {
        [PropertySpace( 5 )]

        #region Rain settings struct

        [Serializable]
        public struct RainSettings
        {
            public int ID
            {
                get
                {
                    return ( int ) _rainType;
                }
            }

            [SerializeField, LabeledArray( typeof( Enums.Rain_Type ) )] private string _name;

            [field: CstmHeader( "Main", true )]
            [SerializeField] private Enums.Rain_Type _rainType;
            [field: SerializeField, Range( 0, 500 )]
            public float RainRate { get; private set; }

            [field: Header( "Audio" )]
            [SerializeField] private bool _hasAudio;
            [field: SerializeField, ShowIf( "_hasAudio" )]
            public SimpleAudioEvent AudioEvent { get; private set; }
        }
        public RainSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        #endregion

        private GameObject _rainGO = null;

        private Tween _rainRateTween = null;
        private ParticleSystem _rainParticleSystem = null;

        #region Init

        public void Init( GameObject rainGO )
        {
            SetRainGO( rainGO );
        }

        private void SetRainGO( GameObject rainGO )
        {
            if ( rainGO.IsNull<GameObject>() )
            {
                this.Debugger(
                    "The rainGO sent to this object is null.",
                    DebugType.Error );
                return;
            }

            _rainGO = rainGO;
        }

        #endregion

        #region Apply / Stop

        /// <summary>
        /// Applies the rain -> emission == settings.RainRate, rainGO is displayed
        /// </summary>
        /// <param name="rainType"> The type of rain used by the current settings </param>
        /// <param name="rainGO"> The gameObject that holds the rain PS </param>
        public void ApplySettings( Enums.Rain_Type rainType )
        {
            // We need to check if the rain's particle system is defined or not...
            if ( _rainParticleSystem.IsNull()
                || _rainGO.IsNull() )
            {
                this.Debugger(
                    "Rain Module - ApplySettings - Rain particle system reference is null..."
                    + $" Trying to find one on {_rainGO.name}" );

                // ...if not then we need to try to get it.
                _rainParticleSystem = TryToGetRainParticleSystem( true );
            }

            // We need to check again after the first attempt to get the rain's particle system.
            if ( _rainParticleSystem.IsNull<ParticleSystem>() )
            {
                this.Debugger(
                    "Rain Module - ApplySettings - Rain particle system reference is null !" );
                return;
            }

            // Here we need to get the emission module on the PS to set the rateOverTime...
            // corresponding to the rain.RainRate of 0, cause we stopping the rain.

            EmissionModule emissionModule = _rainParticleSystem.emission;

            RainSettings settings = GetSettingsByID( ( int ) rainType );
            if ( settings.IsNull<RainSettings>() )
            {
                this.Debugger(
                    "Rain Module - ApplySettings - Rain settings reference is null",
                    DebugType.Error );
                return;
            }

            _rainGO.Display();
            SetRainRate( emissionModule, settings );
        }

        /// <summary>
        /// Stops the rain -> emission == 0, rainGO is hidden
        /// </summary>
        /// <param name="rainGO"> The gameObject that holds the rain PS </param>
        public void Stop()
        {
            // We need to check if the rain's particle system is defined or not...
            if ( _rainParticleSystem.IsNull() )
            {
                this.Debugger(
                    "Rain Module - ApplySettings - Rain particle system reference is null..."
                    + $" Trying to find one on {_rainGO.name}" );

                // ...if not then we need to try to get it
                _rainParticleSystem = TryToGetRainParticleSystem( true );
            }

            // We need to check again after the first attempt to get the rain's particle system
            if ( _rainParticleSystem.IsNull<ParticleSystem>() )
            {
                this.Debugger(
                    "Rain Module - ApplySettings - Rain particle system reference is null !" );
                return;
            }

            // Then as the PS is not required, we hide it
            _rainGO.Hide();

            // Here we need to get the emission module on the PS to set the rateOverTime
            // corresponding to the rain.RainRate of 0, cause we stopping the rain

            EmissionModule emissionModule = _rainParticleSystem.emission;

            if ( emissionModule.rateOverTime.Equals( 0f ) ) { return; }

            emissionModule.rateOverTime = GetSettingsByID( ( int ) Enums.Rain_Type.None ).RainRate;
            this.Debugger( "Rain setting has been stopped." );
        }

        #endregion

        #region Utils

        /// <summary>
        /// Sets the rain's PS emission rate equals to the rain rate of the setting.
        /// </summary>
        /// <param name="emissionModule"> The object that holds the rate parameter. </param>
        /// <param name="settings"> The current setting where we can find the correct rate value. </param>
        private void SetRainRate( EmissionModule emissionModule, RainSettings settings )
        {
            // Before setting the rain rate we need to make sure that the rate value is not already set...
            // or it is in the process of being set.

            if ( emissionModule.rateOverTime.Equals( settings.RainRate )
                || _rainRateTween.IsActive() )
            {
                return;
            }

            Debug.Log( $"Rain setting has been applied with a rate of : {settings.RainRate}." );

            // We call a tween to set the value gradually, it's smoother, more visually appealing.
            _rainRateTween = DOTween.To( () => emissionModule.rateOverTime.Evaluate( .5f ), _ => emissionModule.rateOverTime = _, settings.RainRate, 5f );

            // When the value has been reached, we need to kill the tween...
            // and as a security layer, we reassign the value to the what we wanted...
            // in order to make sure it is equal to this value.
            _rainRateTween.OnStepComplete( () =>
            {
                emissionModule.rateOverTime = settings.RainRate;
                _rainRateTween.Kill();
            } );
        }

        /// <summary>
        /// Tries to grab any PS on the given gameObject.
        /// </summary>
        /// <param name="rainGO"> The PS's holder. </param>
        /// <param name="hasAPivot"> This parameter is usefull when the PS is the children of a pivot. </param>
        /// <returns></returns>
        private ParticleSystem TryToGetRainParticleSystem( bool hasAPivot )
        {
            ParticleSystem particleSystem = null;

            this.Debugger( $"Particle system child: {_rainGO.transform.childCount}" );

            Transform transform = hasAPivot ? _rainGO.transform.GetFirstChild() : _rainGO.transform;

            foreach ( Transform trs in transform )
            {
                if ( trs.gameObject.TryGetComponent( out ParticleSystem component ) )
                {
                    particleSystem = component;
                    this.Debugger( $"Particle system found: {particleSystem.name}" );
                }
            }

            this.Debugger( $"Particle system found: {particleSystem}" );

            return particleSystem;
        }

        #endregion
    }
}