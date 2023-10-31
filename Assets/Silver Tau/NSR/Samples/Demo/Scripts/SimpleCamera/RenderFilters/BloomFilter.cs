using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class BloomFilter : BlurFilter
    {
        private const int thresholdPass = 0;
        private const int horizontalPass = 2;
        private const int verticalPass = 3;
        private const int bloomPass = 4;

        protected BloomFilter(string name, Color color, Shader shader) : base(name, color, shader)
        {
            mainMaterial.SetFloat("_Spread", 5.0f);
        }

        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            var thresholdTex = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

            Graphics.Blit(src, thresholdTex, mainMaterial, thresholdPass);

            var blurTex = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

            base.OnRenderImage(thresholdTex, blurTex);

            RenderTexture.ReleaseTemporary(thresholdTex);

            mainMaterial.SetTexture("_SrcTex", src);

            Graphics.Blit(blurTex, dst, mainMaterial, bloomPass);

            RenderTexture.ReleaseTemporary(blurTex);
        }
    }
}
