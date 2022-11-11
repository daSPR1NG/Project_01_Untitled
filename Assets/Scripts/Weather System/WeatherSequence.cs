using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;

namespace dnSR_Coding
{
    // Required components or public findable enum here.

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/WeatherSequence" )]

    ///<summary> WeatherSequence description <summary>
    [Component("WeatherSequence", "")]
    [DisallowMultipleComponent]
    public abstract class WeatherSequence : MonoBehaviour, IDebuggable
    {
        [Title( "SETTINGS", 12, "white" )]

        [SerializeField] protected bool _isApplied = false;
        [SerializeField] protected WeatherType _weatherType = WeatherType.None;
        [SerializeField] private float _sunShaftFadeSpeed = 0.5f;
        [SerializeField] private float _lightFadeSpeed = 0.5f;
        [SerializeField] private float _vfxFadeSpeed = 0.5f;

        [Space( 10f )]

        [Title( "ELEMENTS", 12, "white" )]

        [SerializeField] protected List<Material> _sunShaftMaterials = new ();
        [SerializeField] protected List<SequenceLightData> _lights = new ();
        [SerializeField] protected List<SequenceParticleSystemData> _visualEffects = new ();

        private Coroutine _sunShaftCoroutine = null;
        private Coroutine _lightsCoroutine = null;
        private Coroutine _visualEffectCoroutine = null;

        [System.Serializable]
        public class SequenceLightData
        {
            public Light Light;
            public float Intensity;
        }

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

        public abstract void ApplyWeatherSequence();
        public abstract void RemoveWeatherSequence();

        #region Set Weather elements handle

        protected virtual void SetSunshaftsAlphaValue( float alphaToReach )
        {
            if ( _sunShaftMaterials.IsEmpty() ) { return; }

            if ( !_sunShaftCoroutine.IsNull() ) { StopCoroutine( _sunShaftCoroutine ); }

            for ( int i = 0; i < _sunShaftMaterials.Count; i++ )
            {
                _sunShaftCoroutine =
                    StartCoroutine( FadeMaterialColorAlphaTo( _sunShaftMaterials [ i ], alphaToReach, _sunShaftFadeSpeed ) );
            }
        }

        protected virtual void SetLightsAlphaValue( bool fadesOut )
        {
            if ( _lights.IsEmpty() ) { return; }

            if ( !_lightsCoroutine.IsNull() ) { StopCoroutine( _lightsCoroutine ); }

            for ( int i = 0; i < _lights.Count; i++ )
            {
                float intensity = fadesOut ? 0.1f : _lights [ i ].Intensity;

                _lightsCoroutine = StartCoroutine(
                    FadeLightIntensityTo( _lights [ i ].Light, intensity, _lightFadeSpeed ) );
            }
        }

        protected virtual void SetVisualEffectsAlphaValue( bool fadesOut )
        {
            if ( _visualEffects.IsEmpty() ) { return; }

            if ( !_visualEffectCoroutine.IsNull() ) { StopCoroutine( _visualEffectCoroutine ); }

            for ( int i = 0; i < _visualEffects.Count; i++ )
            {
                float alpha = fadesOut ? 0 : _visualEffects [ i ].Alpha;

                _visualEffectCoroutine = StartCoroutine(
                    FadeParticleEffectStartingColorAlphaTo(
                        _visualEffects [ i ].ParticleSystem, alpha, _vfxFadeSpeed ) );
            }
        }

        #endregion

        #region Utils - Coroutines

        protected IEnumerator FadeLightIntensityTo( Light light, float toValue, float delay )
        {
            float initialIntensity = light.intensity;

            for ( float t = 0.0f; t < 1.0f; t += Time.deltaTime / delay )
            {
                light.intensity = Mathf.Lerp( initialIntensity, toValue, t );
                yield return null;
            }

            yield break;
        }

        protected IEnumerator FadeMaterialColorAlphaTo( Material mat, float toValue, float delay )
        {
            float alpha = mat.color.a;

            for ( float t = 0.0f; t < 1.0f; t += Time.deltaTime / delay )
            {
                Color newColor = new( 1, 1, 1, Mathf.Lerp( alpha, toValue, t ) );
                mat.color = newColor;

                yield return null;
            }

            yield break;
        }

        protected IEnumerator FadeParticleEffectStartingColorAlphaTo( ParticleSystem pS, float toValue, float delay )
        {
            float alpha = pS.main.startColor.color.a;

            for ( float t = 0.0f; t < 1.0f; t += Time.deltaTime / delay )
            {
                Color newColor = new( 1, 1, 1, Mathf.Lerp( alpha, toValue, t ) );

                var main = pS.main;
                main.startColor = newColor;

                yield return null;
            }

            yield break;
        }

        #endregion

        public bool IsApplied => _isApplied;
        public WeatherType GetWeatherType() => _weatherType;

        #region OnValidate

#if UNITY_EDITOR

        protected virtual void OnValidate()
        {

        }
#endif

        #endregion
    }
}