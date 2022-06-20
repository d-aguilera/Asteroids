namespace Asteroids.Core
{
    public interface IRandom
    {
        int Next(int minValue, int maxValue);
        double NextDouble();
    }
}
