using UnityEngine;
using dnSR_Coding.Utilities;
using NaughtyAttributes;

namespace dnSR_Coding
{
    // Required components or public findable enum here.

    // Uncomment this block right below if you need to use add component menu for this component.
    // [AddComponentMenu( menuName:"Custom Scripts/EnvironmentLightsContainer" )]

    ///<summary> EnvironmentLightManager description <summary>
    [DisallowMultipleComponent]
    public class EnvironmentLightsContainer : MonoBehaviour, IDebuggable
    {
        [Header( "Dependencies" )]
        [SerializeField] private GameObject _lightsContainer;

        private Light _mainLight;
        private Light _additionalLight;

        public Light MainLight { get => _mainLight; private set => _mainLight = value; }
        public Light AdditionalLight { get => _additionalLight; private set => _additionalLight = value; }

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        #region Enable, Disable

        void OnEnable() { }

        void OnDisable() { }

        #endregion

        #region Setup

        void Awake() => Init();
        void Init()
        {
            SetLightsReference( ref _lightsContainer );
        }

        /// <summary>
        ///     This method tries to reference the main light and the additional light used for simulating a sun influence.
        /// </summary>
        /// <param name="lightsContainer"></param>
        private void SetLightsReference( ref GameObject lightsContainer )
        {
            if ( lightsContainer.IsNull() )
            {
                lightsContainer.LogNullException();
                return;
            }

            if ( !_mainLight.IsNull() && !_additionalLight.IsNull() ) { return; }

            Transform lightContainerTrs = lightsContainer.transform;

            if ( _mainLight.IsNull()
                && lightContainerTrs.HasChild()
                && lightContainerTrs.GetFirstChild().TryGetComponent( out Light mainLight ) )
            {
                _mainLight = mainLight;
            }
            else
            {
                Debug.Log( "No main light was found so a new one has been created." );

                //Object creation and component assignment
                GameObject mainLightGO = new( "Main Light", typeof( Light ) );
                mainLightGO.transform.SetParent( _lightsContainer.transform );
                mainLightGO.tag = "Environment/DirectionalLight";
                _mainLight = mainLightGO.GetComponent<Light>();

                // Light type and color
                _mainLight.type = LightType.Directional;
                _mainLight.lightmapBakeType = LightmapBakeType.Realtime;
                _mainLight.color = Color.white;
                _mainLight.intensity = 1;

                // Shadows
                _mainLight.shadows = LightShadows.Soft;
                _mainLight.shadowNearPlane = .1f;
                _mainLight.shadowStrength = .85f;
            }

            if ( _additionalLight.IsNull()
                && lightContainerTrs.HasChild()
                && lightContainerTrs.childCount >= 2
                && lightContainerTrs.GetChild( 1 ).TryGetComponent( out Light additionalLight ) )
            {
                _additionalLight = additionalLight;
            }
            else
            {
                Debug.Log( "No additional light was found so a new one has been created." );

                //Object creation and component assignment
                GameObject additionalLightGO = new( "Additional Light", typeof( Light ) );
                additionalLightGO.transform.SetParent( _lightsContainer.transform );
                _additionalLight = additionalLightGO.GetComponent<Light>();

                // Light type and color
                _additionalLight.type = LightType.Directional;
                _additionalLight.lightmapBakeType = LightmapBakeType.Mixed;
                _additionalLight.color = Color.white;
                if ( ColorUtility.TryParseHtmlString( "#FFCB00", out Color color ) ) { _additionalLight.color = color; }
                _additionalLight.intensity = 1;
            }
        }

        #endregion

        #region On Editor

#if UNITY_EDITOR
        private void OnValidate()
        {
            SetLightsReference( ref _lightsContainer );
        }
#endif

        #endregion
    }
}