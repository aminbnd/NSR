namespace SilverTau.NSR.Recorders {

    using System;
    using Internal;

    public sealed class MP4FormatRecorder : NativeRecorder {
        private static IntPtr Create (
            int width,
            int height,
            float frameRate,
            int sampleRate,
            int channelCount,
            int videoBitRate,
            int keyframeInterval,
            int audioBitRate
        ) {
            if (width % 2 != 0)
                throw new ArgumentException(@"MP4Recorder width must be divisible by 2", nameof(width));
            if (height % 2 != 0)
                throw new ArgumentException(@"MP4Recorder height must be divisible by 2", nameof(height));
            Corder.CreateMP4Recorder(
                HelperUtility.GetPath(@".mp4"),
                width,
                height,
                frameRate,
                sampleRate,
                channelCount,
                videoBitRate,
                keyframeInterval,
                audioBitRate,
                out var recorder
            );
            if (recorder == IntPtr.Zero)
                throw new InvalidOperationException(@"Failed to create MP4Recorder");
            return recorder;
        }
        
        public MP4FormatRecorder (
            int width,
            int height,
            float frameRate,
            int sampleRate = 0,
            int channelCount = 0,
            int videoBitRate = 10_000_000,
            int keyframeInterval = 2,
            int audioBitRate = 64_000
        ) : base(Create(width, height, frameRate, sampleRate, channelCount, videoBitRate, keyframeInterval, audioBitRate)) { }
    }
}