namespace Asteroids.WinFormsApp
{
    using System.Threading;

    internal class Counter : ICounter
    {
        protected long Count;

        private long _snapshot;

        public Counter() : this(0L)
        {
        }

        internal Counter(long initialValue)
        {
            Count = initialValue;
        }

        public long Snapshot => Interlocked.Read(ref _snapshot);

        public long Add(long value) => Interlocked.Add(ref Count, value);

        public void TakeSnapshot()
        {
            Interlocked.Exchange(ref _snapshot, Interlocked.Exchange(ref Count, 0L));
        }
    }
}
