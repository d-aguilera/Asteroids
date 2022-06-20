namespace Asteroids.WinFormsApp
{
    using System.Media;
    using Properties;

    internal static class SoundPlayers
    {
        public static readonly SoundPlayer Burn = new(Resources.burn);
        public static readonly SoundPlayer Fire = new(Resources.laser);
        public static readonly SoundPlayer Collision = new(Resources.damage);
        public static readonly SoundPlayer Explosion = new(Resources.boom);

        static SoundPlayers()
        {
            Burn.LoadAsync();
            Fire.LoadAsync();
            Collision.LoadAsync();
            Explosion.LoadAsync();
        }
    }
}
