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

            [SerializeField, NamedArrayElement( typeof( Enums.EnvironmentLightIntensityType ) )] private string _name;

            [field: Header( "Main" )]
            [SerializeField] private Enums.EnvironmentLightIntensityType _lightIntensity;
            [field: SerializeField, Range( 0, 1 )]
            public float Intensity { get; private set; }
        }
        public EnvironmentLightSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        private Tween _lightIntensityTween;

        public void ApplySettings( Enums.EnvironmentLightIntensityType lightIntensity, Light mainLightRef )
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
    }
}