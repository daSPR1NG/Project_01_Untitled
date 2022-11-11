using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System.Threading.Tasks;
using System;

namespace dnSR_Coding
{
    // Required components or public findable enum here.

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/RainSequence" )]

    ///<summary> RainSequence description <summary>
    [Component("RainSequence", "")]
    [DisallowMultipleComponent]
    public class RainSequence : WeatherSequence
    {
        public static Action<WeatherSequence> OnApplyingRainySequence;
        public static Action<WeatherSequence> OnRemovingRainySequence;

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        [ContextMenu( "Apply Weather Sequence")]
        public override void ApplyWeatherSequence()
        {
            // Smoothly turn sunshaft alpha from O to max value
            SetSunshaftsAlphaValue( 1f );

            // Smoothly turn light intensity from 0 to max value
            SetLightsAlphaValue( false );

            // Smoothly turn VFX start color alpha from 0 to max value
            SetVisualEffectsAlphaValue( false );

            _isApplied = true;
            OnApplyingRainySequence?.Invoke( this );
        }

        [ContextMenu( "Remove Weather Sequence" )]
        public override void RemoveWeatherSequence()
        {
            // Smoothly turn sunshaft alpha from max value to 0
            SetSunshaftsAlphaValue( 0f );

            // Smoothly turn light intensity from max value to 0
            SetLightsAlphaValue( true );

            // Smoothly turn VFX start color alpha from max value to 0
            SetVisualEffectsAlphaValue( true );

            _isApplied = false;
            OnRemovingRainySequence?.Invoke( this );
        }
    }
}