using UnityEngine;
using NaughtyAttributes;
using dnSR_Coding.Utilities;

namespace dnSR_Coding
{
    ///<summary> CameraPixelizerSettings description <summary>
    [Component("CameraPixelizerSettings", "")]
    [DisallowMultipleComponent]
    public class CameraPixelizerSettings : MonoBehaviour, IDebuggable
    {
        [Header( "Rendering settings" )]
        [SerializeField] private bool _isEnabled = true;
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
            SetPixelizerPixelSize( _pixelSize );
            ToggleFeature();
        }

        /// <summary>
        /// Is in charge to find the pixelize feature used by the current render setting.
        /// </summary>
        private void GetPixelizerFeature()
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
        /// <summary>
        /// Sets the pixel value of the pixelizer feature.
        /// </summary>
        /// <param name="pixelSize"> Corresponds to the pixel size used by the pixelizer feature. </param>
        private void SetPixelizerPixelSize( int pixelSize )
        {
            GetPixelizerFeature();

            for ( int i = 0; i < ScriptableRendererData.rendererFeatures.Count; i++ )
            {
                if ( ScriptableRendererData.rendererFeatures [ i ] is FullScreenRenderPassFeature feature )
                {
                    PixelizeFeature = feature;
                    PixelizeFeature.SetPixelSize( pixelSize );
                }
            }
        }

        private void ToggleFeature()
        {
            if ( PixelizeFeature.IsNull() ) { return; }

            switch ( _isEnabled )
            {
                case true:
                    PixelizeFeature.EnableFeature();
                    break;

                case false:
                    PixelizeFeature.DisableFeature();
                    break;
            }
        }

        #region OnValidate

#if UNITY_EDITOR

        private void OnValidate()
        {
            SetPixelizerPixelSize( _pixelSize );
            ToggleFeature();
        }        
#endif

        #endregion
    }
}