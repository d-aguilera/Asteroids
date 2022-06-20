namespace Asteroids.Core
{
    using System;

    internal class RespawnSequence
    {
        private long _elapsedBlink;
        private TimeSpan _elapsedRespawn;

        public RespawnSequence(IShipInternal ship)
        {
            Ship = ship;
        }

        public void BeginRespawn(TimeSpan duration)
        {
            if (Ship.Status != ShipStatus.Ok)
            {
                throw new InvalidOperationException();
            }

            Ship.SetStatus(ShipStatus.Respawning);

            _elapsedBlink = 0;
            _elapsedRespawn = duration;
        }

        public void RespawnContinue(TimeSpan elapsed)
        {
            if (Ship.Status != ShipStatus.Respawning)
            {
                throw new InvalidOperationException();
            }

            _elapsedRespawn -= elapsed;

            if (_elapsedRespawn <= TimeSpan.Zero)
            {
                Ship.SetIsVisible(true);
                Ship.SetStatus(ShipStatus.Ok);
                Ship.SetStatusSequence(0);
                return;
            }

            _elapsedBlink += elapsed.Ticks;

            if (_elapsedBlink <= 100 * TimeSpan.TicksPerMillisecond)
            {
                return;
            }

            _elapsedBlink = 0;
            Ship.SetIsVisible(!Ship.IsVisible);
        }

        private IShipInternal Ship { get; }
    }
}
