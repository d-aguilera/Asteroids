namespace Asteroids.Core
{
    public interface IRadialSpriteWithHitBox : IRadialSprite
    {
        double HitDiameterInches { get; }
        double HitRadiusInches { get; }
    }
}
