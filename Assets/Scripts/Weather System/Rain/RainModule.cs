using DG.Tweening;
using dnSR_Coding.Attributes;
using dnSR_Coding.Utilities;
using NaughtyAttributes;
using System;
using UnityEngine;

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

            [SerializeField, NamedArrayElement( typeof( Enums.RainType ) )] private string _name;

            [field: Header( "Main" )]
            [SerializeField] private Enums.RainType _associatedRainType;
            [field: SerializeField, Range( 0, 500 )]
            public float ParticleAmount { get; private set; }

            [field: Header( "Audio" )]
            [SerializeField] private bool _hasAudio;
            [field: SerializeField, AllowNesting, ShowIf( "_hasAudio" )] 
            public SimpleAudioEvent AudioEvent { get; private set; }
        }
        public RainSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        private Tween _rainRateTween;

        public void ApplySettings( Enums.RainType rainType, ref GameObject rainEffectGO )
        {
            if ( Settings.IsEmpty() )
            {
                Settings.LogIsEmpty();
                return;
            }

            if ( rainEffectGO.TryGetComponent( out ParticleSystem rainParticleSystem ) )
            {
                ParticleSystem.EmissionModule emissionModule = rainParticleSystem.emission;

                RainSettings settings = GetSettingsByID( ( int ) rainType );
                if ( emissionModule.rateOverTime.Equals( settings.ParticleAmount ) || _rainRateTween.IsActive() ) { return; }

                Debug.Log( $"Rain setting has been applied with a rate of : {settings.ParticleAmount}." );

                _rainRateTween = DOTween.To( () => emissionModule.rateOverTime.Evaluate(.5f), _ => emissionModule.rateOverTime = _, settings.ParticleAmount, 5f );
                _rainRateTween.OnStepComplete( () =>
                {
                    emissionModule.rateOverTime = settings.ParticleAmount;
                    _rainRateTween.Kill();
                } );
            }
        }
        public void Stop( ref GameObject rainEffectGO )
        {
            if ( rainEffectGO.TryGetComponent( out ParticleSystem rainParticleSystem ) )
            {
                ParticleSystem.EmissionModule emissionModule = rainParticleSystem.emission;

                if ( emissionModule.rateOverTime.Equals( 0f ) ) { return; }

                emissionModule.rateOverTime = GetSettingsByID( 0 ).ParticleAmount;
                Debug.Log( "Rain setting has been stopped." );
            }            
        }
    }
}