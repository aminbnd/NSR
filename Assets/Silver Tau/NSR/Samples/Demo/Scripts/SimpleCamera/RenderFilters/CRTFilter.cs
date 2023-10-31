using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class CRTFilter : BaseFilter
    {
        private PixelFilter pixelFilter;

        private const float brightness = 27.0f;
        private const float contrast = 2.1f;

        public CRTFilter(string name, Color color, Shader shader, PixelFilter pixelFilter) : base(name, color, shader)
        {
            this.pixelFilter = pixelFilter;

            mainMaterial.SetFloat("_Brightness", brightness);
            mainMaterial.SetFloat("_Contrast", contrast);
        }

        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            var tmp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

            pixelFilter.OnRenderImage(src, tmp);

            Graphics.Blit(tmp, dst, mainMaterial);

            RenderTexture.ReleaseTemporary(tmp);
        }
    }
}
