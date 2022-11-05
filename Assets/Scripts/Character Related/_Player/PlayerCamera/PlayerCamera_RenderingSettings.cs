//using UnityEngine;
//using dnSR_Coding.Utilities;
//using NaughtyAttributes;
//using Cinemachine;

//namespace dnSR_Coding
//{
//    ///<summary> PlayerCamera_RenderingSettings description <summary>
//    [Component( "Projection Settings", "Handle the rendering used by the camera, in this case, the precision of the pixelization feature." )]
//    public class PlayerCamera_RenderingSettings : PlayerCamera_DefaultSettings, IValidatable
//    {
//        [Space( 5 )]

//        [Title( "GENERAL SETTINGS", 12, "white" )]

//        [Range( 1, 650 ), SerializeField] private int _pixelSize = 150;

//        private int _zoomPixelSubstraction;
//        private int _maxZoomedPixelSize;
//        private int _minZoomedPixelSize;

//        private UnityEngine.Rendering.Universal.ScriptableRendererData ScriptableRendererData;
//        private PixelizeFeature PixelizeFeature;

//        private CinemachineVirtualCamera _virtualCamera;

//        public bool IsValid { get; private set; }

//        #region Enable, Disable

//        void OnEnable() 
//        {
//            PlayerCamera_Zoom.OnCameraZoom += AdjustPixelSizeOnZoom;
//        }

//        void OnDisable() 
//        {
//            PlayerCamera_Zoom.OnCameraZoom -= AdjustPixelSizeOnZoom;
//        }

//        #endregion            

//        void Awake() => Init();
//        void Init()
//        {
//            SetPixelAdjustmentVariables();
//            SetPixelizerPixelSize( _pixelSize );
//        }

//        private void GetPixelizeFeature()
//        {
//            if ( ScriptableRendererData.IsNull() )
//            {
//                UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset pipeline =
//                ( UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset ) UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;

//                System.Reflection.FieldInfo fieldInfo =
//                    pipeline.GetType().GetField( "m_RendererDataList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic );

//                ScriptableRendererData = ( ( UnityEngine.Rendering.Universal.ScriptableRendererData [] ) fieldInfo?.GetValue( pipeline ) )? [ 0 ];
//            }
//        }

//        private void SetPixelizerPixelSize( int pixelSize )
//        {
//            GetPixelizeFeature();

//            for ( int i = 0; i < ScriptableRendererData.rendererFeatures.Count; i++ )
//            {
//                if ( ScriptableRendererData.rendererFeatures [ i ] is PixelizeFeature )
//                {
//                    PixelizeFeature = ( PixelizeFeature ) ScriptableRendererData.rendererFeatures [ i ];
//                    PixelizeFeature.GetSettings().PixelSize = pixelSize;
//                }
//            }
//        }

//        private void AdjustPixelSizeOnZoom( float currentFOV, float _maxFOV )
//        {
//            _pixelSize = ( int ) ( currentFOV * ( _maxZoomedPixelSize - _minZoomedPixelSize ) / _maxFOV );
//            _pixelSize += _minZoomedPixelSize;

//            SetPixelizerPixelSize( _pixelSize );
//        }

//        private void SetPixelAdjustmentVariables()
//        {
//            _zoomPixelSubstraction = _pixelSize / 2;
//            _minZoomedPixelSize = _pixelSize - _zoomPixelSubstraction;
//            _maxZoomedPixelSize = _pixelSize;
//        }

//        #region OnValidate

//#if UNITY_EDITOR
//        protected override void OnValidate()
//        {
//            base.OnValidate();

//            if ( _virtualCamera.IsNull() ) _virtualCamera = GetComponent<CinemachineVirtualCamera>();

//            IsValid = !_virtualCamera.IsNull() && _virtualCamera.enabled;

//            SetPixelAdjustmentVariables();
//            SetPixelizerPixelSize( _pixelSize );
//        }
//#endif

//        #endregion
//    }
//}