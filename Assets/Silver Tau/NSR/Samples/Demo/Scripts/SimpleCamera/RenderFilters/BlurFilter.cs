using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class BlurFilter : RenderFilter
    {
        public BlurFilter(string name, Color color, Shader shader) : base(name, color, shader)
        {
            mainMaterial.SetInt("_KernelSize", 21);
        }

        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            var tmp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

            Graphics.Blit(src, tmp, mainMaterial, 0);
            Graphics.Blit(tmp, dst, mainMaterial, 1);

            RenderTexture.ReleaseTemporary(tmp);
        }
    }
}
