namespace SilverTau.NSR.Recorders {

    using System;
    using Internal;

    public sealed class HEVCFormatRecorder : NativeRecorder {
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
                throw new ArgumentException(@"HEVCRecorder width must be divisible by 2", nameof(width));
            if (height % 2 != 0)
                throw new ArgumentException(@"HEVCRecorder height must be divisible by 2", nameof(height));
            Corder.CreateHEVCRecorder(
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
                throw new InvalidOperationException(@"Failed to create HEVCRecorder");
            return recorder;
        }
        
        public HEVCFormatRecorder (
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