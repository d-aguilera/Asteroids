namespace Asteroids.WinFormsApp
{
    using System.Diagnostics;
    using System.Threading;

    internal class TimeCounter : Counter, ITimeCounter
    {
        private long _start;

        public TimeCounter()
        {
        }

        public TimeCounter(long initialValue) : base(initialValue)
        {
        }

        public void Start()
        {
            Interlocked.Exchange(ref _start, Stopwatch.GetTimestamp());
        }

        public void Stop()
        {
            Interlocked.Add(ref Count, Stopwatch.GetTimestamp() - _start);
        }
    }
}
