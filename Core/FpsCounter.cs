using System.Diagnostics;

namespace FpsMonitor.Core
{
    public sealed class FpsCounter
    {
        private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        private long _lastTick;
        private double _currentFps;

        public double CurrentFps => _currentFps;

        public void Tick()
        {
            var now = _stopwatch.ElapsedTicks;
            var deltaTicks = now - _lastTick;

            if (deltaTicks > 0)
            {
                _currentFps = Stopwatch.Frequency / (double)deltaTicks;
            }

            _lastTick = now;
        }
    }
}
