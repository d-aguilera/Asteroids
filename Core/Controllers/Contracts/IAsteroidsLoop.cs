namespace Asteroids.Core
{
    using System.Collections.Generic;
    using GameLoop;

    internal interface IAsteroidsLoop : IGameLoopBase<IGameState>
    {
        IAsteroidsController AsteroidsController { get; }
        IBulletsController BulletsController { get; }

        bool IsAnyKeyDown(IEnumerable<LoopKeys> keyCodes);
    }
}
