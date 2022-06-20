namespace Asteroids.Core
{
    using System;
    using System.Numerics;

    internal class BrakeSequence
    {
        private readonly IShipController _shipController;

        private int _sequence;
        private Angle _targetHeading;

        public BrakeSequence(IShip ship, IShipController shipController)
        {
            Ship = ship;
            _shipController = shipController;
        }

        public void Reset()
        {
            _sequence = 0;
        }

        public void Brake(TimeSpan elapsed)
        {
            switch (_sequence)
            {
                case 0:
                    DetermineTargetHeading();
                    break;

                case 1:
                    SpinToTargetHeading(elapsed);
                    break;

                case 2:
                    RetroBurn();
                    break;
            }
        }

        private void DetermineTargetHeading()
        {
            if (Ship.VelocityIps == Vector2D.Zero)
            {
                // ship is not moving
                return;
            }

            // find angle opposite to current velocity
            _targetHeading = Angle.FromVector(-Ship.VelocityIps);

            _sequence++;
        }

        private void SpinToTargetHeading(TimeSpan elapsed)
        {
            // decide whether to steer left or right
            var diff = Ship.HeadingAngle - _targetHeading;
            var step = elapsed.TotalMinutes * ShipController.SpinSpeedRpm * 360.0;

            if (diff.Degrees - step <= 0.0 || diff.Degrees + step > 360.0)
            {
                // we won't get any closer to target heading
                _shipController.SetHeadingAngle(_targetHeading);
                _sequence++;
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

        private void RetroBurn()
        {
            // retro-burn until speed is equal to zero (with a tolerance)
            if (Ship.VelocityIps.Length() >= 0.1)
            {
                _shipController.Burn();
            }
            else
            {
                _shipController.SetVelocityIps(Vector2D.Zero);
                _sequence++;
            }
        }

        private IShip Ship { get; }
    }
}
