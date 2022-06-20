namespace Asteroids.Core
{
    using System.Collections.Generic;
    using System.Numerics;

    internal class BulletsController : ControllerBase, IBulletsController
    {
        private const double MaxBulletDistanceInches = AsteroidsLoop.ViewportWidthInches;

        private readonly List<IBulletInternal> _astray = new();
        private readonly List<IBulletInternal> _collided = new();

        private int _nextKey = 1;

        public BulletsController(IAsteroidsLoop gameLoop) : base(gameLoop)
        {
        }

        private List<IBulletInternal> Bullets => (List<IBulletInternal>)Loop.State.Bullets;

        public void Add(IBulletInternal bullet)
        {
            Bullets.Add(bullet);
        }

        public override void HandleInput()
        {
        }

        public override void UpdateModel()
        {
            if (Bullets.Count == 0) return;

            _astray.Clear();

            foreach (var bullet in Bullets)
            {
                bullet.UpdateModel(Loop.Elapsed);

                if (bullet.DistanceTraveledInches > MaxBulletDistanceInches)
                {
                    _astray.Add(bullet);
                }
            }

            foreach (var bullet in _astray)
            {
                Bullets.Remove(bullet);
            }
        }

        public override void Restart()
        {
            Bullets.Clear();
        }

        public override void HandleCollisions()
        {
            if (Bullets.Count == 0) return;

            _collided.Clear();

            var asteroids = (List<IAsteroidInternal>)Loop.State.Asteroids;

            foreach (var bullet in Bullets)
            {
                // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
                foreach (var asteroid in asteroids)
                {
                    var centerToCenterDistance = (asteroid.PositionInches - bullet.PositionInches).Length();
                    var physicalDistance = centerToCenterDistance - asteroid.HitRadiusInches - bullet.HitRadiusInches;
                    if (physicalDistance > 0.0) continue;

                    // hit
                    _collided.Add(bullet);
                    Loop.AsteroidsController.Damage(asteroid);
                    break;
                }
            }

            foreach (var bullet in _collided)
            {
                Bullets.Remove(bullet);
            }
        }

        public IBullet Spawn(Vector2D positionInches, Vector2D velocityIps)
        {
            var bullet = Bullet.Create(GetNextKey());
            bullet.SetDiameterInches(AsteroidsLoop.BulletDiameterInches);
            bullet.SetPositionInches(positionInches);
            bullet.SetVelocityIps(velocityIps);
            bullet.SetHitDiameterInches(AsteroidsLoop.BulletDiameterInches);

            Add(bullet);

            return bullet;
        }

        private int GetNextKey() => _nextKey++;
    }
}
