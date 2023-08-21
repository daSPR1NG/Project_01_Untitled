using UnityEngine;
using System;
using DG.Tweening;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using static UnityEngine.ParticleSystem;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Fog Module Settings" )]
    public class FogModule : WeatherSystemModule<FogModule.FogSettings>
    {
        #region Fog settings struct

        [Serializable]
        public struct FogSettings
        {
            public int ID
            {
                get
                {
                    return ( int ) _associatedFogType;
                }
            }

            [SerializeField, LabeledArray( typeof( Enums.Fog_Type ) )] private string _name;

            [field: Header( "Main" )]
            [SerializeField] private Enums.Fog_Type _associatedFogType;

            [field: SerializeField, Range( 0, .085f )]
            public float FogDensity { get; private set; }

            [field: SerializeField]
            public Color FogColor { get; private set; }
        }
        public FogSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        #endregion

        private Tween _fogDensityTween = null;
        private Tween _fogColorTween = null;

        #region Apply / Stop

        public void ApplySettings( Enums.Fog_Type fogType )
        {
            // We need to make sure that a setting exists.
            FogSettings settings = GetSettingsByID( ( int ) fogType );
            if ( settings.IsNull<FogSettings>() )
            {
                Debug.LogError( "Fog Module - ApplySettings - Fog settings reference is null" );
                return;
            }

            SetFogDensity( settings );
            SetFogColor( settings );
        }
        public void Stop()
        {
            // We need to check if the fog is active or not.
            if ( !RenderSettings.fog ) { return; }

            RenderSettings.fog = false;
            RenderSettings.fogDensity = 0;

            Debug.Log( "Fog setting has been stopped." );
        }

        #endregion

        #region Utils

        private void SetFogDensity( FogSettings settings )
        {
            // We need to check if a the fog density is not already equal to the new value...
            // or that the value is being set.
            if ( RenderSettings.fogDensity == settings.FogDensity
                || _fogDensityTween.IsActive() )
            {
                return;
            }

            EnableFog();

            // We call a tween to set the value gradually, it's smoother, more visually appealing.
            _fogDensityTween = DOTween.To( () => RenderSettings.fogDensity, _ => RenderSettings.fogDensity = _, settings.FogDensity, 5f );

            // When the value has been reached, we need to kill the tween...
            // and as a security layer, we reassign the value to the what we wanted...
            // in order to make sure it is equal to this value.
            _fogDensityTween.OnComplete( () =>
            {
                RenderSettings.fogDensity = settings.FogDensity;
                _fogDensityTween.Kill();
            } );

            Debug.Log( $"Fog setting has been applied with a density of : {settings.FogDensity}." );
        }

        private void SetFogColor( FogSettings settings )
        {
            // We need to check if a the fog color is not already equal to the new value...
            // or that the value is being set.
            if ( RenderSettings.fogColor == settings.FogColor
                || _fogColorTween.IsActive() )
            {
                return;
            }

            Debug.Log( $"Fog setting has been applied with a color of : {settings.FogColor}." );

            EnableFog();

            // We call a tween to set the value gradually, it's smoother, more visually appealing.
            _fogColorTween = DOTween.To( 
                () => RenderSettings.fogColor, 
                _ => RenderSettings.fogColor = _, 
                settings.FogColor, 
                5f );

            // When the value has been reached, we need to kill the tween...
            // and as a security layer, we reassign the value to the what we wanted...
            // in order to make sure it is equal to this value.
            _fogColorTween.OnComplete( () =>
            {
                RenderSettings.fogColor = settings.FogColor;
                _fogColorTween.Kill();
            } );
        }

        /// <summary>
        /// Enables the fog if it has/is disabled.
        /// </summary>
        private void EnableFog()
        {
            if ( !RenderSettings.fog ) { RenderSettings.fog = true; }
        }

        #endregion
    }
}