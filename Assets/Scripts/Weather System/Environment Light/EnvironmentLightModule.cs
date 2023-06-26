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

        public void ApplySettings( Enums.EnvironmentLightIntensityType lightIntensity, LightController mainLightController )
        {
            EnvironmentLightSettings settings = GetSettingsByID( ( int ) lightIntensity );
            if ( settings.IsNull() )
            {
                Debug.LogError( "Environment Light Module - ApplySettings - Environment Light settings reference is null" );
                return;
            }

            if ( mainLightController.IsNull() )
            {
                Debug.LogError( "Environment Light Module - ApplySettings - Environment Light main light reference is null" );
                return;
            }

            if ( mainLightController.DoesLightIntensityEquals( settings.Intensity ) || _lightIntensityTween.IsActive() ) { return; }

            _lightIntensityTween = DOTween.To(
                () => mainLightController.GetControllerLight().intensity,
                _ => mainLightController.GetControllerLight().intensity = _,
                settings.Intensity,
                1.25f );

            _lightIntensityTween.OnComplete( () =>
            {
                mainLightController.SetLightIntensity( settings.Intensity );
                _lightIntensityTween.Kill();
            } );

            Debug.Log( $"Environment light setting has been applied with an intensity of : {settings.Intensity}." );
        }
    }
}