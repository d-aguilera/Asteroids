namespace Asteroids.Core
{
    using System;
    using System.Collections.Generic;
    using System.Numerics;
    using GameLoop;

    internal class ShipController : ControllerBase, IShipController
    {
        public const double SpinSpeedRpm = 60.0;

        private const double BurnAcceleration = 5.0;
        private const double BulletSpeedIps = 10.0;
        private const int MaxShotsPerSecond = 6;

        private static readonly IEnumerable<LoopKeys> KeysAccel = new[] { LoopKeys.Up, LoopKeys.W };
        private static readonly IEnumerable<LoopKeys> KeysLeft = new[] { LoopKeys.Left, LoopKeys.A };
        private static readonly IEnumerable<LoopKeys> KeysBrake = new[] { LoopKeys.Down, LoopKeys.S };
        private static readonly IEnumerable<LoopKeys> KeysRight = new[] { LoopKeys.Right, LoopKeys.D };
        private static readonly IEnumerable<LoopKeys> KeysFire = new[] { LoopKeys.Space, LoopKeys.Enter };
        private static readonly IEnumerable<LoopKeys> KeysHome = new[] { LoopKeys.Home, LoopKeys.C };

        private readonly BrakeSequence _brakeSequence;
        private readonly HeadHomeSequence _headHomeSequence;
        private readonly CollisionSequence _collisionSequence;
        private readonly ExplosionSequence _explosionSequence;
        private readonly RespawnSequence _respawnSequence;

        private long _lastFireTicks;

        private IShipInternal Ship => (IShipInternal)Loop.State.Ship;

        public ShipController(IAsteroidsLoop gameLoop) : base(gameLoop)
        {
            _brakeSequence = new BrakeSequence(Ship, this);
            _headHomeSequence = new HeadHomeSequence(Ship, this);
            _collisionSequence = new CollisionSequence(Ship, this);
            _explosionSequence = new ExplosionSequence(Ship);
            _respawnSequence = new RespawnSequence(Ship);
        }

        public override void HandleInput()
        {
            if (Ship.Status == ShipStatus.Dead)
            {
                if (!IsKeyDown(KeysFire)) return;

                Ship.SetHealthPercent(1.0);
                Ship.SetStatus(ShipStatus.Ok);
                RespawnBegin(TimeSpan.FromSeconds(3.0));

                return;
            }

            if (Ship.Status == ShipStatus.Exploding)
            {
                return;
            }

            if (IsKeyDown(KeysAccel))
            {
                Burn();
            }
            else
            {
                Meco();
            }

            if (IsKeyDown(KeysLeft))
            {
                SpinLeft();
            }

            if (IsKeyDown(KeysBrake))
            {
                Brake();
            }
            else
            {
                ReleaseBrake();
            }

            if (IsKeyDown(KeysRight))
            {
                SpinRight();
            }

            if (IsKeyDown(KeysFire))
            {
                Fire();
            }

            if (IsKeyDown(KeysHome))
            {
                HeadHome();
            }
        }

        public override void UpdateModel()
        {
            switch (Ship.Status)
            {
                case ShipStatus.Dead:
                    return;

                case ShipStatus.Exploding:
                    Explode();
                    return;

                case ShipStatus.Respawning:
                    RespawnContinue();
                    break;

                case ShipStatus.Ok:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Ship.UpdateModel(Loop.Elapsed);
        }

        public override void HandleCollisions()
        {
            if (!AsteroidsLoop.CollisionDetection || Ship.Status != ShipStatus.Ok)
            {
                return;
            }

            var collisionDetected = false;
            foreach (var asteroid in Loop.State.Asteroids)
            {
                var centerToCenterDistance = (asteroid.PositionInches - Ship.PositionInches).Length();
                var edgeToEdgeDistance = centerToCenterDistance - asteroid.HitRadiusInches - Ship.RadiusInches;
                if (edgeToEdgeDistance > 0.0) continue;

                // asteroid is close enough, check the hit box
                foreach (var point in Ship.HitBoxInches)
                {
                    var distanceToHitBoxPoint = (point - asteroid.PositionInches).Length() - asteroid.HitRadiusInches;
                    if (distanceToHitBoxPoint > 0.0) continue;

                    collisionDetected = true;
                    break;
                }

                if (collisionDetected)
                    break;
            }

            if (collisionDetected)
            {
                Ship.SetIsColliding(true);
                Ship.SetLastCollisionTimestamp(Loop.Clock);
                Collide();
            }
            else
            {
                Ship.SetIsColliding(false);
            }
        }

        private void Collide()
        {
            _collisionSequence.Collide();
        }

        public void Brake()
        {
            _brakeSequence.Brake(Loop.Elapsed);
        }

        public void Burn()
        {
            Ship.SetAccel(BurnAcceleration);
        }

        public void Explode()
        {
            _explosionSequence.Explode(Loop.Elapsed);
        }

        public void Fire()
        {
            if (_lastFireTicks > 0L)
            {
                var overheating = Loop.Clock - _lastFireTicks < TimeSpan.TicksPerSecond / MaxShotsPerSecond;

                if (overheating)
                {
                    return;
                }
            }

            _lastFireTicks = Loop.Clock;

            Loop.BulletsController.Spawn(Ship.NosePositionInches, BulletSpeedIps * Ship.Heading);
        }

        public void HeadHome()
        {
            _headHomeSequence.HeadHome(Loop.Elapsed);
        }

        public void Meco()
        {
            Ship.SetAccel(0.0);
        }

        public void ReleaseBrake()
        {
            _brakeSequence.Reset();
        }

        public void RespawnBegin(TimeSpan duration)
        {
            _respawnSequence.BeginRespawn(duration);
        }

        public void RespawnContinue()
        {
            _respawnSequence.RespawnContinue(Loop.Elapsed);
        }

        public void SpinLeft()
        {
            Ship.SetHeadingAngle(Ship.HeadingAngle - Angle.FromDegrees(Loop.Elapsed.TotalMinutes * SpinSpeedRpm * 360.0));
        }

        public void SpinRight()
        {
            Ship.SetHeadingAngle(Ship.HeadingAngle + Angle.FromDegrees(Loop.Elapsed.TotalMinutes * SpinSpeedRpm * 360.0));
        }

        public void SetHeadingAngle(Angle angle)
        {
            Ship.SetHeadingAngle(angle);
        }

        public void SetVelocityIps(Vector2D targetVelocity)
        {
            Ship.SetVelocityIps(targetVelocity);
        }

        public override void Restart()
        {
            Ship.SetStatus(ShipStatus.Dead);
            Ship.SetStatusSequence(0);
            Ship.SetDiameterInches(AsteroidsLoop.ShipDiameterInches);
            Ship.SetPositionInches(Vector2D.Zero);
            Ship.SetVelocityIps(Vector2D.Zero);
            Ship.SetHeadingAngle(Angle.Zero);
            Ship.SetAccel(0.0);
        }
    }
}
