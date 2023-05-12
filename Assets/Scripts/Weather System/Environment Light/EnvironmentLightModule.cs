using UnityEngine;
using dnSR_Coding.Attributes;
using System;
using dnSR_Coding.Utilities;
using DG.Tweening;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Environment Light Module Settings" )]
    public class EnvironmentLightModule : WeatherSystemModule<EnvironmentLightModule.EnvironmentLightSettings>
    {
        [Serializable]
        public struct EnvironmentLightSettings
        {
            public int ID
            {
                get
                {
                    return ( int ) _lightIntensity;
                }
            }

            [SerializeField, NamedArrayElement( typeof( Enums.EnvironmentLightIntensity ) )] private string _name;

            [field: Header( "Main" )]
            [SerializeField] private Enums.EnvironmentLightIntensity _lightIntensity;
            [field: SerializeField, Range( 0, 1 )]
            public float Intensity { get; private set; }
        }
        public EnvironmentLightSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        private Tween _lightIntensityTween;

        public void ApplySettings( Enums.EnvironmentLightIntensity lightIntensity, Light mainLightRef )
        {
            EnvironmentLightSettings settings = GetSettingsByID( ( int ) lightIntensity );
            if ( mainLightRef.intensity == settings.Intensity || _lightIntensityTween.IsActive() ) { return; }

            _lightIntensityTween = DOTween.To( () => mainLightRef.intensity, _ => mainLightRef.intensity = _, settings.Intensity, 1.25f );
            _lightIntensityTween.OnComplete( () =>
            {
                mainLightRef.intensity = settings.Intensity;
                _lightIntensityTween.Kill();
            } );

            this.Log( $"Environment light setting has been applied with an intensity of : {settings.Intensity}." );
        }
        public void Stop( Light mainLightRef )
        {
            EnvironmentLightSettings lastSettings = GetSettingsByID( Helper.GetEnumLength( typeof( Enums.EnvironmentLightIntensity ) ) - 1 );
            if ( mainLightRef.intensity == lastSettings.Intensity || _lightIntensityTween.IsActive() ) { return; }

            _lightIntensityTween = DOTween.To( () => mainLightRef.intensity, _ => mainLightRef.intensity = _, 0, 1.25f );
            _lightIntensityTween.OnComplete( () =>
            {
                mainLightRef.intensity = 0;
                _lightIntensityTween.Kill();
            } );

            this.Log( "Environment light setting has been stopped." );
        }
    }
}