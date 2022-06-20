namespace Asteroids.WinFormsApp
{
    internal interface ICounter
    {
        long Snapshot { get; }
        long Add(long value);
        void TakeSnapshot();

        public static double operator / (ICounter value1, ICounter value2)
        {
            return 1.0 * value1.Snapshot / value2.Snapshot;
        }
    }
}
