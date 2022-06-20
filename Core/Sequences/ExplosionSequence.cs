namespace Asteroids.Core
{
    using System;
    using System.Numerics;

    internal class ExplosionSequence
    {
        private long _elapsedAcum;

        public ExplosionSequence(IShipInternal ship)
        {
            Ship = ship;
        }

        public void Explode(TimeSpan elapsed)
        {
            if (Ship.StatusSequence == 0)
            {
                Ship.SetStatus(ShipStatus.Exploding);
                Ship.SetVelocityIps(Vector2D.Zero);
                Ship.SetAccel(0.0);
                Ship.SetStatusSequence(Ship.StatusSequence + 1);
                _elapsedAcum = 0;
            }
            else
            {
                _elapsedAcum += elapsed.Ticks;
                Ship.SetStatusSequence(1 + (int)(81.0 * _elapsedAcum / (1620L * TimeSpan.TicksPerMillisecond)));
                if (Ship.StatusSequence > 81)
                {
                    Ship.SetStatus(ShipStatus.Dead);
                    Ship.SetStatusSequence(0);
                }
            }
        }

        private IShipInternal Ship { get; }
    }
}
