namespace SilverTau.NSR.Recorders.Internal {

    using AOT;
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;

    public abstract class NativeRecorder : IMRecorder {
        
        private readonly IntPtr recorder;

        protected NativeRecorder (IntPtr recorder) => this.recorder = recorder;

        [MonoPInvokeCallback(typeof(Corder.RecordingHandler))]
        private static unsafe void OnRecorderCompleted (IntPtr context, IntPtr path) {
            var handle = (GCHandle)context;
            var recordingTask = handle.Target as TaskCompletionSource<string>;
            handle.Free();
            if (path != null)
                recordingTask.SetResult(Marshal.PtrToStringAnsi(path));
            else
                recordingTask.SetException(new Exception(@"Recorder failed to finish writing"));
        }

        public static implicit operator IntPtr (NativeRecorder recorder) => recorder.recorder;
        public virtual (int width, int height) frameSize {
            get {
                recorder.FrameSize(out var width, out var height);
                return (width, height);
            }
        }
        
        public virtual unsafe void CommitFrame<T> (T[] pixelBuffer, long timestamp) where T : unmanaged {
            fixed (T* baseAddress = pixelBuffer)
                CommitFrame(baseAddress, timestamp);
        }

        public virtual unsafe void CommitFrame (void* nativeBuffer, long timestamp) {
            recorder.CommitFrame(nativeBuffer, timestamp);
        }

        public virtual unsafe void CommitSamples (float[] sampleBuffer, long timestamp) {
            fixed (float* baseAddress = sampleBuffer)
                CommitSamples(baseAddress, sampleBuffer.Length, timestamp);
        }

        public virtual unsafe void CommitSamples (float* nativeBuffer, int sampleCount, long timestamp) {
            recorder.CommitSamples(nativeBuffer, sampleCount, timestamp);
        }

        public virtual unsafe Task<string> FinishWriting () {
            var recordingTask = new TaskCompletionSource<string>();
            var handle = GCHandle.Alloc(recordingTask, GCHandleType.Normal);
            recorder.FinishWriting(OnRecorderCompleted, (IntPtr)handle);
            return recordingTask.Task;
        }
    }
}