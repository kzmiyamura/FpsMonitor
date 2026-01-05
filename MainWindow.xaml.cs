using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace FpsMonitor
{
    public partial class MainWindow : Window
    {
        // UI更新（低頻度）
        private readonly DispatcherTimer _uiTimer;

        // 描画計測（高頻度）
        private long _lastRenderTicks;
        private double _latestFps;

        // frame time(ms) の移動平均
        private readonly RollingAverage _avg10s =
            new(TimeSpan.FromSeconds(10));

        private readonly RollingAverage _avg30s =
            new(TimeSpan.FromSeconds(30));

        // 60fps基準
        private const double TargetFrameMs = 1000.0 / 60.0;

        // 連続フレーム落ち検出
        private int _slowFrameCount;
        private const int SlowFrameThreshold = 5;

        public MainWindow()
        {
            InitializeComponent();

            // 描画フレームごとの計測
            CompositionTarget.Rendering += OnRendering;

            // UI表示更新（100ms）
            _uiTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _uiTimer.Tick += OnUiTick;
            _uiTimer.Start();
        }

        /// <summary>
        /// 実際の描画フレームごとに呼ばれる
        /// </summary>
        private void OnRendering(object? sender, EventArgs e)
        {
            long now = Stopwatch.GetTimestamp();

            if (_lastRenderTicks == 0)
            {
                _lastRenderTicks = now;
                return;
            }

            double deltaSeconds =
                (double)(now - _lastRenderTicks) / Stopwatch.Frequency;

            _lastRenderTicks = now;

            if (deltaSeconds <= 0)
                return;

            // フレーム時間（ms）
            double frameMs = deltaSeconds * 1000.0;

            // 移動平均は frame time で取る（外れ値に強い）
            _avg10s.AddSample(frameMs);
            _avg30s.AddSample(frameMs);

            // 瞬間FPS表示用
            _latestFps = 1000.0 / frameMs;

            // フレーム落ち連続検出
            if (frameMs > TargetFrameMs)
                _slowFrameCount++;
            else
                _slowFrameCount = 0;
        }

        /// <summary>
        /// UI表示更新（低頻度）
        /// </summary>
        private void OnUiTick(object? sender, EventArgs e)
        {
            double avg10Fps = _avg10s.Average > 0
                ? 1000.0 / _avg10s.Average
                : 0;

            double avg30Fps = _avg30s.Average > 0
                ? 1000.0 / _avg30s.Average
                : 0;

            CurrentFpsText.Text = $"{_latestFps:F1} FPS";
            Avg10sText.Text = $"avg10s: {avg10Fps:F1}";
            Avg30sText.Text = $"avg30s: {avg30Fps:F1}";

            if (_slowFrameCount >= SlowFrameThreshold)
            {
                StateText.Text = "Frame Drop";
                StateText.Foreground = Brushes.Red;
            }
            else
            {
                StateText.Text = "Normal";
                StateText.Foreground = Brushes.Green;
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= OnRendering;
            base.OnClosed(e);
        }
    }
}
