namespace Asteroids.Core
{
    public interface IBullet : IRadialSpriteWithHitBox, ICloneable<IBullet>
    {
        double DistanceTraveledInches { get; }
        bool IsNew { get; set; }
        int Key { get; }
    }
}
