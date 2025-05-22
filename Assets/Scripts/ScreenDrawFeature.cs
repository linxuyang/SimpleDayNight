using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Marko
{
    public class ScreenDrawFeature : ScriptableRendererFeature
    {
        public RenderPassEvent evt;
        public Material material;
        private ScreenDrawPass _pass;


        public override void Create()
        {
            if (_pass != null) return;
            _pass = new ScreenDrawPass(this)
            {
                renderPassEvent = evt
            };
        }

        public void SetMaterial(Material mat)
        {
            material = mat;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (material == null)
            {
                return;
            }

            renderer.EnqueuePass(_pass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (_pass != null)
            {
                _pass.Setup(renderer.cameraColorTargetHandle);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _pass.Dispose();
        }

        class ScreenDrawPass : ScriptableRenderPass
        {
            private readonly ProfilingSampler _profilingSampler;
            private ScreenDrawFeature _feature;
            private RTHandle _renderTarget;

            public ScreenDrawPass(ScreenDrawFeature feature)
            {
                _feature = feature;
            }

            public void Setup(RTHandle cameraColorTarget)
            {
                _renderTarget = cameraColorTarget;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get(_feature.name);
                using (new ProfilingScope(cmd, _profilingSampler))
                {
                    CoreUtils.SetRenderTarget(cmd, _renderTarget);
                    cmd.DrawProcedural(Matrix4x4.identity, _feature.material, 0, MeshTopology.Triangles, 3);
                }
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                cmd.Dispose();
            }

            public void Dispose()
            {
                _feature = null;
                _renderTarget = null;
            }
        }
    }
}