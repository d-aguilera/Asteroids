namespace Asteroids.Core
{
    using System;
    using System.Numerics;

    internal class Ship : RadialSpriteInternalBase, IShipInternal
    {
        private static readonly Matrix3x2D HitBoxScale = Matrix3x2D.CreateScale(1.0 / 16.0);
        private static readonly Vector2D[] HitBox =
        {
            // bow
            new(+14.0, +0.0),

            // starboard side
            new(+12.0, +2.0),
            new(+06.0, +4.0),
            new(+00.0, +4.0),
            new(-01.0, +6.0),
            new(-08.0, +5.0),

            // port side
            new(-08.0, -5.0),
            new(-01.0, -6.0),
            new(+00.0, -4.0),
            new(+06.0, -4.0),
            new(+12.0, -2.0),

            // bow
            new (+14.0, +0.0),
        };

        private Matrix3x2D _hitBoxRotation;

        private Ship()
        {
        }

        public double Accel { get; private set; }
        public double HealthPercent { get; private set; }
        public Vector2D[] HitBoxInches { get; } = new Vector2D[HitBox.Length];
        public bool IsColliding { get; private set; }
        public bool IsVisible { get; private set; } = true;
        public long LastCollisionTimestamp { get; private set; }
        public Vector2D NosePositionInches { get; private set; }
        public ShipStatus Status { get; private set; }
        public int StatusSequence { get; private set; }

        public static IShipInternal Create()
        {
            return new Ship
            {
                Status = ShipStatus.Dead,
            };
        }

        public IShip Clone()
        {
            var clone = Create();
            CopyTo(clone);
            return clone;
        }

        public override void CopyTo(ISpriteInternal target)
        {
            base.CopyTo(target);

            var local = (IShipInternal)target;
            local.SetAccel(Accel);
            local.SetHeadingAngle(HeadingAngle);
            local.SetHealthPercent(HealthPercent);
            local.SetIsColliding(IsColliding);
            local.SetIsVisible(IsVisible);
            local.SetLastCollisionTimestamp(LastCollisionTimestamp);
            local.SetPositionInches(PositionInches);
            local.SetStatus(Status);
            local.SetStatusSequence(StatusSequence);
        }

        public void SetAccel(double value)
        {
            Accel = value;
        }

        public override void SetHeadingAngle(Angle value)
        {
            base.SetHeadingAngle(value);

            _hitBoxRotation = Matrix3x2D.CreateRotation(HeadingAngle.Radians);
        }

        public void SetHealthPercent(double value)
        {
            HealthPercent = value;
        }

        public void SetIsColliding(bool value)
        {
            IsColliding = value;
        }

        public void SetIsVisible(bool value)
        {
            IsVisible = value;
        }

        public void SetLastCollisionTimestamp(long value)
        {
            LastCollisionTimestamp = value;
        }

        public override void SetPositionInches(Vector2D value)
        {
            base.SetPositionInches(value);

            var translation = Matrix3x2D.CreateTranslation(PositionInches);

            for (var i = 0; i < HitBox.Length; i++)
            {
                HitBoxInches[i] =
                    Vector2D.Transform(
                        Vector2D.Transform(
                            Vector2D.Transform(
                                HitBox[i],
                                _hitBoxRotation),
                            HitBoxScale),
                        translation);
            }

            NosePositionInches = HitBoxInches[0];
        }

        public void SetStatus(ShipStatus value)
        {
            Status = value;
        }

        public void SetStatusSequence(int value)
        {
            StatusSequence = value;
        }

        public override void UpdateModel(TimeSpan elapsed)
        {
            SetVelocityIps(VelocityIps + Accel * elapsed.TotalSeconds * Heading);
            base.UpdateModel(elapsed);
        }
    }
}
