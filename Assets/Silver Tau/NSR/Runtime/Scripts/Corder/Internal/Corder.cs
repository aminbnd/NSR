namespace SilverTau.NSR.Recorders.Internal {

    using System;
    using System.Runtime.InteropServices;
    
    public static class Corder {

        private const string Assembly =
        #if PLATFORM_IOS && !UNITY_EDITOR
        "\u005f\u005f\u0049\u006e\u0074\u0065\u0072\u006e\u0061\u006c";
        #elif PLATFORM_STANDALONE && !UNITY_EDITOR
        "\u0053\u0074\u0063\u0043\u006f\u0072\u0064\u0065\u0072";
        #else
        "\u0053\u0074\u0063\u0043\u006f\u0072\u0064\u0065\u0072";
        #endif
        
        public delegate void RecordingHandler (IntPtr context, IntPtr path);
        
        [DllImport(Assembly, EntryPoint = "\u004e\u0043\u004d\u0065\u0064\u0069\u0061\u0052\u0065\u0063\u006f\u0072\u0064\u0065\u0072\u0046\u0072\u0061\u006d\u0065\u0053\u0069\u007a\u0065")]
        public static extern void FrameSize (
            this IntPtr recorder,
            out int width,
            out int height
        );

        [DllImport(Assembly, EntryPoint = "\u004e\u0043\u004d\u0065\u0064\u0069\u0061\u0052\u0065\u0063\u006f\u0072\u0064\u0065\u0072\u0043\u006f\u006d\u006d\u0069\u0074\u0046\u0072\u0061\u006d\u0065")]
        public static extern unsafe void CommitFrame (
            this IntPtr recorder,
            void* pixelBuffer,
            long timestamp
        );

        [DllImport(Assembly, EntryPoint = "\u004e\u0043\u004d\u0065\u0064\u0069\u0061\u0052\u0065\u0063\u006f\u0072\u0064\u0065\u0072\u0043\u006f\u006d\u006d\u0069\u0074\u0053\u0061\u006d\u0070\u006c\u0065\u0073")]
        public static extern unsafe void CommitSamples (
            this IntPtr recorder,
            float* sampleBuffer,
            int sampleCount,
            long timestamp
        );

        [DllImport(Assembly, EntryPoint = "\u004e\u0043\u004d\u0065\u0064\u0069\u0061\u0052\u0065\u0063\u006f\u0072\u0064\u0065\u0072\u0046\u0069\u006e\u0069\u0073\u0068\u0057\u0072\u0069\u0074\u0069\u006e\u0067")]
        public static extern void FinishWriting (
            this IntPtr recorder,
            RecordingHandler handler,
            IntPtr context
        );
        
        [DllImport(Assembly, EntryPoint = "\u004e\u0043\u0043\u0072\u0065\u0061\u0074\u0065\u004d\u0050\u0034\u0052\u0065\u0063\u006f\u0072\u0064\u0065\u0072")]
        public static extern void CreateMP4Recorder (
            [MarshalAs(UnmanagedType.LPStr)] string path,
            int width,
            int height,
            float frameRate,
            int sampleRate,
            int channelCount,
            int videoBitrate,
            int keyframeInterval,
            int audioBitRate,
            out IntPtr recorder
        );
        
        [DllImport(Assembly, EntryPoint = "\u004e\u0043\u0043\u0072\u0065\u0061\u0074\u0065\u0048\u0045\u0056\u0043\u0052\u0065\u0063\u006f\u0072\u0064\u0065\u0072")]
        public static extern void CreateHEVCRecorder (
            [MarshalAs(UnmanagedType.LPStr)] string path,
            int width,
            int height,
            float frameRate,
            int sampleRate,
            int channelCount,
            int videoBitRate,
            int keyframeInterval,
            int audioBitRate,
            out IntPtr recorder
        );
    }
}