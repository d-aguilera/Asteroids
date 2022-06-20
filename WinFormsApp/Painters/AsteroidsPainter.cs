namespace Asteroids.WinFormsApp
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Linq;
    using System.Numerics;
    using Core;
    using Properties;

    internal class AsteroidsPainter : IPainterWithGC
    {
        private readonly Dictionary<int, Bitmap> _asteroidImagesCache = new();
        private readonly Brush _projectionBrush;

        private Pen _hitBoxPen;

        private Images Images { get; }
        private IGameState GameState { get; }
        private Settings Settings { get; }

        public AsteroidsPainter(IGameState gameState, Settings settings, Images images)
        {
            GameState = gameState;
            Settings = settings;
            Images = images;

            _projectionBrush = new SolidBrush(Color.FromArgb(64, Settings.AsteroidHitBoxColor));
        }

        public void Draw(GraphicsD g)
        {
            var width = Settings.AsteroidHitBoxWidth / (float)g.DpiX;
            if (_hitBoxPen is null)
            {
                _hitBoxPen = new Pen(Settings.AsteroidHitBoxColor, width);
            }
            else if (Math.Abs(width - _hitBoxPen.Width) > 0.001)
            {
                _hitBoxPen.Dispose();
                _hitBoxPen = new Pen(Settings.AsteroidHitBoxColor, width);
            }

            foreach (var asteroid in GameState.Asteroids)
            {
                if (Settings.DrawAsteroidProjection)
                {
                    DrawAsteroidProjection(g, asteroid);
                }

                if (Settings.HitBoxMode)
                {
                    DrawAsteroidHitBox(g, asteroid);
                }
                else
                {
                    DrawAsteroid(g, asteroid);
                }
            }
        }

        private void DrawAsteroid(GraphicsD g, IAsteroid asteroid)
        {
            Bitmap bitmap;
            if (_asteroidImagesCache.ContainsKey(asteroid.Key))
            {
                // existing asteroid
                // get bitmap from cache
                bitmap = _asteroidImagesCache[asteroid.Key];
            }
            else
            {
                // new asteroid
                // create, scale, rotate and cache a new bitmap for it
                bitmap = Images.InitializeAsteroidImage(asteroid);
                _asteroidImagesCache.Add(asteroid.Key, bitmap);
            }

            var ctx0 = (g, asteroid, bitmap);

            Util.UndoTransform(g, ctx0, ctx1 =>
            {
                var g1 = ctx1.g;
                var asteroid1 = ctx1.asteroid;
                var pos = asteroid1.PositionInches;

                if (asteroid1.RotationSpeedRpm != 0.0)
                {
                    g1.TranslateTransform(pos.X, pos.Y);
                    g1.RotateTransform(asteroid1.HeadingAngle.Degrees);
                    g1.TranslateTransform(-pos.X, -pos.Y);
                }

                g1.DrawImage(ctx1.bitmap, pos.X - asteroid1.RadiusInches, pos.Y - asteroid1.RadiusInches);
            });
        }

        private void DrawAsteroidHitBox(GraphicsD g, IRadialSpriteWithHitBox asteroid)
        {
            var p = asteroid.PositionInches;
            var d = asteroid.HitDiameterInches;
            var r = asteroid.HitRadiusInches;

            g.DrawEllipse(_hitBoxPen, p.X - r, p.Y - r, d, d);
        }

        private void DrawAsteroidProjection(GraphicsD g, IRadialSpriteWithHitBox asteroid)
        {
            var vel = asteroid.VelocityIps;
            var pos1 = asteroid.PositionInches;
            var pos2 = pos1 + Settings.AsteroidProjectionLengthInSeconds * vel; // project asteroid position N seconds in the future
            var r = asteroid.HitRadiusInches;
            var normalVector1 = r * Vector2D.Normalize(new Vector2D(-vel.Y, vel.X));
            var normalVector2 = r * Vector2D.Normalize(new Vector2D(vel.Y, -vel.X));

            var p1 = pos1 + normalVector1;
            var p2 = pos2 + normalVector1;
            var p3 = pos2 + normalVector2;
            var p4 = pos1 + normalVector2;

            var d = (float)asteroid.HitDiameterInches;
            var normal2Degrees = (float)Angle.FromVector(normalVector2).Degrees;

            var path = new GraphicsPath();
            path.AddLine((float)p1.X, (float)p1.Y, (float)p2.X, (float)p2.Y);
            path.AddArc((float)(pos2.X - r), (float)(pos2.Y - r), d, d, normal2Degrees, 180f);
            path.AddLine((float)p3.X, (float)p3.Y, (float)p4.X, (float)p4.Y);
            path.AddArc((float)(pos1.X - r), (float)(pos1.Y - r), d, d, normal2Degrees + 180f, 180f);

            g.Graphics.FillPath(_projectionBrush, path);
        }

        public int CollectGarbage()
        {
            // garbage-collect orphan cache items
            var itemsCollected = 0;
            var referencedKeys = new HashSet<int>(GameState.Asteroids.Select(a => a.Key));

            foreach (var key in _asteroidImagesCache.Keys)
            {
                if (referencedKeys.Contains(key)) continue;
                _asteroidImagesCache[key].Dispose();
                _asteroidImagesCache.Remove(key);
                itemsCollected++;
            }

            return itemsCollected;
        }
    }
}
