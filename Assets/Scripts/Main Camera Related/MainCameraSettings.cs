using UnityEngine;
using dnSR_Coding.Utilities;
using System.Collections.Generic;
using Cinemachine;
using ExternalPropertyAttributes;

namespace dnSR_Coding
{
    ///<summary> MainCameraSettings component is used to facilitate the modification of the main camera settings <summary>
    [Component("MainCameraSettings", "")]
    [DisallowMultipleComponent]
    public class MainCameraSettings : MonoBehaviour, IDebuggable
    {
        private enum CameraProjection { Perspective, Orthographic }

        [Title( "GENERAL SETTINGS", 12, "white" )]
        [SerializeField] private CameraProjection _projection;

        [Title( "Perspective VIEW SETTINGS", 12, "white" )]
        [SerializeField, ShowIf( "IsPerspective" )] private float _verticalFov = 60;


        [Title( "Perspective VIEW SETTINGS", 12, "white" )]
        [SerializeField, ShowIf( "IsOrthographic" )] private float _orthographicSize = 12.5f;

        [Title( "Common VIEW SETTINGS", 12, "white" )]

        [SerializeField] private float _nearClipPlane = 0.1f;
        [SerializeField] private float _farClipPlane = 100f;
        [SerializeField, Range( -180f, 180f)] private float _dutch = 0f;

        private readonly List<CinemachineVirtualCamera> _virtualCameras = new();
        private Camera _cameraComponent;

        [Space( 10f )]

        [Title( "Rendering settings", 12, "white" )]

        [Range( 1, 1000 ), SerializeField] private int _pixelSize = 150;

        private UnityEngine.Rendering.Universal.ScriptableRendererData ScriptableRendererData;
        private FullScreenRenderPassFeature PixelizeFeature;

        #region Debug

        [Space( 10 ), HorizontalLine( .5f, EColor.Gray )]
        [SerializeField] private bool _isDebuggable = true;
        public bool IsDebuggable => _isDebuggable;

        #endregion

        void Awake() => Init();
        void Init()
        {
            GetLinkedComponents();
            SetPixelizerPixelSize( _pixelSize );
        }
        private void GetLinkedComponents()
        {
            if ( _cameraComponent.IsNull() ) { _cameraComponent = GetComponent<Camera>(); }
        }

        private void GetPixelizeFeature()
        {
            if ( ScriptableRendererData.IsNull() )
            {
                UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipeline =
                ( UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset ) UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;

                System.Reflection.FieldInfo fieldInfo =
                    pipeline.GetType().GetField( "m_RendererDataList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );

                ScriptableRendererData = ( ( UnityEngine.Rendering.Universal.ScriptableRendererData [] ) fieldInfo?.GetValue( pipeline ) )? [ 0 ];
            }
        }
        private void SetPixelizerPixelSize( int pixelSize )
        {
            GetPixelizeFeature();

            for ( int i = 0; i < ScriptableRendererData.rendererFeatures.Count; i++ )
            {
                if ( ScriptableRendererData.rendererFeatures [ i ] is FullScreenRenderPassFeature feature )
                {
                    PixelizeFeature = feature;
                    PixelizeFeature.SetPixelSize( pixelSize );
                }
            }
        }

        public bool IsOrthographic => _projection == CameraProjection.Orthographic;
        public bool IsPerspective => _projection == CameraProjection.Perspective;

        #region OnValidate

#if UNITY_EDITOR

        void TryToFindAnyVirtualCameraInScene()
        {
            var cvc = FindObjectOfType<CinemachineVirtualCamera>();

            _virtualCameras.AppendItem( cvc );
        }
        void LinkSettingsToAnyActiveVirtualCameras()
        {
            if ( _virtualCameras.IsEmpty() ) { return; }

            for ( int i = 0; i < _virtualCameras.Count; i++ )
            {
                switch ( _projection )
                {
                    case CameraProjection.Perspective:

                        _virtualCameras [ i ].m_Lens.FieldOfView = _verticalFov;
                        break;
                    case CameraProjection.Orthographic:

                        _virtualCameras [ i ].m_Lens.OrthographicSize = _orthographicSize;
                        break;
                }

                _virtualCameras [ i ].m_Lens.NearClipPlane = _nearClipPlane;
                _virtualCameras [ i ].m_Lens.FarClipPlane = _farClipPlane;
                _virtualCameras [ i ].m_Lens.Dutch = _dutch;

                Debug.Log( "Virtual cameras lens settings set." );
            }
        }

        void ApplyProjection()
        {
            if ( !_cameraComponent.IsNull() )
            {
                switch ( _projection )
                {
                    case CameraProjection.Perspective:
                        _cameraComponent.orthographic = false;
                        break;
                    case CameraProjection.Orthographic:
                        _cameraComponent.orthographic = true;
                        break;
                }
            }
        }

        private void OnValidate()
        {
            GetLinkedComponents();
            ApplyProjection();

            TryToFindAnyVirtualCameraInScene();
            LinkSettingsToAnyActiveVirtualCameras();

            SetPixelizerPixelSize( _pixelSize );
        }
#endif

        #endregion
    }
}