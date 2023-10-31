using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public class BaseFilter : RenderFilter
    {
        public BaseFilter(string name, Color color, Shader shader) : base(name, color, shader) { }

        public override void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            Graphics.Blit(src, dst, mainMaterial);
        }
    }
}
