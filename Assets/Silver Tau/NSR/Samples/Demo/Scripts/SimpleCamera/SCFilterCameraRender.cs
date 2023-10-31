using UnityEngine;

namespace SilverTau.NSR.Samples
{
    [RequireComponent(typeof(Camera))]
    public class SCFilterCameraRender : MonoBehaviour
    {
        /// <summary>
        /// Target filter.
        /// </summary>
        public RenderFilter Filter
        {
            get => _renderFilter;
            set => _renderFilter = value;
        }
        private RenderFilter _renderFilter;
        
        private void Start()
        {
        }
        
        /// <summary>
        /// Event function that Unity calls after a Camera has finished rendering, that allows you to modify the Camera's final image.
        /// </summary>
        /// <param name="src">Source RenderTexture.</param>
        /// <param name="dst">Destination RenderTexture.</param>
        private void OnRenderImage(RenderTexture src, RenderTexture dst)
        {
            _renderFilter?.OnRenderImage(src, dst);
        }
    }
}