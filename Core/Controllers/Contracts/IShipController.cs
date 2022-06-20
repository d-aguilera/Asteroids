using System;

namespace Asteroids.Core
{
    using System.Numerics;

    internal interface IShipController : IController
    {
        void Brake();

        void Burn();

        void Explode();

        void Fire();

        void HeadHome();

        void Meco();

        void ReleaseBrake();

        void RespawnBegin(TimeSpan duration);

        void RespawnContinue();

        void SetHeadingAngle(Angle angle);

        void SetVelocityIps(Vector2D velocityIps);

        void SpinLeft();

        void SpinRight();
    }
}
