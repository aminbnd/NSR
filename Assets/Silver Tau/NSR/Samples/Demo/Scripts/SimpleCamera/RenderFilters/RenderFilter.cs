using UnityEngine;

namespace SilverTau.NSR.Samples
{
    public abstract class RenderFilter
    {
        protected Material mainMaterial;

        private string _name;
        private Color _color;

        protected RenderFilter(string name, Color color, Shader shader)
        {
            this._name = name;
            this._color = color;
            mainMaterial = new Material(shader);
        }

        public abstract void OnRenderImage(RenderTexture src, RenderTexture dst);

        public string GetName()
        {
            return _name;
        }

        public Color GetColor()
        {
            return _color;
        }
    }
}
