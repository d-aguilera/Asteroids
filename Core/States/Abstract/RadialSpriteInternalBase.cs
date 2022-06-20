namespace Asteroids.Core
{
    using System;

    internal abstract class RadialSpriteInternalBase : SpriteInternalBase, IRadialSpriteInternal
    {
        public double DiameterInches { get; private set; }
        public double Mass { get; private set; }
        public double RadiusInches { get; private set; }

        public void SetDiameterInches(double value)
        {
            DiameterInches = value;
            RadiusInches = value / 2.0;
            Mass = Math.PI * RadiusInches * RadiusInches;
        }

        public override void CopyTo(ISpriteInternal target)
        {
            base.CopyTo(target);

            var local = (IRadialSpriteInternal)target;
            local.SetDiameterInches(DiameterInches);
        }
    }
}
