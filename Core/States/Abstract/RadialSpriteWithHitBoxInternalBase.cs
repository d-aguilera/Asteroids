namespace Asteroids.Core
{
    internal abstract class RadialSpriteWithHitBoxInternalBase : RadialSpriteInternalBase, IRadialSpriteWithHitBoxInternal
    {
        public double HitDiameterInches { get; private set; }
        public double HitRadiusInches { get; private set; }

        public void SetHitDiameterInches(double value)
        {
            HitDiameterInches = value;
            HitRadiusInches = value / 2.0;
        }

        public override void CopyTo(ISpriteInternal target)
        {
            base.CopyTo(target);

            var radialWithHitBox = (IRadialSpriteWithHitBoxInternal)target;
            radialWithHitBox.SetHitDiameterInches(HitDiameterInches);
        }
    }
}
