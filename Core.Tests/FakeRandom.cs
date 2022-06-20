namespace Asteroids.Core.Tests
{
    using System;

    internal class FakeRandom : IRandom
    {
        private readonly double[] _sequence;

        private int _index;

        public FakeRandom(double[] sequence)
        {
            _sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));

            if (sequence.Length == 0)
            {
                throw new ArgumentException("Sequence is empty.", nameof(sequence));
            }
        }

        public int Next(int minValue, int maxValue)
        {
            if (maxValue <= minValue)
            {
                throw new ArgumentException("Invalid argument.", nameof(maxValue));
            }

            CheckIndex();

            return Convert.ToInt32(minValue + _sequence[_index++] * (maxValue - minValue));
        }

        public double NextDouble()
        {
            CheckIndex();

            return _sequence[_index++];
        }

        private void CheckIndex()
        {
            while (_index >= _sequence.Length)
            {
                _index -= _sequence.Length;
            }
        }
    }
}
