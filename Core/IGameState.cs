namespace Asteroids.Core
{
    using System.Collections.Generic;

    public interface IGameState
    {
        IEnumerable<IAsteroid> Asteroids { get; }
        IEnumerable<IBullet> Bullets { get; }
        IShip Ship { get; }
    }
}
