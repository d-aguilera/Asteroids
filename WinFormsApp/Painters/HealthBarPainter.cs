namespace Asteroids.WinFormsApp
{
    using System.Drawing;
    using Core;

    internal class HealthBarPainter : IPainter
    {
        public const float BarHeightPixels = 16f;

        private double _healthPercent;

        private PaintState PaintState { get; }
        private IGameState GameState { get; }

        public HealthBarPainter(IGameState gameState, PaintState getPaintState)
        {
            GameState = gameState;
            PaintState = getPaintState;
        }

        public void Draw(GraphicsD g)
        {
            var ship = GameState.Ship;

            if (ship is null) return;

            var shipHealth = ship.HealthPercent;

            if (_healthPercent > shipHealth)
            {
                _healthPercent -= 0.1 * PaintState.TimeSinceLastPaintEvent.TotalSeconds;
            }

            if (_healthPercent < shipHealth)
            {
                _healthPercent = shipHealth;
            }

            using var brush = new SolidBrush(GetHealthColor(_healthPercent));

            var width = _healthPercent * PaintState.ClientSize.Width;

            var ctx0 = (g, brush, width);

            Util.UndoClip(g, ctx0, ctx1 =>
            {
                ctx1.g.ResetClip();

                Util.UndoTransform(ctx1.g, ctx1, ctx2 =>
                {
                    ctx2.g.ResetTransform();
                    ctx2.g.FillRectangle(ctx2.brush, 0.0, 0.0, ctx2.width, BarHeightPixels);
                });
            });
        }

        private static Color GetHealthColor(double percent)
        {
            const double yellowPeak = 0.5;
            const double yellowWidth = 0.6;
            const double halfWidth = yellowWidth / 2.0;

            var distFromPeak = percent - yellowPeak;
            var ratio = distFromPeak / halfWidth;

            double red, green;

            switch (distFromPeak)
            {
                case <= -halfWidth:
                    red = 1.0;
                    green = 0.0;
                    break;

                case <= 0.0:
                    red = 1.0;
                    green = ratio + 1.0;
                    break;

                case <= halfWidth:
                    red = 1.0 - ratio;
                    green = 1.0;
                    break;

                default:
                    red = 0.0;
                    green = 1.0;
                    break;
            }

            return Color.FromArgb((int)(255 * red), (int)(255 * green), 0);
        }
    }
}
