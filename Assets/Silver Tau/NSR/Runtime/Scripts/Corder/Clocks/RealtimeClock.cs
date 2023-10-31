namespace SilverTau.NSR.Recorders.Clocks {

    using System;
    using System.Runtime.CompilerServices;
    using Stopwatch = System.Diagnostics.Stopwatch;
    
    public sealed class RealtimeClock : IClock {
        
        private readonly Stopwatch stopwatch;
        
        public long timestamp {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get {
                var time = stopwatch.Elapsed.Ticks * 100L;
                if (!stopwatch.IsRunning)
                    stopwatch.Start();
                return time;
            }
        }

        public bool paused {
            [MethodImpl(MethodImplOptions.Synchronized)] get => !stopwatch.IsRunning;
            [MethodImpl(MethodImplOptions.Synchronized)] set => (value ? (Action)stopwatch.Stop : stopwatch.Start)();
        }

        public RealtimeClock () => this.stopwatch = new Stopwatch();
    }
}