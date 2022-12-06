using UnityEngine;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> PlayerCamera_RenderingSettings description <summary>
    [Component( "Projection Settings", "Handle the rendering used by the camera, in this case, the precision of the pixelization feature." )]
    public class PlayerCamera_RenderingSettings : PlayerCamera_DefaultSettings
    {
        [Space( 5 )]

        [Header( "GENERAL SETTINGS" )]

        [Range( 1, 1000 ), SerializeField] private int _pixelSize = 150;

        private UnityEngine.Rendering.Universal.ScriptableRendererData ScriptableRendererData;
        private FullScreenRenderPassFeature PixelizeFeature;     

        void Awake() => Init();
        void Init()
        {
            SetPixelizerPixelSize( _pixelSize );
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
                if ( ScriptableRendererData.rendererFeatures [ i ] is FullScreenRenderPassFeature )
                {
                    PixelizeFeature = ( FullScreenRenderPassFeature ) ScriptableRendererData.rendererFeatures [ i ];
                    PixelizeFeature.SetPixelSize( pixelSize );
                }
            }
        }

        #region OnValidate

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            SetPixelizerPixelSize( _pixelSize );
        }
#endif

        #endregion
    }
}