namespace SilverTau.NSR.Recorders.Inputs {

    using System;
    using UnityEngine;
    using Clocks;
    
    public sealed class AudioInput : IDisposable {

        private readonly IMRecorder recorder;
        private readonly IClock clock;
        private readonly AudioInputAttachment attachment;
        private readonly bool mute;

        private AudioInput (IMRecorder recorder, IClock clock, GameObject gameObject, bool mute = false) {
            this.recorder = recorder;
            this.clock = clock;
            this.attachment = gameObject.AddComponent<AudioInputAttachment>();
            this.attachment.sampleBufferDelegate = OnSampleBuffer;
            this.mute = mute;
        }

        private void OnSampleBuffer (float[] data) {
            AndroidJNI.AttachCurrentThread();
            recorder.CommitSamples(data, clock?.timestamp ?? 0L);
            if (mute)
                Array.Clear(data, 0, data.Length);
        }

        private class AudioInputAttachment : MonoBehaviour {
            public Action<float[]> sampleBufferDelegate;
            private void OnAudioFilterRead (float[] data, int channels) => sampleBufferDelegate?.Invoke(data);
        }
        
        public AudioInput (IMRecorder recorder, AudioListener audioListener) : this(recorder, default, audioListener) {}

        public AudioInput (IMRecorder recorder, IClock clock, AudioListener audioListener) : this(recorder, clock, audioListener.gameObject) {}

        public AudioInput (IMRecorder recorder, AudioSource audioSource, bool mute = false) : this(recorder, default, audioSource, mute) {}

        public AudioInput (IMRecorder recorder, IClock clock, AudioSource audioSource, bool mute = false) : this(recorder, clock, audioSource.gameObject, mute) {}

        public void Dispose () => AudioInputAttachment.Destroy(attachment);
    }
}