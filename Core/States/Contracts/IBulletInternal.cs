namespace Asteroids.Core
{
    internal interface IBulletInternal : IBullet, IRadialSpriteWithHitBoxInternal
    {
        void SetDistanceTraveledInches(double value);
    }
}
