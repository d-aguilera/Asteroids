namespace Asteroids.Core
{
    using System;

    internal class CollisionSequence
    {
        private readonly IShipController _shipController;

        public CollisionSequence(IShipInternal ship, IShipController shipController)
        {
            Ship = ship;
            _shipController = shipController;
        }

        public void Collide()
        {
            if (Ship.Status != ShipStatus.Ok)
            {
                return;
            }

            Ship.SetHealthPercent(Ship.HealthPercent - 0.05);

            if (Ship.HealthPercent < 0.025)
            {
                Ship.SetHealthPercent(0.0);
                Ship.SetStatus(ShipStatus.Exploding);
                Ship.SetStatusSequence(0);
                return;
            }

            _shipController.RespawnBegin(TimeSpan.FromMilliseconds(500.0));
        }

        private IShipInternal Ship { get; }
    }
}
