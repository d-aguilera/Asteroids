namespace Asteroids.Core
{
    internal interface IAsteroidsController : IController
    {
        void Damage(IAsteroid asteroid);
    }
}
