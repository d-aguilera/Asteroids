namespace Asteroids.Core
{
    using System;
    using System.Numerics;

    internal interface ISpriteInternal : ISprite
    {
        void CopyTo(ISpriteInternal target);
        void SetCourseAngle(Angle value);
        void SetHeadingAngle(Angle value);
        void SetPositionInches(Vector2D value);
        void SetVelocityIps(Vector2D value);
        void UpdateModel(TimeSpan elapsed);
    }
}
