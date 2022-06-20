using System;
using System.Collections.Generic;
using Asteroids.GameLoop;

namespace Asteroids.Core.Tests
{
    internal class TestGameLoop : IAsteroidsLoop
    {
        public IAsteroidsController AsteroidsController => throw new NotImplementedException();
        public IBulletsController BulletsController => throw new NotImplementedException();
        public long Clock { get; private set; }
        public TimeSpan Elapsed { get; private set; }
        public IGameState State => throw new NotImplementedException();

        public bool IsAnyKeyDown(IEnumerable<LoopKeys> keyCodes) => throw new NotImplementedException();

        public void Tick(TimeSpan elapsed)
        {
            Elapsed = elapsed;
            Clock += elapsed.Ticks;
        }
    }
}
