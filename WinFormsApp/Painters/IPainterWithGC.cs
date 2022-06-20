namespace Asteroids.WinFormsApp
{
    // ReSharper disable once InconsistentNaming
    internal interface IPainterWithGC : IPainter
    {
        int CollectGarbage();
    }
}
