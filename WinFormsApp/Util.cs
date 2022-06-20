namespace Asteroids.WinFormsApp
{
    using System;
    using System.Drawing;
    using JetBrains.Annotations;

    internal static class Util
    {
        public static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1.0);

        public static void UndoClip<TContext>(GraphicsD g, TContext context, [InstantHandle] Action<TContext> draw)
        {
            var clip = g.Clip;
            draw(context);
            g.Clip = clip;
        }

        public static void UndoTransform<TContext>(GraphicsD g, TContext context, [InstantHandle] Action<TContext> draw)
        {
            var matrix = g.Transform;
            draw(context);
            g.Transform = matrix;
        }
    }
}
