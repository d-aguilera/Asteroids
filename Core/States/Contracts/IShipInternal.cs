namespace Asteroids.Core
{
    internal interface IShipInternal : IShip, IRadialSpriteInternal
    {
        void SetAccel(double value);
        void SetHealthPercent(double value);
        void SetIsColliding(bool value);
        void SetIsVisible(bool value);
        void SetLastCollisionTimestamp(long value);
        void SetStatus(ShipStatus value);
        void SetStatusSequence(int value);
    }
}
