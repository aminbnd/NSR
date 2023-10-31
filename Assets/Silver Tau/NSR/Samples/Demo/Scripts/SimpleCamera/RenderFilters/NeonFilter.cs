using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class NeonFilter : BloomFilter
    {
        private BaseFilter neonFilter;

        public NeonFilter(string name, Color color, Shader shader, BaseFilter neonFilter) : base(name, color, shader)
        {
            this.neonFilter = neonFilter;
        }

        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            var tmp = RenderTexture.GetTemporary(src.width, src.height, 0, src.format);

            neonFilter.OnRenderImage(src, tmp);

            base.OnRenderImage(tmp, dst);

            RenderTexture.ReleaseTemporary(tmp);
        }
    }
}
