namespace SilverTau.NSR.Recorders {

    using System.Threading.Tasks;

    public interface IMRecorder {

        (int width, int height) frameSize { get; }

        void CommitFrame<T> (T[] pixelBuffer, long timestamp) where T : unmanaged;

        unsafe void CommitFrame (void* nativeBuffer, long timestamp);
        
        void CommitSamples (float[] sampleBuffer, long timestamp);

        unsafe void CommitSamples (float* nativeBuffer, int sampleCount, long timestamp);

        Task<string> FinishWriting ();
    }
}