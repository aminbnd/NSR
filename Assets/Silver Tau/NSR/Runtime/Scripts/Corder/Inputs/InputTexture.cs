namespace SilverTau.NSR.Recorders.Inputs {

    using UnityEngine;
    using Unity.Collections.LowLevel.Unsafe;
    public sealed class InputTexture : IInputTexture {

        private readonly IMRecorder recorder;
        private readonly Texture2D readbackBuffer;
        (int, int) IInputTexture.frameSize => recorder.frameSize;
        public InputTexture (IMRecorder recorder) {
            this.recorder = recorder;
            this.readbackBuffer = new Texture2D(
                recorder.frameSize.width,
                recorder.frameSize.height,
                TextureFormat.RGBA32,
                false,
                false
            );
        }

        public unsafe void CommitFrame (Texture texture, long timestamp) {
            var (width, height) = recorder.frameSize;
            var renderTexture = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            Graphics.Blit(texture, renderTexture);
            var prevActive = RenderTexture.active;
            RenderTexture.active = renderTexture;
            readbackBuffer.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
            RenderTexture.active = prevActive;
            RenderTexture.ReleaseTemporary(renderTexture);
            recorder.CommitFrame(
                readbackBuffer.GetRawTextureData<byte>().GetUnsafeReadOnlyPtr(),
                timestamp
            );
        }
        
        public void Dispose () => Texture2D.Destroy(readbackBuffer);
    }
}