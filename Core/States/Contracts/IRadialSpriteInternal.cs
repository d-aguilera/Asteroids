namespace Asteroids.Core
{
    internal interface IRadialSpriteInternal : IRadialSprite, ISpriteInternal
    {
        void SetDiameterInches(double value);
    }
}
