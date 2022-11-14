using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using DG.Tweening;

namespace dnSR_Coding
{
    // Required components or public findable enum here.    

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/WeatherSequence" )]

    ///<summary> WeatherSequence description <summary>
    [Component("WeatherSequence", "")]
    [DisallowMultipleComponent]
    public class WeatherSequence : MonoBehaviour, IDebuggable
    {
        [Title( "SETTINGS", 12, "white" )]

        [SerializeField] private bool _isApplied = false;
        [SerializeField] private WeatherType _weatherType = WeatherType.None;
        [SerializeField, Range( 0f, 15f )] private float _elementsFadeSpeed = 1f;
        [SerializeField, ExposedScriptableObject] private EnvironmentLightingSettings _lightingSettings;

        [Space( 10f )]

        [Title( "ELEMENTS", 12, "white" )]

        [SerializeField] private List<Material> _sunShaftMaterials = new ();
        [SerializeField] private List<SequenceParticleSystemData> _visualEffects = new ();

        private Coroutine _visualEffectRoutine = null;

        [System.Serializable]
        public class SequenceParticleSystemData
        {
            public ParticleSystem ParticleSystem;
            public float Alpha;
        }

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() {}

        void OnDisable() 
        {
            StopAllCoroutines();
        }

        #endregion

        private void Update()
        {
            #if UNITY_EDITOR

            if ( KeyCode.B.IsPressed() )
            {
                ApplyWeatherSequence();
            }

            if ( KeyCode.N.IsPressed() )
            {
                RemoveWeatherSequence();
            }
            #endif
        }

        [ContextMenu( "Apply Sequence" ), Button]
        public void ApplyWeatherSequence()
        {
            if ( _isApplied ) { return; }

            // Smoothly turn sunshaft alpha from O to max value
            SetSunshaftsAlphaValue( 1f );

            // Smoothly turn VFX start color alpha from 0 to max value
            SetVisualEffectsAlphaValue( false );

            _isApplied = true;
        }

        [ContextMenu( "Remove Sequence" ), Button]
        public void RemoveWeatherSequence()
        {
            if ( !_isApplied ) { return; }

            // Smoothly turn sunshaft alpha from O to max value
            SetSunshaftsAlphaValue( 0f );

            // Smoothly turn VFX start color alpha from 0 to max value
            SetVisualEffectsAlphaValue( true );

            _isApplied = false;
        }

        #region Set Weather elements handle

        private void SetSunshaftsAlphaValue( float alphaToReach )
        {
            if ( _sunShaftMaterials.IsEmpty() ) { return; }

            for ( int i = 0; i < _sunShaftMaterials.Count; i++ )
            {
                if ( _sunShaftMaterials [ i ].color.a == alphaToReach ) { continue; }

                _sunShaftMaterials [ i ].DOFade( alphaToReach, _elementsFadeSpeed );
            }
        }

        private void SetVisualEffectsAlphaValue( bool fadesOut )
        {
            if ( _visualEffects.IsEmpty() ) { return; }

            for ( int i = 0; i < _visualEffects.Count; i++ )
            {
                float alphaToReach = fadesOut ? 0 : _visualEffects [ i ].Alpha;

                if ( _visualEffects [ i ].ParticleSystem.main.startColor.color.a == alphaToReach ) { continue; }

                Color newColor = new( 1, 1, 1, _visualEffects [ i ].ParticleSystem.main.startColor.color.a );

                var main = _visualEffects [ i ].ParticleSystem.main;

                DOTween.ToAlpha( () => newColor, newColor => main.startColor = newColor,
                    alphaToReach, _elementsFadeSpeed );
            }
        }

        #endregion

        public bool IsApplied => _isApplied;
        public WeatherType GetWeatherType() => _weatherType;
        public EnvironmentLightingSettings GetLightingSettings() => _lightingSettings;

        #region OnValidate

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {

        }
#endif

        #endregion
    }
}