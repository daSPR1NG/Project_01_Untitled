using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using ExternalPropertyAttributes;
using System;

namespace dnSR_Coding
{
    // Required components or public findable enum here.

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/SunnySequence" )]

    ///<summary> SunnySequence description <summary>
    [Component("SunnySequence", "")]
    [DisallowMultipleComponent]
    public class SunnySequence : WeatherSequence, IDebuggable
    {
        public static Action<WeatherSequence> OnApplyingSunnySequence;
        public static Action<WeatherSequence> OnRemovingSunnySequence;

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        [ContextMenu( "Apply Weather Sequence" )]
        public override void ApplyWeatherSequence()
        {
            // Smoothly turn sunshaft alpha from O to max value
            SetSunshaftsAlphaValue( 1f );

            // Smoothly turn light intensity from 0 to max value
            SetLightsAlphaValue( false );

            // Smoothly turn VFX start color alpha from 0 to max value
            SetVisualEffectsAlphaValue( false );

            _isApplied = true;
            OnApplyingSunnySequence?.Invoke( this );
        }

        [ContextMenu( "Remove Weather Sequence" )]
        public override void RemoveWeatherSequence()
        {
            // Smoothly turn sunshaft alpha from O to max value
            SetSunshaftsAlphaValue( 0f );

            // Smoothly turn light intensity from 0 to max value
            SetLightsAlphaValue( true );

            // Smoothly turn VFX start color alpha from 0 to max value
            SetVisualEffectsAlphaValue( true );

            _isApplied = false;
            OnRemovingSunnySequence?.Invoke( this );
        }
    }
}