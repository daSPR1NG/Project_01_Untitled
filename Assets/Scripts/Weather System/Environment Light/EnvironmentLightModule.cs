using UnityEngine;
using System;
using DG.Tweening;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Environment Light Module Settings" )]
    public class EnvironmentLightModule : WeatherSystemModule<EnvironmentLightModule.EnvironmentLightSettings>
    {
        #region EnvironmentLight setting struct

        [Serializable]
        public struct EnvironmentLightSettings
        {
            public readonly int ID
            {
                get
                {
                    return ( int ) _lightIntensity;
                }
            }

            [field: CenteredHeader( "Main" )]
            [SerializeField] private Enums.Environment_LightIntensity_Type _lightIntensity;
            [field: SerializeField, Range( 0, 1 )]
            public float Intensity { get; private set; }
        }
        public EnvironmentLightSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        #endregion

        private LightController _mainLightController = null;
        private Tween _lightIntensityTween = null;

        #region Init

        public void Init( LightController mainLightController )
        {
            SetMainLightController( mainLightController );
        }

        private void SetMainLightController( LightController mainLightController )
        {
            if ( mainLightController.IsNull<GameObject>() )
            {
                this.Debugger(
                    "The mainLightController sent to this object is null.",
                    DebugType.Error );
                return;
            }

            _mainLightController = mainLightController;
        }

        #endregion

        #region Apply

        public void Apply( Enums.Environment_LightIntensity_Type lightIntensity )
        {
            EnvironmentLightSettings settings = GetSettingsByID( ( int ) lightIntensity );
            if ( settings.IsNull<EnvironmentLightSettings>() )
            {
                this.Debugger(
                    "Environment Light Module - Apply - Environment Light settings reference is null",
                    DebugType.Error );
                return;
            }

            SetLightIntensity( settings );

            this.Debugger( $"Environment light setting has been applied with an intensity of : {settings.Intensity}." );
        }

        #endregion

        #region Utils

        private void SetLightIntensity( EnvironmentLightSettings settings )
        {
            // We need to check if the main light controller is defined.
            if ( _mainLightController.IsNull<LightController>() )
            {
                this.Debugger(
                    "Environment Light Module - ApplySettings - Environment Light main light reference is null",
                    DebugType.Error );
                return;
            }

            // Then we need to check if the intensity is already equal to the new value...
            // or that it is currently set.
            if ( _mainLightController.DoesLightIntensityEquals( settings.Intensity )
                || _lightIntensityTween.IsActive() )
            {
                return;
            }

            // We call a tween to set the value gradually, it's smoother, more visually appealing.
            _lightIntensityTween = DOTween.To(
                () => _mainLightController.GetControllerLight().intensity,
                _ => _mainLightController.GetControllerLight().intensity = _,
                settings.Intensity,
                1.25f );

            // When the value has been reached, we need to kill the tween...
            // and as a security layer, we reassign the value to the what we wanted...
            // in order to make sure it is equal to this value.
            _lightIntensityTween.OnComplete( () =>
            {
                _mainLightController.SetLightIntensity( settings.Intensity );
                _lightIntensityTween.Kill();
            } );
        }

        #endregion
    }
}