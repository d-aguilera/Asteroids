namespace Asteroids.GameLoop
{
    using System;
    using System.Collections.Generic;

    public abstract class GameLoopBase<TState> : IGameLoopBase<TState> where TState : class
    {
        private readonly long _ticksInOneSecond = TimeSpan.FromSeconds(1.0).Ticks;
        private readonly object _keySync = new();
        private readonly HashSet<LoopKeys> _keyInfo = new();

        private long _lastFpsUpdate;
        private long _nextFpsUpdate;
        private long _frameCount;

        public long Clock { get; private set; }
        public TimeSpan Elapsed { get; private set; }
        public double CurrentFps { get; private set; }
        public abstract TState State { get; }
        public object StateSync { get; } = new();

        public void RegisterKeyEvent(LoopKeyEventTypes type, LoopKeyEventArgs e)
        {
            switch (type)
            {
                case LoopKeyEventTypes.KeyDown:
                    lock (_keySync)
                    {
                        _keyInfo.Add(e.KeyCode);
                    }

                    break;

                case LoopKeyEventTypes.KeyUp:
                    lock (_keySync)
                    {
                        _keyInfo.Remove(e.KeyCode);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public void Tick(long clock)
        {
            UpdateClock(clock);
            HandleInput();
            UpdateModel();
            HandleCollisions();
            UpdateFps();
        }

        protected abstract void HandleCollisions();

        protected abstract void HandleInput();

        protected bool IsKeyDown(LoopKeys keyCode)
        {
            lock (_keySync)
            {
                return _keyInfo.Contains(keyCode);
            }
        }

        protected bool IsAnyKeyDown(IEnumerable<LoopKeys> keyCodes)
        {
            lock (_keySync)
            {
                return _keyInfo.Overlaps(keyCodes);
            }
        }

        protected abstract void UpdateModel();

        protected abstract void Restart();

        private void UpdateClock(long clock)
        {
            Elapsed = TimeSpan.FromTicks(clock - Clock);
            Clock = clock;
        }

        private void UpdateFps()
        {
            _frameCount++;

            if (Clock < _nextFpsUpdate) return;

            CurrentFps = _frameCount / TimeSpan.FromTicks(Clock - _lastFpsUpdate).TotalSeconds;

            _lastFpsUpdate = Clock;
            _nextFpsUpdate = Clock + _ticksInOneSecond;
            _frameCount = 0;
        }
    }
}
