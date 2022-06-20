namespace Asteroids.Core
{
    public interface IAsteroid : IRadialSpriteWithHitBox, ICloneable<IAsteroid>
    {
        double HealthPercent { get; }
        int Key { get; }
        int ParentKey { get; }
        double RotationSpeedRpm { get; }
        int Type { get; }
    }
}
