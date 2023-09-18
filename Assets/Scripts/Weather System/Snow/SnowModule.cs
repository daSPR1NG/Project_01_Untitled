using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using dnSR_Coding.Utilities.Helpers;
using dnSR_Coding.Utilities.Attributes;
using dnSR_Coding.Utilities.Interfaces;
using dnSR_Coding.Utilities.Runtime;
using Sirenix.OdinInspector;
using System.Linq;

namespace dnSR_Coding
{
    // NOTES : 
    // Flickering Rate => Range entre deux valeurs -> random choisi
    // Curve => tableau de différentes intensités


    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Modules/Create New Snow Module Settings" )]
    public class SnowModule : WeatherSystemModule<SnowModule.SnowSettings>
    {
        // Find all the snow shader users and store them into an array - DONE
        // This operation should be done again when applying the module
        // - The solution found was to register all weather entities when they're created
        // When applied we need to know some information about how long the snow will last -> snowCycleTotalDuration
        // So we can tween the application of the snow to correspond to this duration
        // The duration would be cut in half like a day cycle

        private MonoBehaviour _monoBehaviour = null;
        private SnowVisibilityTweener _snowTweener = null;

        #region Snow settings struct

        [Serializable]
        public struct SnowSettings
        {
            public readonly int ID
            {
                get
                {
                    return ( int ) _associatedSnowType;
                }
            }

            [field: CenteredHeader( "Main settings" )]
            [SerializeField] private Enums.Snow_Type _associatedSnowType;
        }
        public SnowSettings GetSettingsByID( int id )
        {
            return Settings [ id ];
        }

        #endregion

        private readonly List<WeatherEntity> _weatherEntities = new List<WeatherEntity>();

        public SnowModule()
        {

        }

        #region Init

        #region ENABLE, DISABLE

        void OnEnable()
        {
            EventManager.WeatherEntity_OnRegistration.Subscribe( AddWeatherEntity );
            EventManager.WeatherEntity_OnUnregistration.Subscribe( RemoveWeatherEntity );
        }

        void OnDisable()
        {
            EventManager.WeatherEntity_OnRegistration.Unsubscribe( AddWeatherEntity );
            EventManager.WeatherEntity_OnUnregistration.Unsubscribe( RemoveWeatherEntity );
        }

        #endregion


        public void Init( MonoBehaviour monoBehaviour, float snowCycleTotalDuration )
        {
            _monoBehaviour = monoBehaviour;
            _snowTweener = new SnowVisibilityTweener( snowCycleTotalDuration, GetSnowShaderFromWeatherEntities() );
        }

        #endregion

        #region Apply / Stop

        public void Apply( Enums.Snow_Type snowType )
        {
            SnowSettings settings = GetSettingsByID( ( int ) snowType );
            if ( settings.IsNull<SnowSettings>() )
            {
                this.Debugger(
                    "Snow Module - Apply - Snow settings reference is null",
                    DebugType.Error );
                return;
            }

            // Here we would start a tween following the cycle duration
            // It would act as a day cycle duration
            _snowTweener.TweenSnowVisibility( _monoBehaviour );
            this.Debugger( "Snow settings have been applied !" );
        }

        public void Stop()
        {

        }

        #endregion

        #region Utils

        private void AddWeatherEntity( WeatherEntity weatherEntity )
        {
            _weatherEntities.AppendItem( weatherEntity );

            this.Debugger( _weatherEntities.Count );

            // Check if this weather entity can be affected by the module ?
            if ( !weatherEntity.CanBeAffectedBySnow() )
            {
                Debug.Log( "CANT BE AFFECTED BY SNOW !", this );
                return;
            }

            Debug.Log( "CAN BE AFFECTED BY SNOW !", this );

            // Set current snow settings to this entity
        }
        private void RemoveWeatherEntity( WeatherEntity weatherEntity )
        {
            // Remove snow settings from this entity
            _weatherEntities.RemoveItem( weatherEntity );
        }

        private Material [] GetSnowShaderFromWeatherEntities()
        {
            List<Material> snowShaders = new();

            foreach ( WeatherEntity weatherEntity in _weatherEntities )
            {
                if ( weatherEntity.TryGetComponent( out MeshRenderer meshRenderer ) )
                {
                    snowShaders.Add( meshRenderer.sharedMaterial );
                }
            }

            return snowShaders.ToArray();
        }

        #endregion
    }
}