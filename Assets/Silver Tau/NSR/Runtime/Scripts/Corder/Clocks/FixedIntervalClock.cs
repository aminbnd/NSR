namespace SilverTau.NSR.Recorders.Clocks {

    using System.Runtime.CompilerServices;

    public sealed class FixedIntervalClock : IClock {
        
        private readonly bool autoTick;
        private long ticks;
        public readonly double interval;
        
        public long timestamp {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get => (long)((autoTick ? ticks++ : ticks) * interval * 1e+9);
        }
        
        public FixedIntervalClock (float framerate, bool autoTick = true) {
            this.interval = 1.0 / framerate;
            this.ticks = 0L;
            this.autoTick = autoTick;
        }
        
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Tick () => ticks++;
    }
}