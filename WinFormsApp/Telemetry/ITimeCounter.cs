namespace Asteroids.WinFormsApp
{
    internal interface ITimeCounter : ICounter
    {
        void Start();
        void Stop();
    }
}
