namespace Asteroids.Core
{
    using System.Numerics;

    public interface IShip : IRadialSprite, ICloneable<IShip>
    {
        double Accel { get; }
        double HealthPercent { get; }
        Vector2D[] HitBoxInches { get; }
        bool IsColliding { get; }
        bool IsVisible { get; }
        long LastCollisionTimestamp { get; }
        Vector2D NosePositionInches { get; }
        ShipStatus Status { get; }
        int StatusSequence { get; }
    }
}
