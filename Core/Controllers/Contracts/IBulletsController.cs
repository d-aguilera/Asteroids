namespace Asteroids.Core
{
    using System.Numerics;

    internal interface IBulletsController : IController
    {
        void Add(IBulletInternal bullet);
        IBullet Spawn(Vector2D positionInches, Vector2D velocityIps);
    }
}
