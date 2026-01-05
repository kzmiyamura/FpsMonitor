using System;
using System.Collections.Generic;

namespace FpsMonitor
{
    /// <summary>
    /// 指定した時間窓での移動平均
    /// （frame time(ms) を想定）
    /// </summary>
    public sealed class RollingAverage
    {
        private readonly TimeSpan _window;
        private readonly Queue<(DateTime time, double value)> _samples = new();

        public RollingAverage(TimeSpan window)
        {
            _window = window;
        }

        public void AddSample(double value)
        {
            var now = DateTime.UtcNow;
            _samples.Enqueue((now, value));

            while (_samples.Count > 0 &&
                   now - _samples.Peek().time > _window)
            {
                _samples.Dequeue();
            }
        }

        public double Average
        {
            get
            {
                if (_samples.Count == 0)
                    return 0;

                double sum = 0;
                foreach (var s in _samples)
                    sum += s.value;

                return sum / _samples.Count;
            }
        }
    }
}
