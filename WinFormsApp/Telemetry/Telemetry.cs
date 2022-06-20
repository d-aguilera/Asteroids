namespace Asteroids.WinFormsApp
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    internal class Telemetry
    {
        private readonly ICounter _ticks = new Counter();
        private readonly ITimeCounter _total = new TimeCounter();
        private readonly IDictionary<string, ITimeCounter> _timeCounters = new Dictionary<string, ITimeCounter>();

        private long _lastSnapshot;

        public Telemetry() : this(Stopwatch.GetTimestamp())
        {
        }

        internal Telemetry(long now)
        {
            _lastSnapshot = now;
        }

        public TimeSpan GetAverage(string timeCounterName)
        {
            return TimeSpan.FromTicks(_timeCounters[timeCounterName].Snapshot) / _ticks.Snapshot;
        }

        public void MeasureFrame<T>(string counterName, T context, Action<T> action)
        {
            Measure(counterName, context, action);
            _ticks.Add(1L);
        }

        public void Measure<T>(string counterName, T context, Action<T> action)
        {
            var counter = _timeCounters[counterName];

            counter.Start();
            action(context);
            counter.Stop();
        }

        public long ReadTickSnapshot()
        {
            return _ticks.Snapshot;
        }

        public void RegisterTimeCounter(string counterName)
        {
            _timeCounters.Add(counterName, new TimeCounter(0L));
        }

        public bool TakeSnapshotAndResetEvery(TimeSpan interval, long now, out TimeSpan elapsed)
        {
            elapsed = TimeSpan.FromTicks(now - _lastSnapshot);

            if (elapsed < interval)
            {
                return false;
            }

            foreach (var counterName in _timeCounters.Keys)
            {
                _timeCounters[counterName].TakeSnapshot();
            }

            _total.TakeSnapshot();
            _ticks.TakeSnapshot();

            _lastSnapshot = now;

            return true;
        }
    }
}
