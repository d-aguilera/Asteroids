namespace Asteroids.Core
{
    internal interface IRadialSpriteWithHitBoxInternal : IRadialSpriteWithHitBox, IRadialSpriteInternal
    {
        void SetHitDiameterInches(double value);
    }
}
