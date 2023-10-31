using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class PixelFilter : RenderFilter
    {
        private const int pixelSize = 3;

        public PixelFilter(string name, Color color, Shader shader) : base(name, color, shader) { }

        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            var width = src.width / pixelSize;
            var height = src.height / pixelSize;

            var tmp = RenderTexture.GetTemporary(width, height, 0, src.format);

            tmp.filterMode = FilterMode.Point;

            Graphics.Blit(src, tmp);

            Graphics.Blit(tmp, dst, mainMaterial);

            RenderTexture.ReleaseTemporary(tmp);
        }
    }
}
