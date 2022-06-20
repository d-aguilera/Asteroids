namespace Asteroids.WinFormsApp
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using Core;
    using Properties;

    internal class GridPainter : IPainter
    {
        private const byte Start = (byte)PathPointType.Start;
        private const byte Line = (byte)PathPointType.Line;
        private const double MaxSizeInches = 2.0;
        private const double HalfMaxSizeInches = MaxSizeInches / 2.0;
        private const double HalfWidthInches = AsteroidsLoop.ViewportWidthInches / 2.0;
        private const double HalfHeightInches = AsteroidsLoop.ViewportHeightInches / 2.0;

        private static readonly RectangleD InnerViewport = new(
            -HalfWidthInches,
            -HalfHeightInches,
            AsteroidsLoop.ViewportWidthInches,
            AsteroidsLoop.ViewportHeightInches);

        private static readonly RectangleD InflatedViewport = RectangleD.Inflate(InnerViewport, HalfMaxSizeInches, HalfMaxSizeInches);
        private static readonly RectangleD OuterViewport = RectangleD.Inflate(InnerViewport, 3.0 * MaxSizeInches, 3.0 * MaxSizeInches);

        private Pen _gridPen;
        private Pen _viewportsPen;
        private GraphicsPath _gridPath;
        private GraphicsPath _viewportPaths;

        private Settings Settings { get; }

        public GridPainter(Settings settings, double dpi)
        {
            Settings = settings;

            CreatePens(dpi);
            CreateGridPaths();
            CreateViewportPaths();
        }

        public void Draw(GraphicsD g)
        {
            if (!Settings.DrawGrid) return;

            g.DrawPath(_viewportsPen, _viewportPaths);
            g.DrawPath(_gridPen, _gridPath);
        }

        private void CreateViewportPaths()
        {
            _viewportPaths = new GraphicsPath();
            _viewportPaths.AddRectangles(new[]
            {
                OuterViewport.ToRectangleF(),
                InflatedViewport.ToRectangleF(),
                InnerViewport.ToRectangleF(),
            });
        }

        private void CreatePens(double dpi)
        {
            var width = Settings.GridWidthMajor / (float)dpi;
            _gridPen = new Pen(Settings.GridColor, width);
            _viewportsPen = new Pen(Color.Green, width);
        }

        private void CreateGridPaths()
        {
            var inchesWide = Convert.ToInt32(Math.Ceiling(AsteroidsLoop.ViewportWidthInches));
            if (inchesWide % 2 == 1) inchesWide++;

            var inchesTall = Convert.ToInt32(Math.Ceiling(AsteroidsLoop.ViewportHeightInches));
            if (inchesTall % 2 == 1) inchesTall++;

            var bottom = inchesTall / 2;
            var top = 0 - bottom;
            var right = inchesWide / 2;
            var left = 0 - right;

            var count = inchesWide * 2 + inchesTall * 2;
            var pts = new PointF[count];
            var types = new byte[count];
            var i = -1;

            for (var x = left; x < right; x++)
            {
                pts[++i] = new Point(x, top);
                types[i] = Start;
                pts[++i] = new Point(x, bottom);
                types[i] = Line;
            }

            for (var y = top; y < bottom; y++)
            {
                pts[++i] = new Point(left, y);
                types[i] = Start;
                pts[++i] = new Point(right, y);
                types[i] = Line;
            }

            _gridPath = new GraphicsPath(pts, types);
        }
    }
}
