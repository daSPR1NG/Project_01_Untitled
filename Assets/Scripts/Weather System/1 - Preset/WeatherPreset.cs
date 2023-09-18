using UnityEngine;
using dnSR_Coding.Utilities.Helpers;
using Sirenix.OdinInspector;
using dnSR_Coding.Utilities.Attributes;

namespace dnSR_Coding
{
    [CreateAssetMenu( menuName = "Scriptable Objects/Weather System/Presets/Create New Weather Preset" )]
    [InlineEditor( InlineEditorObjectFieldModes.Foldout )]
    public class WeatherPreset : ScriptableObject
    {
        public int ID => GetInstanceID();
        public bool IsActive { get; private set; } = false;

        #region Rain module fields

        [CenteredHeader( Header = "Rain details", TextAnchor = TextAnchor.MiddleLeft )]
        [SerializeField, Required] private RainModule _rainModule;
        [SerializeField] private Enums.Rain_Type _rainType;

        #endregion

        #region Thunder module fields

        [CenteredHeader( Header = "Thunder details", TextAnchor = TextAnchor.MiddleLeft )]
        [SerializeField, Required] private ThunderModule _thunderModule;
        [SerializeField] private Enums.Thunder_Type _thunderType;

        #endregion

        #region Fog module fields

        [CenteredHeader( Header = "Fog details" )]
        [SerializeField, Required] private FogModule _fogModule;
        [SerializeField] private Enums.Fog_Type _fogType;

        #endregion

        #region Snow module fields

        [CenteredHeader( Header = "Snow details" )]
        [SerializeField, Required] private SnowModule _snowModule;
        [SerializeField] private Enums.Snow_Type _snowType;

        #endregion

        #region Light module fields

        [CenteredHeader( Header = "Lighting details" )]
        [SerializeField] private EnvironmentLightModule _environmentLightModule;
        [SerializeField] private Enums.Environment_LightIntensity_Type _environmentLightIntensityType;
        [field: SerializeField] public bool IsSunHidden { get; private set; }

        #endregion

        #region Weather modules definition checks

        private bool HasActiveRainModule =>
            !_rainModule.IsNull<RainModule>() && _rainType != Enums.Rain_Type.None;

        private bool HasActiveThunderModule =>
            !_thunderModule.IsNull<ThunderModule>() && _thunderType != Enums.Thunder_Type.None;

        private bool HasActiveFogModule =>
            !_fogModule.IsNull<FogModule>() && _fogType != Enums.Fog_Type.None;

        private bool HasActiveSnowModule =>
            !_snowModule.IsNull<FogModule>() && _snowType != Enums.Snow_Type.None;

        #endregion

        #region Init

        private void Init(
            MonoBehaviour monoBehaviour,
            GameObject rainGO,
            LightController mainLightController,
            LightController thunderLightController )
        {
            IsActive = false;

            _rainModule.Init( rainGO );
            _thunderModule.Init( monoBehaviour, thunderLightController );
            _environmentLightModule.Init( mainLightController );
            _snowModule.Init( monoBehaviour, 120 ); // VALUE AS TEST PURPOSE, WE'LL NEED A PRECISE VALUE AFTER
        }

        #endregion

        #region Apply / Stop

        public void Apply(
            MonoBehaviour monoBehaviour,
            GameObject rainGO,
            LightController mainLightController,
            LightController thunderLightController )
        {
            if ( !AreAllModulesSet() )
            {
                Debug.LogError( "Weather Preset problem, one or more module(s) is/are missing, please check." );
                return;
            }

            Init( monoBehaviour, rainGO, mainLightController, thunderLightController );

            SetLightIntensity( _environmentLightIntensityType );

            Debug.Log( $"Module: {_rainModule} / Type: {_rainType}" );
            Debug.Log( $"RainGO: {rainGO.name}" );

            System.Action rainAction = HasActiveRainModule
                ? () => _rainModule.Apply( _rainType )
                : () => _rainModule.Stop();

            System.Action thunderAction = HasActiveThunderModule
                ? () => _thunderModule.Apply(
                    _thunderType,
                    mainLightController.GetControllerLight().color )
                : () => _thunderModule.Stop();

            System.Action fogAction = HasActiveFogModule
                ? () => _fogModule.Apply( _fogType )
                : () => _fogModule.Stop();

            System.Action snowAction = HasActiveSnowModule
                ? () => _snowModule.Apply( _snowType )
                : () => _snowModule.Stop();

            rainAction?.Invoke();
            thunderAction?.Invoke();
            fogAction?.Invoke();
            snowAction?.Invoke();

            IsActive = true;
        }

        public void Stop()
        {
            if ( HasActiveRainModule ) { _rainModule.Stop(); }
            if ( HasActiveThunderModule ) { _thunderModule.Stop(); }
            if ( HasActiveFogModule ) { _fogModule.Stop(); }

            SetLightIntensity( Enums.Environment_LightIntensity_Type.Max );

            IsActive = false;
        }

        #endregion

        #region Utils

        private bool AreAllModulesSet()
        {
            return !_rainModule.IsNull()
                && !_thunderModule.IsNull()
                && !_fogModule.IsNull()
                && !_snowModule.IsNull()
                && !_environmentLightModule.IsNull();
        }

        #region Main Light Intensity

        private void SetLightIntensity( Enums.Environment_LightIntensity_Type lightIntensity )
        {
            _environmentLightModule.Apply( lightIntensity );
        }

        #endregion

        #region Weather modules info

        public (RainModule module, Enums.Rain_Type type) GetRainInfo()
        {
            return (_rainModule, _rainType);
        }
        public (ThunderModule module, Enums.Thunder_Type type) GetThunderInfo()
        {
            return (_thunderModule, _thunderType);
        }
        public (FogModule module, Enums.Fog_Type type) GetFogInfo()
        {
            return (_fogModule, _fogType);
        }
        public (EnvironmentLightModule module, Enums.Environment_LightIntensity_Type type) GetEnvironmentLightInfo()
        {
            return (_environmentLightModule, _environmentLightIntensityType);
        }

        #endregion

        #endregion
    }
}