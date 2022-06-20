namespace Asteroids.WinFormsApp
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using Core;
    using Properties;

    internal class BulletsPainter : IPainterWithGC
    {
        private readonly HashSet<int> _bulletsCache = new();

        private Images Images { get; }
        private IGameState GameState { get; }
        private Settings Settings { get; }

        public BulletsPainter(IGameState gameState, Settings settings, Images images)
        {
            GameState = gameState;
            Settings = settings;
            Images = images;
        }

        public void Draw(GraphicsD g)
        {
            foreach (var bullet in GameState.Bullets)
            {
                if (!_bulletsCache.Contains(bullet.Key))
                {
                    _bulletsCache.Add(bullet.Key);

                    if (Settings.SoundEffects)
                    {
                        SoundPlayers.Fire.Play();
                    }
                }

                if (Settings.HitBoxMode)
                {
                    DrawHitBox(g, Settings, bullet);
                }
                else
                {
                    g.DrawImage(
                        Images.Bullet,
                        bullet.PositionInches.X - bullet.RadiusInches,
                        bullet.PositionInches.Y - bullet.RadiusInches);
                }
            }
        }

        private static void DrawHitBox(GraphicsD g, Settings settings, IRadialSpriteWithHitBox bullet)
        {
            using var pen = new Pen(settings.BulletHitBoxColor, settings.BulletHitBoxWidth / (float)g.DpiX);

            var rect = new RectangleD(
                bullet.PositionInches.X - bullet.HitRadiusInches,
                bullet.PositionInches.Y - bullet.HitRadiusInches,
                bullet.HitDiameterInches,
                bullet.HitDiameterInches);

            g.DrawEllipse(pen, rect);
        }

        public int CollectGarbage()
        {
            // garbage-collect orphan bullets periodically
            var itemsCollected = 0;
            var referencedKeys = GameState.Bullets.Select(b => b.Key).ToList();

            _bulletsCache
                .Where(key => !referencedKeys.Contains(key))
                .ToList()
                .ForEach(key =>
                {
                    _bulletsCache.Remove(key);
                    itemsCollected++;
                });

            return itemsCollected;
        }
    }
}
