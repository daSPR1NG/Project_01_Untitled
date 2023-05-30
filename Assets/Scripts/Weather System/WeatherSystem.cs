using UnityEngine;
using NaughtyAttributes;
using dnSR_Coding.Utilities;
using DG.Tweening;
using System.Collections.Generic;

namespace dnSR_Coding
{
    [RequireComponent( typeof( EnvironmentLightsContainer ) )]

    ///<summary> WeatherSystem description <summary>
    [DisallowMultipleComponent]
    public class WeatherSystem : MonoBehaviour, IEnvironmentLightsUser, IDebuggable
    {
        [Header( "Weather Presets" )]
        [SerializeField] private List<WeatherPreset> _weatherPresets = new();
        private WeatherPreset _activePreset = null;
        private GameObject _rainGO;
        private Light _thunderLight;

        public EnvironmentLightsContainer EnvironmentLightsContainer { get; set; }
        public Light MainLight { get; set; }
        public Light AdditionalLight { get; set; }       

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() 
        {
            DOTween.Kill( this );
        }

        #endregion

        private void Awake() => Init();
        private void Init()
        {
            SetDependencies();

            ToggleSunDisplay_OnApplyingPreset( false );
            ApplyWeatherPreset( index: 0 );
        }
        private void SetDependencies()
        {
            _rainGO = GameObject.FindGameObjectWithTag( "Environment/Rain" );
            SetLightsReference();
            GetOrSetThunderLight();
        }

        private void Update()
        {
            if ( KeyCode.T.IsPressed() )
            {
                _activePreset.StopThunder( this );
                _activePreset.ApplyThunder( this, _thunderLight );
            }
        }

        /// <summary>
        /// Activates a weather preset, chosen by index.
        /// </summary>
        /// <param name="index"></param>
        public void ApplyWeatherPreset( int index )
        {
            // If the index is higher than the chosen index we throw an error and kill the method...
            if ( index > _weatherPresets.Count )
            {
                this.Debugger( "The index you choose is higher than the preset amount, it's impossible to activate it", DebugType.Error );
                return;
            }

            // If no active preset is set, we set it...
            if ( _activePreset.IsNull() ) 
            {
                _activePreset = _weatherPresets [ index ];
                _activePreset.Init();

                this.Log( "Assign active preset" );
            }

            bool aPresetIsActive = !_activePreset.IsNull() && _activePreset.IsActive;

            // If a preset is already active and we still trying to aply it again, we kill the method...
            if ( aPresetIsActive && _activePreset == _weatherPresets [ index ] ) 
            {
                this.Debugger( "Killing the method a preset is active and is the same as the one already passed" );
                return; 
            }

            // If a preset is active but a new one is applied (different from the current one), we stop the current preset to properly activate the new one...
            if ( aPresetIsActive && _activePreset != _weatherPresets [ index ] )
            {
                _activePreset.Stop( monoBehaviour: this, ref _rainGO, MainLight );
                // Active preset is reassigned and we apply the preset we want.
                _activePreset = _weatherPresets [ index ];
                _activePreset.Init();
            }            

            _activePreset.Apply( monoBehaviour: this, ref _rainGO, MainLight, _thunderLight );
            EventManager.OnApplyingWeatherPreset?.Invoke( _activePreset );

            Debug.Log( "Invocation List " + EventManager.OnApplyingWeatherPreset.GetInvocationList().Length );

            ToggleSunDisplay_OnApplyingPreset( _activePreset.IsSunHidden );
            this.Debugger( $"Assign active preset {_activePreset.name}" );
        }
        public void StopActiveWeatherPreset()
        {
            if ( _activePreset.IsNull() || !_activePreset.IsActive ) { return; }

            _activePreset.Stop( monoBehaviour: this, ref _rainGO, MainLight );
            _activePreset = null;
        }

        private void ToggleSunDisplay_OnApplyingPreset( bool sunIsHidden )
        {
            AdditionalLight.gameObject.SetActive( sunIsHidden );
        }

        #region References grab group

        public void SetLightsReference()
        {
            EnvironmentLightsContainer = GetComponent<EnvironmentLightsContainer>();

            MainLight = EnvironmentLightsContainer.MainLight;
            AdditionalLight = EnvironmentLightsContainer.AdditionalLight;
        }

        private void GetOrSetThunderLight()
        {
            bool isThereALightInChild = false;

            // Cycle through children to find a Light component...
            foreach ( Transform trs in transform )
            {
                // If there is one we notify it and assign it...
                if ( trs.TryGetComponent( out Light light ) )
                {
                    isThereALightInChild = true;
                    _thunderLight = light;
                }
            }

            // If there is no child, or no children with a light component -> create and add one...
            if ( transform.HasNoChild() || !isThereALightInChild )
            {
                GameObject thunderLightGO = new( "Thunder Light" );
                thunderLightGO.transform.SetParent( transform );

                _thunderLight = transform.GetFirstChild().gameObject.AddComponent<Light>();
                _thunderLight.transform.eulerAngles = new Vector3( 90, 0, 0 );

                _thunderLight.type = LightType.Directional;
                _thunderLight.lightmapBakeType = LightmapBakeType.Mixed;
                _thunderLight.color = Color.white;
                _thunderLight.intensity = 0;
                _thunderLight.renderMode = LightRenderMode.ForceVertex;

                int defaultLM = 1 << 0;
                int ignoreRaycastLM = 1 << 2;
                int groundLM = 1 << 6;

                _thunderLight.cullingMask = defaultLM | ignoreRaycastLM | groundLM;
            }
        }

        #endregion

        #region On Editor

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetLightsReference();
            GetOrSetThunderLight();
        }        
#endif

        #endregion
    }
}