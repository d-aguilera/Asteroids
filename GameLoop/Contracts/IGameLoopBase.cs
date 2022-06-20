namespace Asteroids.GameLoop
{
    using System;

    public interface IGameLoopBase<out TState> where TState : class
    {
        long Clock { get; }
        double CurrentFps { get; }
        TimeSpan Elapsed { get; }
        TState State { get; }
        object StateSync { get; }

        void RegisterKeyEvent(LoopKeyEventTypes type, LoopKeyEventArgs e);
        void Tick(long clock);
    }
}
