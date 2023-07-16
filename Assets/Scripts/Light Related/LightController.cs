using UnityEngine;
using System.Collections;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    [RequireComponent( typeof( Light ) )]

    ///<summary> LightController description <summary>
    [DisallowMultipleComponent]
    public class LightController : MonoBehaviour, IDebuggable
    {
        [SerializeField] private Enums.LightType _lightType = Enums.LightType.None;
        private Light _controllerLight;

        public LightController( Enums.LightType lightType, Light light )
        {
            _lightType = lightType;
            _controllerLight = light;
        }

        #region DEBUG

        //[Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region SETUP

        void Awake() => SetControllerLight( GetComponent<Light>() );
        
        // Set all datas that need it at the start of the game
        public void Init( Light light, Enums.LightType lightType )
        {
            SetControllerLight( light );
            SetLightType( lightType );
        }

        #endregion

        #region SETTING LIGHT PARAMETERS | DIFFERENT CONSTRUCTORS

        public void SetLightParameters( LightType type, LightmapBakeType bakeType, Color color, float intensity, LightShadows shadows, float shadowNearPlane, float shadowStrength )
        {
            if ( _controllerLight.IsNull<LightController>() ) { return; }

            // Light type and color
            SetLightType( type );
            SetLightmapBakeType( bakeType );
            SetLightColor( color );
            SetLightIntensity( intensity );

            // Shadows
            SetLightShadows( shadows );
            SetShadowNearPlane( shadowNearPlane );
            SetShadowStrength( shadowStrength );
        }

        public void SetLightParameters( LightType type, LightmapBakeType bakeType, LightRenderMode renderMode, Color color, float intensity, int cullingMask )
        {
            if ( _controllerLight.IsNull<LightController>() ) { return; }

            SetLightType( type );
            SetLightmapBakeType( bakeType );
            SetLightmapRenderMode( renderMode );
            SetLightColor( color );
            SetLightIntensity( intensity );
            SetLightCullingMask( cullingMask );
        }

        public void SetLightParameters( LightType type, LightmapBakeType bakeType, Color color, float intensity )
        {
            if ( _controllerLight.IsNull<LightController>() ) { return; }

            // Light type and color
            SetLightType( type );
            SetLightmapBakeType( bakeType );
            SetLightColor( color );
            SetLightIntensity( intensity );
        }

        #endregion

        #region LIGHT

        private void SetLightType( LightType type ) {
            _controllerLight.type = type; 
        }

        private void SetLightmapBakeType( LightmapBakeType bakeType ) {
            _controllerLight.lightmapBakeType = bakeType; 
        }

        private void SetLightmapRenderMode( LightRenderMode renderMode ) {
            _controllerLight.renderMode = renderMode; 
        }

        public void SetLightIntensity( float intensity ) {
            _controllerLight.intensity = intensity; 
        }

        public void SetLightColor( Color color ) {
            _controllerLight.color = color; 
        }

        public bool DoesLightIntensityEquals( float value ) {
            return _controllerLight.intensity == value; 
        }

        #endregion

        #region SHADOW

        private void SetLightShadows( LightShadows shadows ) {
            _controllerLight.shadows = shadows; 
        }

        private void SetShadowNearPlane( float shadowNearPlane ) {
            _controllerLight.shadowNearPlane = shadowNearPlane; 
        }

        private void SetShadowStrength ( float shadowStrength ) {
            _controllerLight.shadowStrength = shadowStrength; 
        }

        #endregion

        public void SetLightCullingMask( int cullingMask ){
            _controllerLight.cullingMask = cullingMask;
        }

        #region SET ENABLE/DISABLE

        public void ToggleEnabledLightState( bool needToEnable )
        {
            if ( needToEnable )
            {
                EnableLight();
                return;
            }

            DisableLight();
        }

        public void EnableLight() 
        {
            if ( _controllerLight.enabled ) { return; }

            _controllerLight.Enable();
            ChangeGameObjectName( "Enabled" );
        }
        public void DisableLight() 
        {
            if ( !_controllerLight.enabled ) { return; }

            SetLightIntensity( 0 );
            _controllerLight.Disable();
            ChangeGameObjectName( "Disabled" );
        }

#if UNITY_EDITOR
        private void ChangeGameObjectName( string value )
        {
            if ( _controllerLight.gameObject.name.Equals( "Additional Light " + value ) ) { return; }
            _controllerLight.gameObject.name = "Additional Light - " + value;
        }
#endif

        #endregion

        #region CONTROLLER LIGHT GET/SET + HAS LIGHT ?

        public Light GetControllerLight() {
            return _controllerLight; 
        }

        public void SetControllerLight( Light light )
        {
            if ( light.IsNull<Light>() ) { return; }

            _controllerLight = light;
        }


        public bool HasLight() {
            return !_controllerLight.IsNull<LightController>(); 
        }

        public Enums.LightType GetLightType() {
            return _lightType;
        }

        public void SetLightType( Enums.LightType lightType ) {
            _lightType = lightType;
        }

        #endregion

        #region EDITOR

#if UNITY_EDITOR
        private void OnValidate() {
            SetControllerLight( GetComponent<Light>() );
        }
#endif

        #endregion
    }
}