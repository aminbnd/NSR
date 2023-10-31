namespace SilverTau.NSR.Recorders.Inputs {

    using UnityEngine;
    using UnityEngine.Rendering;
    using Unity.Collections.LowLevel.Unsafe;
    
    public sealed class InputTextureAsync : IInputTexture {

        private IMRecorder recorder;
        (int, int) IInputTexture.frameSize => recorder.frameSize;
        
        public InputTextureAsync (IMRecorder recorder) => this.recorder = recorder;

        public unsafe void CommitFrame (Texture texture, long timestamp) {
            var (width, height) = recorder.frameSize;
            var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            Graphics.Blit(texture, renderTexture);
            AsyncGPUReadback.Request(
                renderTexture,
                0,
                request => recorder?.CommitFrame(request.GetData<byte>().GetUnsafeReadOnlyPtr(), timestamp)
            );
            RenderTexture.ReleaseTemporary(renderTexture);
        }
        
        public void Dispose () => recorder = null;
    }
}