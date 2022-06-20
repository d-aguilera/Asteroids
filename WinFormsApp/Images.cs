namespace Asteroids.WinFormsApp
{
    using System.Drawing;
    using System.Drawing.Imaging;

    using Core;
    using Properties;

    internal class Images
    {
        private float Dpi { get; }

        private float BitmapResX { get; }
        private float BitmapResY { get; }

        public Images(float dpi)
        {
            this.Dpi = dpi;
            this.BitmapResX = dpi * dpi;
            this.BitmapResY = dpi * dpi;
            this.Spaceship = InitializeSpaceshipImage();
            this.SpaceshipThrust = InitializeSpaceshipThrustImage();
            this.Bullet = InitializeBulletImage();
            this.Explosions = InitializeExplosionImages();
            this.Asteroids = InitializeAsteroidImages();
        }

        public readonly Bitmap Spaceship;
        public readonly Bitmap SpaceshipThrust;
        public readonly Bitmap Bullet;
        public readonly Bitmap[] Explosions;
        public readonly Bitmap[] Asteroids;

        private static Bitmap[] InitializeAsteroidImages()
        {
            return new[]
            {
                Resources.asteroid01,
                Resources.asteroid02,
                Resources.asteroid03,
                Resources.asteroid04,
                Resources.asteroid05,
                Resources.asteroid06,
                Resources.asteroid07,
                Resources.asteroid08,
                Resources.asteroid09,
                Resources.asteroid10,
            };
        }

        private Bitmap[] InitializeExplosionImages()
        {
            var images = new Bitmap[81];

            var tileSizePixels = Resources.explosion.Size / 9;
            var size = (int)(AsteroidsLoop.ExplosionDiameterInches * Dpi);
            var destRect = new RectangleF(0, 0, size, size);

            for (var seq = 0; seq < images.Length; seq++)
            {
                var srcX = tileSizePixels.Width * (seq % 9);
                var srcY = tileSizePixels.Height * (int)(seq / 9.0);
                var srcRect = new RectangleF(srcX, srcY, tileSizePixels.Width, tileSizePixels.Height);

                var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                bitmap.SetResolution(BitmapResX, BitmapResY);

                using var g = Graphics.FromImage(bitmap);
                g.DrawImage(Resources.explosion, destRect, srcRect, GraphicsUnit.Pixel);

                images[seq] = bitmap;
            }

            return images;
        }

        private Bitmap InitializeSpaceshipImage()
        {
            var size = (int)(AsteroidsLoop.ShipDiameterInches * Dpi);
            var bitmap = new Bitmap(Resources.spaceship, size, size);
            bitmap.SetResolution(BitmapResX, BitmapResY);
            return bitmap;
        }

        private Bitmap InitializeSpaceshipThrustImage()
        {
            var size = (int)(AsteroidsLoop.ShipDiameterInches * Dpi);
            var bitmap = new Bitmap(Resources.spaceship_thrust, size, size);
            bitmap.SetResolution(BitmapResX, BitmapResY);
            return bitmap;
        }

        private Bitmap InitializeBulletImage()
        {
            var sizePixels = (int)(AsteroidsLoop.BulletDiameterInches * Dpi);
            var diameterPixels = new Size(sizePixels, sizePixels);
            var bitmap = new Bitmap(Resources.bullet, diameterPixels);
            bitmap.SetResolution(BitmapResX, BitmapResY);
            return bitmap;
        }

        public Bitmap InitializeAsteroidImage(IAsteroid asteroid)
        {
            var diameterPixels = (int)(asteroid.DiameterInches * Dpi);
            var radiusPixels = diameterPixels / 2;

            var bitmap = new Bitmap(diameterPixels, diameterPixels, PixelFormat.Format32bppArgb);
            bitmap.SetResolution(BitmapResX, BitmapResY);

            using var g = Graphics.FromImage(bitmap);
            g.TranslateTransform(radiusPixels, radiusPixels);
            g.RotateTransform((float)asteroid.HeadingAngle.Degrees);
            g.TranslateTransform(-radiusPixels, -radiusPixels);
            g.DrawImage(Asteroids[asteroid.Type], 0f, 0f, diameterPixels, diameterPixels);

            return bitmap;
        }
    }
}
