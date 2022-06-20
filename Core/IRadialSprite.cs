namespace Asteroids.Core
{
    public interface IRadialSprite : ISprite
    {
        double DiameterInches { get; }
        double Mass { get; }
        double RadiusInches { get; }
    }
}
