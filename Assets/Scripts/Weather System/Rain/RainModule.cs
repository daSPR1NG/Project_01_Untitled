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
    [CreateAssetMenu(menuName = "Scriptable Objects/Weather System/Modules/Create New Rain Module Settings" )]
    public class RainModule : WeatherSystemModule<RainModule.RainSettings>
    {
        [Serializable]
        public struct RainSettings
        {
            public int ID
            {
                get
                {
                    return ( int ) _associatedRainType;
                }
            }

            [SerializeField, LabeledArray( typeof( Enums.Rain_Type ) )] private string _name;

            [field: CstmHeader( "Main" , true )]
            [SerializeField] private Enums.Rain_Type _associatedRainType;
            [field: SerializeField, Range( 0, 500 )]
            public float ParticleAmount { get; private set; }

            [field: Header( "Audio" )]
            [SerializeField] private bool _hasAudio;
            [field: SerializeField, ShowIf( "_hasAudio" )] 
            public SimpleAudioEvent AudioEvent { get; private set; }
        }
        public RainSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        private Tween _rainRateTween;

        public void ApplySettings( Enums.Rain_Type rainType, GameObject rainGO )
        {
            if ( rainGO.TryGetComponent( out ParticleSystem rainParticleSystem ) )
            {
                EmissionModule emissionModule = rainParticleSystem.emission;

                RainSettings settings = GetSettingsByID( ( int ) rainType );
                if ( settings.IsNull<RainSettings>() )
                {
                    Debug.LogError( "Rain Module - ApplySettings - Rain settings reference is null" );
                    return;
                }

                rainGO.Display();
                SetRainRate( emissionModule, settings );
            }
            else {
                Debug.LogError( "Rain Module - ApplySettings - Rain particle system reference is null" );
            }
        }
        public void Stop( GameObject rainGO )
        {
            if ( rainGO.TryGetComponent( out ParticleSystem rainParticleSystem ) )
            {
                EmissionModule emissionModule = rainParticleSystem.emission;

                if ( emissionModule.rateOverTime.Equals( 0f ) ) { return; }

                emissionModule.rateOverTime = GetSettingsByID( 0 ).ParticleAmount;
                Debug.Log( "Rain setting has been stopped." );
            }
            else {
                Debug.LogError( "Rain Module - ApplySettings - Rain particle system reference is null" );
            }

            rainGO.Hide();
        }

        private void SetRainRate( EmissionModule emissionModule, RainSettings settings )
        {
            if ( emissionModule.rateOverTime.Equals( settings.ParticleAmount ) || _rainRateTween.IsActive() ) { return; }

            Debug.Log( $"Rain setting has been applied with a rate of : {settings.ParticleAmount}." );

            _rainRateTween = DOTween.To( () => emissionModule.rateOverTime.Evaluate( .5f ), _ => emissionModule.rateOverTime = _, settings.ParticleAmount, 5f );
            _rainRateTween.OnStepComplete( () =>
            {
                emissionModule.rateOverTime = settings.ParticleAmount;
                _rainRateTween.Kill();
            } );
        }
    }
}