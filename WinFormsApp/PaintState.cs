namespace Asteroids.WinFormsApp
{
    using System;
    using System.Drawing;
    using Core;

    internal class PaintState
    {
        private Size _clientSize;

        public Size ClientSize
        {
            get => _clientSize;
            set
            {
                _clientSize = value;

                const double viewportRatio = AsteroidsLoop.ViewportWidthInches / AsteroidsLoop.ViewportHeightInches;

                var clientRatio = 1.0 * value.Width / value.Height;
                var scale = clientRatio > viewportRatio
                    ? value.Height / AsteroidsLoop.ViewportHeightInches
                    : value.Width / AsteroidsLoop.ViewportWidthInches;

                ScaleX = ScaleY = scale;
                TranslateX = value.Width / 2.0;
                TranslateY = value.Height / 2.0;
            }
        }

        public Region Clip { get; } = new(new RectangleD(
            -AsteroidsLoop.ViewportWidthInches / 2.0,
            -AsteroidsLoop.ViewportHeightInches / 2.0,
            AsteroidsLoop.ViewportWidthInches,
            AsteroidsLoop.ViewportHeightInches).ToRectangleF());

        public bool Quit { get; set; }
        public double ScaleX { get; private set; }
        public double ScaleY { get; private set; }
        public Telemetry Telemetry { get; } = new();
        public TimeSpan TimeSinceLastPaintEvent { get; private set; }
        public double TranslateX { get; private set; }
        public double TranslateY { get; private set; }

        private long GcLast { get; set; }
        private long Now { get; set; }

        public bool GcNeeded()
        {
            if (Now - GcLast <= TimeSpan.FromSeconds(5.0).Ticks)
            {
                return false;
            }

            GcLast = Now;

            return true;
        }

        public void UpdateElapsed(long now)
        {
            TimeSinceLastPaintEvent = Now == default
                ? TimeSpan.Zero
                : TimeSpan.FromTicks(now - Now);

            Now = now;
        }
    }
}
