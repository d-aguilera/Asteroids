namespace Asteroids.Core
{
    using System;

    internal class Randomizer : IRandom
    {
        private readonly Random _random;

        public Randomizer()
        {
            _random = new Random();
        }

        public int Next(int minValue, int maxValue) => _random.Next(minValue, maxValue);

        public double NextDouble() => _random.NextDouble();
    }
}
