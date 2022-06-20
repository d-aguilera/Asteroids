namespace Asteroids.Core
{
    using System;
    using System.Numerics;

    internal class HeadHomeSequence
    {
        private readonly IShipController _shipController;

        private Angle _targetAngle;

        public HeadHomeSequence(IShip ship, IShipController shipController)
        {
            Ship = ship;
            _shipController = shipController;
        }

        public void HeadHome(TimeSpan elapsed)
        {
            // calculate bearing to origin
            _targetAngle = Angle.FromVector(-Ship.PositionInches);

            // decide whether to steer left or right
            var diff = Ship.HeadingAngle - _targetAngle;
            var step = elapsed.TotalMinutes * ShipController.SpinSpeedRpm * 360.0;

            if (diff.Degrees <= step || diff.Degrees > 360.0 - step)
            {
                // we won't get any closer to target angle
                _shipController.SetHeadingAngle(_targetAngle);

            }
            else if (diff.Degrees > 180.0)
            {
                _shipController.SpinRight();
            }
            else
            {
                _shipController.SpinLeft();
            }
        }

        private IShip Ship { get; }
    }
}
