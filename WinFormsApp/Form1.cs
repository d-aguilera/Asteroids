namespace Asteroids.WinFormsApp
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    using Core;
    using GameLoop;
    using Properties;

    public sealed partial class Form1 : Form
    {
        private readonly IGameLoopBase<IGameState> _gameLoop = new AsteroidsLoop();
        private readonly PaintState _paintState = new();
        private readonly HashSet<Keys> _pressedKeys = new();

        private readonly Telemetry _telemetry2;

        private readonly IPainter _gridPainter;
        private readonly IPainterWithGC _asteroidsPainter;
        private readonly IPainterWithGC _bulletsPainter;
        private readonly IPainter _healthBarPainter;
        private readonly IPainter _statsPainter;
        private readonly IPainter _shipPainter;

        public Form1()
        {
            InitializeComponent();

            var telemetry1 = _paintState.Telemetry;
            telemetry1.RegisterTimeCounter("total");
            telemetry1.RegisterTimeCounter("get-state");
            telemetry1.RegisterTimeCounter("pre-draw");
            telemetry1.RegisterTimeCounter("draw-grid");
            telemetry1.RegisterTimeCounter("draw-asteroids");
            telemetry1.RegisterTimeCounter("draw-ship");
            telemetry1.RegisterTimeCounter("draw-bullets");
            telemetry1.RegisterTimeCounter("draw-health-bar");
            telemetry1.RegisterTimeCounter("draw-stats");

            _telemetry2 = new Telemetry();
            _telemetry2.RegisterTimeCounter("ticks");

            var images = new Images(DeviceDpi);
            _gridPainter = new GridPainter(Settings.Default, DeviceDpi);
            _asteroidsPainter = new AsteroidsPainter(_gameLoop.State, Settings.Default, images);
            _bulletsPainter = new BulletsPainter(_gameLoop.State, Settings.Default, images);
            _healthBarPainter = new HealthBarPainter(_gameLoop.State, _paintState);
            _shipPainter = new ShipPainter(_gameLoop, Settings.Default, images);
            _statsPainter = new StatsPainter(_gameLoop, Settings.Default, Font);

            LoopWorker.RunWorkerAsync();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            var now = Stopwatch.GetTimestamp();
            var g = new GraphicsD(e.Graphics);

            var ctx0 = (
                settings: Settings.Default,
                now,
                g,
                _gameLoop.StateSync,
                _paintState,
                _asteroidsPainter,
                _bulletsPainter,
                _shipPainter,
                _healthBarPainter,
                _statsPainter,
                _gridPainter);

            _paintState.Telemetry.MeasureFrame("total", ctx0, ctx1 =>
            {
                var telemetry1 = ctx1._paintState.Telemetry;

                telemetry1.Measure("pre-draw", ctx1, c2 => PreDraw(c2.g, c2.now, c2.settings, c2._paintState, c2._asteroidsPainter, c2._bulletsPainter));
                telemetry1.Measure("draw-grid", ctx1, c2 => c2._gridPainter.Draw(c2.g));

                lock (ctx1.StateSync)
                {
                    telemetry1.Measure("draw-asteroids", ctx1, c2 => c2._asteroidsPainter.Draw(c2.g));
                    telemetry1.Measure("draw-ship", ctx1, c2 => c2._shipPainter.Draw(c2.g));
                    telemetry1.Measure("draw-bullets", ctx1, c2 => c2._bulletsPainter.Draw(c2.g));
                    telemetry1.Measure("draw-health-bar", ctx1, c2 => c2._healthBarPainter.Draw(c2.g));
                    telemetry1.Measure("draw-stats", ctx1, c2 => c2._statsPainter.Draw(c2.g));
                }
            });

            if (_paintState.Telemetry.TakeSnapshotAndResetEvery(Util.OneSecond, now, out var elapsed))
            {
                StatsContext.Collect(_paintState.Telemetry);
                StatsContext.Fps = _paintState.Telemetry.ReadTickSnapshot() / elapsed.TotalSeconds;
            }
        }

        private static void PreDraw(
            GraphicsD g, long now, Settings settings, PaintState paintState, IPainterWithGC asteroidsPainter, IPainterWithGC bulletsPainter)
        {
            var zoom = settings.ZoomLevel;

            paintState.UpdateElapsed(now);

            if (paintState.GcNeeded())
            {
                StatsContext.GcItemsCollected += asteroidsPainter.CollectGarbage();
                StatsContext.GcItemsCollected += bulletsPainter.CollectGarbage();
            }

            g.TranslateTransform(paintState.TranslateX, paintState.TranslateY);
            g.ScaleTransform(paintState.ScaleX * zoom, paintState.ScaleY * zoom);

            g.Clip = paintState.Clip;

            g.Clear(Color.FromArgb(0, 0, 64));
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_pressedKeys.Contains(e.KeyCode))
            {
                _pressedKeys.Add(e.KeyCode);

                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        _paintState.Quit = true;
                        return;

                    case Keys.G:
                        Settings.Default.DrawGrid = !Settings.Default.DrawGrid;
                        return;

                    case Keys.H:
                        Settings.Default.HitBoxMode = !Settings.Default.HitBoxMode;
                        return;

                    case Keys.Tab:
                        Settings.Default.DrawStats = !Settings.Default.DrawStats;
                        return;

                    case Keys.Oemplus:
                        Settings.Default.ZoomLevel *= 1.1;
                        return;

                    case Keys.OemMinus:
                        Settings.Default.ZoomLevel /= 1.1;
                        return;
                }
            }

            RegisterKeyEvent(LoopKeyEventTypes.KeyDown, e);
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (_pressedKeys.Contains(e.KeyCode))
            {
                _pressedKeys.Remove(e.KeyCode);
            }

            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (e.KeyCode)
            {
                case Keys.Escape:
                case Keys.G:
                case Keys.H:
                case Keys.Tab:
                case Keys.Oemplus:
                case Keys.OemMinus:
                    return;
            }

            RegisterKeyEvent(LoopKeyEventTypes.KeyUp, e);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _paintState.Quit = true;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            _paintState.ClientSize = ClientSize;
        }

        private void LoopWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var ctx0 = (now: 0L, _gameLoop);

            while (!_paintState.Quit)
            {
                ctx0.now = Stopwatch.GetTimestamp();

                _telemetry2.MeasureFrame("ticks", ctx0, ctx1 =>
                {
                    lock (ctx1._gameLoop.StateSync)
                    {
                        ctx1._gameLoop.Tick(ctx1.now);
                    }
                });

                if (_telemetry2.TakeSnapshotAndResetEvery(Util.OneSecond, ctx0.now, out _))
                {
                    var span = _telemetry2.GetAverage("ticks");
                    Interlocked.Exchange(ref StatsContext.TimeTickAvg, span.TotalMilliseconds);
                }

                Invalidate();
            }
        }

        private void RegisterKeyEvent(LoopKeyEventTypes type, KeyEventArgs e)
        {
            var keyData = (LoopKeys)(int)e.KeyData;
            var args = new LoopKeyEventArgs(keyData)
            {
                Handled = e.Handled,
                SuppressKeyPress = e.SuppressKeyPress,
            };

            _gameLoop.RegisterKeyEvent(type, args);
        }
    }
}
