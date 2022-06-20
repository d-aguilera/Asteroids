namespace Asteroids.Core
{
    using System.Collections.Generic;

    internal class GameState : IGameState
    {
        internal GameState(IShip ship)
        {
            Asteroids = new List<IAsteroidInternal>();
            Bullets = new List<IBulletInternal>();
            Ship = ship;
        }

        public IEnumerable<IAsteroid> Asteroids { get; }
        public IEnumerable<IBullet> Bullets { get; }
        public IShip Ship { get; }
    }
}
