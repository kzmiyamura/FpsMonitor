namespace FpsMonitor.Core
{
    public sealed class FrameDropDetector
    {
        private readonly double _thresholdFps;
        private readonly int _requiredConsecutiveDrops;

        private int _dropCount;
        private FrameState _currentState = FrameState.Normal;

        public FrameState CurrentState => _currentState;

        public FrameDropDetector(
            double thresholdFps = 60.0,
            int requiredConsecutiveDrops = 3)
        {
            _thresholdFps = thresholdFps;
            _requiredConsecutiveDrops = requiredConsecutiveDrops;
        }

        public void Update(double currentFps)
        {
            if (currentFps < _thresholdFps)
            {
                _dropCount++;

                if (_dropCount >= _requiredConsecutiveDrops)
                {
                    _currentState = FrameState.Dropped;
                }
            }
            else
            {
                _dropCount = 0;
                _currentState = FrameState.Normal;
            }
        }
    }
}
