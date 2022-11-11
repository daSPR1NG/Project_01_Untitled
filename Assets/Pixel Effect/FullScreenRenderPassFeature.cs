using dnSR_Coding.Utilities;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FullScreenRenderPassFeature : ScriptableRendererFeature
{
    [Header( "DEPENDENCIES" )]
    [SerializeField] private bool _enableRendererFeature = true;

    [Space( 10 )]

    [Header( "PIXEL EFFECT SETTINGS" )]
    [SerializeField] private Material _material = null;
    [SerializeField, Range( 1, 1000 )] private int _pixelSize = 50;

    class CustomRenderPass : ScriptableRenderPass
    {
        [SerializeField] private bool _enableRendererFeature;

        [SerializeField] private Material _material;
        [SerializeField] private int _pixelSize = 50;

        private RenderTargetIdentifier _sourceTexture;
        private RTHandle _tempTexture;

        public CustomRenderPass( bool enableFeature, Material material, int pixelSize ) : base()
        {
            this._enableRendererFeature = enableFeature;

            this._material = material;
            this._pixelSize = pixelSize;

            SetPixelSize( this._material, pixelSize );
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup( CommandBuffer cmd, ref RenderingData renderingData )
        {
            if ( !_enableRendererFeature ) { return; }

            _sourceTexture = renderingData.cameraData.renderer.cameraColorTarget;

            _tempTexture = RTHandles.Alloc( new RenderTargetIdentifier( "_TempTexture" ), name: "_TempTexture" );
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute( ScriptableRenderContext context, ref RenderingData renderingData )
        {
            if ( !_enableRendererFeature ) { return; }

            CommandBuffer cB = CommandBufferPool.Get( "Full Screen Render Feature" );

            RenderTextureDescriptor targetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            targetDescriptor.depthBufferBits = 0;

            cB.GetTemporaryRT( Shader.PropertyToID( _tempTexture.name ), targetDescriptor, FilterMode.Bilinear );

            Blit( cB, _sourceTexture, _tempTexture, _material );
            Blit( cB, _tempTexture, _sourceTexture );

            context.ExecuteCommandBuffer( cB );

            CommandBufferPool.Release( cB );
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup( CommandBuffer cmd )
        {
            if ( !_enableRendererFeature ) { return; }

            _tempTexture.Release();
        }

        private void SetPixelSize( Material material, int pixelSize )
        {
            int value = pixelSize;

            material.SetFloat( "_PixelSize", value );
        }
    }

    CustomRenderPass m_ScriptablePass;

    /// <inheritdoc/>
    public override void Create()
    {
        if ( !_enableRendererFeature ) { return; }

        m_ScriptablePass = new CustomRenderPass( this._enableRendererFeature, this._material, this._pixelSize )
        {
            // Configures where the render pass should be injected.
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing
        };
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if ( !_enableRendererFeature ) { return; }

        renderer.EnqueuePass(m_ScriptablePass);
    }

    private void OnValidate()
    {
        if ( _material.GetFloat( "_PixelSize" ) != _pixelSize )
        {
            _material.SetFloat( "_PixelSize", _pixelSize );
        }
    }
}