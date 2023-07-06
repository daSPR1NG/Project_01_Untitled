using UnityEngine;
using System;
using DG.Tweening;
using dnSR_Coding.Utilities;
using dnSR_Coding.Attributes;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Fog Module Settings" )]
    public class FogModule : WeatherSystemModule<FogModule.FogSettings>
    {
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

            [SerializeField, LabeledArray( typeof( Enums.FogType ) )] private string _name;

            [field: Header( "Main" )]
            [SerializeField] private Enums.FogType _associatedFogType;

            [field: SerializeField, Range( 0, .085f )]
            public float FogDensity { get; private set; }

            [field: SerializeField]
            public Color FogColor { get; private set; }
        }
        public FogSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        private Tween _fogDensityTween;
        private Tween _fogColorTween;

        public void ApplySettings( Enums.FogType fogType )
        {
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
            if ( !RenderSettings.fog ) { return; }

            RenderSettings.fog = false;
            RenderSettings.fogDensity = 0;

            Debug.Log( "Fog setting has been stopped." );
        }

        private void SetFogDensity( FogSettings settings )
        {
            if ( RenderSettings.fogDensity == settings.FogDensity || _fogDensityTween.IsActive() ) { return; }

            Debug.Log( $"Fog setting has been applied with a density of : {settings.FogDensity}." );

            if ( !RenderSettings.fog ) { RenderSettings.fog = true; }
            _fogDensityTween = DOTween.To( () => RenderSettings.fogDensity, _ => RenderSettings.fogDensity = _, settings.FogDensity, 5f );
            _fogDensityTween.OnComplete( () =>
            {
                RenderSettings.fogDensity = settings.FogDensity;
                _fogDensityTween.Kill();
            } );
        }

        private void SetFogColor( FogSettings settings )
        {
            if ( RenderSettings.fogColor == settings.FogColor || _fogColorTween.IsActive() ) { return; }

            Debug.Log( $"Fog setting has been applied with a color of : {settings.FogColor}." );

            if ( !RenderSettings.fog ) { RenderSettings.fog = true; }
            _fogColorTween = DOTween.To( () => RenderSettings.fogColor, _ => RenderSettings.fogColor = _, settings.FogColor, 5f );
            _fogColorTween.OnComplete( () =>
            {
                RenderSettings.fogColor = settings.FogColor;
                _fogColorTween.Kill();
            } );
        }
    }    
}