using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        [Space( 10f )]

        [Title( "ELEMENTS", 12, "white" )]

        [SerializeField] private List<Material> _sunShaftMaterials = new ();
        [SerializeField] private List<SequenceParticleSystemData> _visualEffects = new ();

        private Coroutine _sunShaftCoroutine = null;
        private Coroutine _visualEffectCoroutine = null;

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

        public void ApplyWeatherSequence()
        {
            // Smoothly turn sunshaft alpha from O to max value
            SetSunshaftsAlphaValue( 1f );

            // Smoothly turn VFX start color alpha from 0 to max value
            SetVisualEffectsAlphaValue( false );

            _isApplied = true;
        }

        public void RemoveWeatherSequence()
        {
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

                _sunShaftCoroutine = StartCoroutine( 
                    FadeSunShaftAlphaTo( _sunShaftMaterials [ i ], alphaToReach, _elementsFadeSpeed ) );
            }
        }

        private void SetVisualEffectsAlphaValue( bool fadesOut )
        {
            if ( _visualEffects.IsEmpty() ) { return; }

            if ( !_visualEffectCoroutine.IsNull() ) { StopCoroutine( _visualEffectCoroutine ); }

            for ( int i = 0; i < _visualEffects.Count; i++ )
            {
                float alphaToReach = fadesOut ? 0 : _visualEffects [ i ].Alpha;

                if ( _visualEffects [ i ].ParticleSystem.main.startColor.color.a == alphaToReach ) { continue; }

                _visualEffectCoroutine = StartCoroutine(
                    FadeVisualEffectAlphaTo(
                        _visualEffects [ i ].ParticleSystem, alphaToReach, _elementsFadeSpeed / 2 ) );
            }
        }

        #endregion

        #region Utils - Coroutines

        private IEnumerator FadeSunShaftAlphaTo( Material mat, float toValue, float delay )
        {
            float fromValue = mat.color.a;

            // We initialize t as 0, then we increment it by deltaTime / delay to match the delay passed in argument.
            // We execute the for loop until t is greater than 1.0f matching the max amplitude of a Color alpha 0-1.
            for ( float t = 0.0f; t < 1.0f; t += Time.deltaTime / delay )
            {
                Color newColor = new( 1, 1, 1, Mathf.Lerp( fromValue, toValue, t ) );
                mat.color = newColor;

                yield return null;
            }

            Debug.Log( "FadeSunShaftAlphaTo COMPLETED" );
            yield break;
        }

        private IEnumerator FadeVisualEffectAlphaTo( ParticleSystem pS, float toValue, float delay )
        {
            float alpha = pS.main.startColor.color.a;

            // We initialize t as 0, then we increment it by deltaTime / delay to match the delay passed in argument.
            // We execute the for loop until t is greater than 1.0f matching the max amplitude of a Color alpha 0-1.
            for ( float t = 0.0f; t < 1.0f; t += Time.deltaTime / delay )
            {
                Color newColor = new( 1, 1, 1, Mathf.Lerp( alpha, toValue, t ) );

                var main = pS.main;
                main.startColor = newColor;

                yield return null;
            }

            Debug.Log( "FadeSunShaftAlphaTo COMPLETED" );
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