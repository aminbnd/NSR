namespace SilverTau.NSR.Recorders.Inputs {

    using System;
    using System.Collections;
    using UnityEngine;
    using Clocks;
    
    public class CameraInput : IDisposable {

        private readonly IInputTexture input;
        private readonly IClock clock;
        protected readonly Camera[] cameras;
        private readonly CameraInputAttachment attachment;
        private RenderTextureDescriptor frameDescriptor;
        private int frameCount;

        private IEnumerator CommitFrames () {
            var yielder = new WaitForEndOfFrame();
            for (;;) {
                yield return yielder;
                if (frameCount++ % (frameSkip + 1) != 0)
                    continue;
                var frameBuffer = RenderTexture.GetTemporary(frameDescriptor);
                for (var i = 0; i < cameras.Length; i++)
                    CommitFrame(cameras[i], frameBuffer);
                input.CommitFrame(frameBuffer, clock?.timestamp ?? 0L);
                RenderTexture.ReleaseTemporary(frameBuffer);
            }
        }

        protected virtual void CommitFrame (Camera source, RenderTexture destination) {
            var prevTarget = source.targetTexture;
            source.targetTexture = destination;
            source.Render();
            source.targetTexture = prevTarget;
        }

        private static IInputTexture CreateInput (IMRecorder recorder) {
            if (SystemInfo.supportsAsyncGPUReadback) 
                return new InputTextureAsync(recorder);
            return new InputTexture(recorder);
        }

        private sealed class CameraInputAttachment : MonoBehaviour { }
        
        public int frameSkip;
        
        public bool HDR {
            get => frameDescriptor.colorFormat == RenderTextureFormat.ARGBHalf;
            set => frameDescriptor.colorFormat = value ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
        }
        
        public CameraInput (IMRecorder recorder, params Camera[] cameras) : this(recorder, default, cameras) { }

        public CameraInput (IMRecorder recorder, IClock clock, params Camera[] cameras) : this(CreateInput(recorder), clock, cameras) { }

        public CameraInput (IInputTexture input, params Camera[] cameras) : this(input, default, cameras) { }

        public CameraInput (IInputTexture input, IClock clock, params Camera[] cameras) {
            Array.Sort(cameras, (a, b) => (int)(100 * (a.depth - b.depth)));
            var (width, height) = input.frameSize;
            this.input = input;
            this.clock = clock;
            this.cameras = cameras;
            this.frameDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 24) {
                sRGB = true,
                msaaSamples = Mathf.Max(QualitySettings.antiAliasing, 1)
            };
            attachment = new GameObject("Corder CameraInputAttachment").AddComponent<CameraInputAttachment>();
            attachment.StartCoroutine(CommitFrames());
        }
        
        public void Dispose () {
            GameObject.Destroy(attachment.gameObject);
            input.Dispose();
        }
    }
}