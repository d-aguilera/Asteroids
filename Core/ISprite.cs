namespace Asteroids.Core
{
    using System.Numerics;

    public interface ISprite
    {
        Vector2D Course { get; }
        Angle CourseAngle { get; }
        Vector2D Heading { get; }
        Angle HeadingAngle { get; }
        Vector2D PositionInches { get; }
        Vector2D VelocityIps { get; }
    }
}
