namespace Asteroids.Core
{
    internal interface IAsteroidInternal : IAsteroid, IRadialSpriteWithHitBoxInternal
    {
        bool WasEverVisible { get; set; }

        void SetHealthPercent(double value);
        void SetParentKey(int value);
        void SetRotationSpeedRpm(double value);
    }
}
