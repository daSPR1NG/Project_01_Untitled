using UnityEngine;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
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
        [Header( "SETTINGS" )]
        [SerializeField] private bool _isActive = false;
        [SerializeField] private WeatherType _weatherType = WeatherType.None;
        [SerializeField, Range( 0f, 15f )] private float _elementsFadeSpeed = 1f;
        [SerializeField, Expandable] private EnvironmentLightingSettings _lightingSettings;

        [Header( "ELEMENTS" )]
        [SerializeField] private List<Material> _sunShaftMaterials = new ();
        [SerializeField] private List<SequenceParticleSystemData> _visualEffects = new ();

        private Transform _elementsTrs;

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

        void OnEnable() 
        {
        }

        void OnDisable() 
        {
        }

        #endregion

        void Awake() => Init();
        void Init()
        {
            GetLinkedComponents();
        }
        void GetLinkedComponents()
        {
            if ( _elementsTrs.IsNull() ) { _elementsTrs = transform.GetFirstChild(); }
        }

        /// <summary>
        /// Applies this weather sequence, applying correct settings relative to current daytime.
        /// </summary>
        /// <param name="isDaytime"> Current state of daytime, would be false if its currently night time. </param>
        public void ApplySequence( bool isDaytime )
        {
            // Meaning it is already applied.
            if ( _isActive ) { return; }

            Helper.Log( this, "Apply " + transform.name + " Sequence" );

            // Smoothly turn sunshaft alpha from O to max value
            DisplaySunShafts( isDaytime );

            // Smoothly turn VFX start color alpha from 0 to max value
            SetVisualEffectsAlphaValue( false );

            _isActive = true;
        }

        /// <summary>
        /// Removes this weather sequence, applying correct settings relative to current daytime.
        /// </summary>
        public void RemoveSequence()
        {
            Helper.Log( this, "Remove " + transform.name + " Sequence" );

            // Smoothly turn sunshaft alpha from O to max value
            HideSunShafts();

            // Smoothly turn VFX start color alpha from 0 to max value
            SetVisualEffectsAlphaValue( true );

            _isActive = false;
        }

        #region Set Weather elements handle

        /// <summary>
        /// Displays all the sun shaft elements used by this weather sequence.
        /// </summary>
        /// <param name="isDaytime"> Current state of daytime, would be false if its currently night time. </param>
        public void DisplaySunShafts( bool isDaytime )
        {
            float alpha = isDaytime ? 1.0f : 0.0f;
            SetSunShaftsAlphaValue( alpha );
        }

        /// <summary>
        /// Hides all the sun shaft elements used by this weather sequence.
        /// </summary>
        /// <param name="isDaytime"> 
        /// Current state of daytime, would be false if its currently night time.
        /// The default value is false because of the System.Action requiring a daytime state.
        /// </param>
        public void HideSunShafts( bool isDaytime = false ) => SetSunShaftsAlphaValue( 0 );

        /// <summary>
        /// Sets all sun shaft elements alpha value.
        /// </summary>
        /// <param name="alphaToReach"> This is the new aplha value applied to all sun shaft elements. </param>
        private void SetSunShaftsAlphaValue( float alphaToReach )
        {
            if ( _sunShaftMaterials.IsEmpty() ) { return; }

            for ( int i = 0; i < _sunShaftMaterials.Count; i++ )
            {
                //Helper.Log( this, "Accesing Sun shafts material alpha" + transform.name );
                if ( _sunShaftMaterials [ i ].color.a == alphaToReach ) { continue; }

                Helper.Log( this, "Fading Sun shafts material alpha" + transform.name );

                if ( !Application.isPlaying )
                {
                    Color newColor = new( 1, 1, 1 )
                    {
                        a = alphaToReach
                    };

                    _sunShaftMaterials [ i ].color = newColor;
                }
                else { _sunShaftMaterials [ i ].DOFade( alphaToReach, _elementsFadeSpeed ); }
            }
        }

        /// <summary>
        /// Sets the alpha value of all VFX used by this weather sequence.
        /// </summary>
        /// <param name="needsToBeHidden"> 
        /// This state defines how this method would adapt -> [ alpha 0.0 - true / alpha 1.0 - false ]
        /// </param>
        private void SetVisualEffectsAlphaValue( bool needsToBeHidden )
        {
            if ( _visualEffects.IsEmpty() ) { return; }

            for ( int i = 0; i < _visualEffects.Count; i++ )
            {
                float alphaToReach = needsToBeHidden ? 0 : _visualEffects [ i ].Alpha;

                if ( _visualEffects [ i ].ParticleSystem.main.startColor.color.a == alphaToReach ) { continue; }

                Color newColor = new( 1, 1, 1, _visualEffects [ i ].ParticleSystem.main.startColor.color.a );

                var main = _visualEffects [ i ].ParticleSystem.main;

                if ( !Application.isPlaying )
                {
                    newColor = new( 1, 1, 1, alphaToReach );
                    main.startColor = newColor;
                } 
                else 
                {
                    DOTween.ToAlpha( () => newColor, newColor => main.startColor = newColor,
                    alphaToReach, _elementsFadeSpeed ); 
                }
            }
        }

        #endregion

        public bool IsActive => _isActive;
        public WeatherType GetWeatherType() => _weatherType;
        public EnvironmentLightingSettings GetLightingSettings() => _lightingSettings;

        #region OnValidate

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {
            GetLinkedComponents();
        }

#endif

        #endregion
    }
}