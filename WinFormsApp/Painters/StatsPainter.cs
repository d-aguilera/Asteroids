namespace Asteroids.WinFormsApp
{
    using System.Drawing;
    using System.Linq;
    using Core;
    using GameLoop;
    using Properties;

    internal class StatsPainter : IPainter
    {
        private const float RowHeight = 15f;
        private const float LabelWidth = 90f;
        private const float ValueWidth = 60f;

        private static readonly Brush Brush = new SolidBrush(Color.FromArgb(0, 255, 0));
        private static readonly StringFormat LabelFormat = new();
        private static readonly StringFormat ValueFormat = new() { Alignment = StringAlignment.Far };

        private IGameLoopBase<IGameState> Loop { get; }
        private Settings Settings { get; }
        private Font Font { get; }

        public StatsPainter(IGameLoopBase<IGameState> loop, Settings settings, Font font)
        {
            Loop = loop;
            Settings = settings;
            Font = font;
        }

        public void Draw(GraphicsD g)
        {
            if (!Settings.DrawStats) return;

            var ctx0 = (g, Settings, Loop, Font, Brush, LabelFormat, ValueFormat);

            Util.UndoClip(g, ctx0, ctx1 =>
            {
                ctx1.g.ResetClip();

                Util.UndoTransform(ctx1.g, ctx1, ctx2 =>
                {
                    DrawStats(ctx2.g, ctx2.Settings, ctx2.Loop, ctx2.Font, ctx2.Brush, ctx2.LabelFormat, ctx2.ValueFormat);
                });
            });
        }

        private static void DrawStats(GraphicsD g, Settings settings, IGameLoopBase<IGameState> loop, Font font, Brush brush, StringFormat labelFormat, StringFormat valueFormat)
        {
            g.ResetTransform();

            var labelRect = new RectangleF(0f, HealthBarPainter.BarHeightPixels, LabelWidth, 17f);
            var valueRect = new RectangleF(LabelWidth, HealthBarPainter.BarHeightPixels, ValueWidth, 17f);

            void DrawStat(string label, string value)
            {
                g.Graphics.DrawString(label, font, brush, labelRect, labelFormat);
                labelRect.Offset(0f, RowHeight);

                g.Graphics.DrawString(value, font, brush, valueRect, valueFormat);
                valueRect.Offset(0f, RowHeight);
            }

            var ship = loop.State.Ship;

            // draw stat labels and values
            DrawStat("Health:", FormatPercent(ship.HealthPercent));
            DrawStat("Position X:", FormatInches(ship.PositionInches.X));
            DrawStat("Position Y:", FormatInches(ship.PositionInches.Y));
            DrawStat("Speed:", ship.VelocityIps.Length().ToString("0.0 in/s"));
            DrawStat("Heading:", FormatDegrees(ship.HeadingAngle.Degrees));
            DrawStat("Course:", FormatDegrees(ship.CourseAngle.Degrees));
            DrawStat("Accel.:", ship.Accel.ToString("0.0 in/s2"));
            DrawStat("Asteroids:", loop.State.Asteroids.Count().ToString());
            DrawStat("Ticks/s:", FormatFps(loop.CurrentFps));
            DrawStat("Tick Avg.:", FormatMilliseconds(StatsContext.TimeTickAvg));
            DrawStat("UI FPS:", FormatFps(StatsContext.Fps));
            DrawStat("Total Paint:", FormatMilliseconds(StatsContext.TimeTotal.TotalMilliseconds));
            DrawStat("Get State:", FormatMilliseconds(StatsContext.TimeGettingState.TotalMilliseconds));
            DrawStat("Pre draw:", FormatMilliseconds(StatsContext.TimePreDraw.TotalMilliseconds));
            DrawStat("Draw grid:", FormatMilliseconds(StatsContext.TimeDrawGrid.TotalMilliseconds));
            DrawStat("Draw asteroids:", FormatMilliseconds(StatsContext.TimeDrawingAsteroids.TotalMilliseconds));
            DrawStat("Draw ship:", FormatMilliseconds(StatsContext.TimeDrawingShip.TotalMilliseconds));
            DrawStat("Draw bullets:", FormatMilliseconds(StatsContext.TimeDrawingBullets.TotalMilliseconds));
            DrawStat("Draw health:", FormatMilliseconds(StatsContext.TimeDrawingHealthBar.TotalMilliseconds));
            DrawStat("Draw stats:", FormatMilliseconds(StatsContext.TimeDrawingStats.TotalMilliseconds));
            DrawStat("Zoom level:", FormatPercent(settings.ZoomLevel));
        }

        private static string FormatDegrees(double value)
        {
            return value.ToString("0.0º");
        }

        private static string FormatFps(double value)
        {
            return value.ToString("0.0");
        }

        private static string FormatInches(double value)
        {
            return value.ToString("0.0 in");
        }

        private static string FormatMilliseconds(double value)
        {
            return value.ToString("0.000 ms");
        }

        private static string FormatPercent(double value)
        {
            return value.ToString("P0");
        }
    }
}
