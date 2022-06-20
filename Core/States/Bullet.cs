namespace Asteroids.Core
{
    using System;

    internal class Bullet : RadialSpriteWithHitBoxInternalBase, IBulletInternal
    {
        private Bullet(int key)
        {
            Key = key;
        }

        public double DistanceTraveledInches { get; private set; }
        public bool IsNew { get; set; } = true;
        public int Key { get; }

        public static IBulletInternal Create(int key)
        {
            return new Bullet(key);
        }

        public IBullet Clone()
        {
            var clone = Create(Key);
            CopyTo(clone);
            return clone;
        }

        public override void CopyTo(ISpriteInternal target)
        {
            base.CopyTo(target);

            var local = (IBulletInternal)target;
            local.SetDistanceTraveledInches(DistanceTraveledInches);
        }

        public void SetDistanceTraveledInches(double value)
        {
            DistanceTraveledInches = value;
        }

        public override void UpdateModel(TimeSpan elapsed)
        {
            var seconds = elapsed.TotalSeconds;
            var distance = VelocityIps * seconds;
            SetPositionInches(PositionInches + VelocityIps * seconds);
            SetDistanceTraveledInches(DistanceTraveledInches + distance.Length());
        }
    }
}
