namespace Asteroids.Core
{
    using System;
    using System.Numerics;

    internal class Asteroid : RadialSpriteWithHitBoxInternalBase, IAsteroidInternal
    {
        private Asteroid(int key, int type)
        {
            Key = key;
            Type = type;
        }

        public double HealthPercent { get; private set; }
        public int Key { get; }
        public int ParentKey { get; private set; }
        public double RotationSpeedRpm { get; private set; }
        public int Type { get; }
        public bool WasEverVisible { get; set; }

        public static IAsteroidInternal Create(int key, int type)
        {
            return new Asteroid(key, type);
        }

        public IAsteroid Clone()
        {
            var clone = Create(Key, Type);
            CopyTo(clone);
            return clone;
        }

        public override void CopyTo(ISpriteInternal target)
        {
            base.CopyTo(target);

            var local = (IAsteroidInternal)target;
            local.SetHealthPercent(HealthPercent);
            local.SetParentKey(ParentKey);
            local.SetRotationSpeedRpm(RotationSpeedRpm);
        }

        public void SetHealthPercent(double value)
        {
            HealthPercent = value;
        }

        public void SetParentKey(int value)
        {
            ParentKey = value;
        }

        public void SetRotationSpeedRpm(double value)
        {
            RotationSpeedRpm = value;
        }

        public override void UpdateModel(TimeSpan elapsed)
        {
            // move asteroid
            base.UpdateModel(elapsed);

            if (RotationSpeedRpm == 0.0) return;

            // rotate asteroid
            SetHeadingAngle(HeadingAngle + Angle.FromDegrees(elapsed.TotalMinutes * RotationSpeedRpm * 360.0));
        }
    }
}
