namespace Asteroids.Core
{
    using System;
    using System.Numerics;

    internal abstract class SpriteInternalBase : ISpriteInternal
    {
        public Vector2D Course { get; private set; }
        public Angle CourseAngle { get; private set; }
        public Vector2D Heading { get; private set; }
        public Angle HeadingAngle { get; private set; }
        public Vector2D PositionInches { get; private set; }
        public Vector2D VelocityIps { get; private set; }

        public virtual void CopyTo(ISpriteInternal target)
        {
            target.SetCourseAngle(CourseAngle);
            target.SetHeadingAngle(HeadingAngle);
            target.SetPositionInches(PositionInches);
            target.SetVelocityIps(VelocityIps);
        }

        public void SetCourseAngle(Angle value)
        {
            CourseAngle = value;
            Course = CourseAngle.ToVector();
            VelocityIps = VelocityIps.Length() * Course;
        }

        public virtual void SetHeadingAngle(Angle value)
        {
            HeadingAngle = value;
            Heading = HeadingAngle.ToVector();
        }

        public virtual void SetPositionInches(Vector2D value)
        {
            PositionInches = value;
        }

        public void SetVelocityIps(Vector2D value)
        {
            VelocityIps = value;
            CourseAngle = Angle.FromVector(value);
            Course = CourseAngle.ToVector();
        }

        public virtual void UpdateModel(TimeSpan elapsed)
        {
            // move sprite
            SetPositionInches(PositionInches + VelocityIps * elapsed.TotalSeconds);
        }
    }
}
