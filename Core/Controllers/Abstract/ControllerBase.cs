namespace Asteroids.Core
{
    using System.Collections.Generic;
    using GameLoop;

    internal abstract class ControllerBase : IController
    {
        protected ControllerBase(IAsteroidsLoop loop)
        {
            Loop = loop;
        }

        protected IAsteroidsLoop Loop { get; }

        public abstract void HandleCollisions();

        public abstract void HandleInput();

        public abstract void Restart();

        public abstract void UpdateModel();

        protected bool IsKeyDown(IEnumerable<LoopKeys> keyCodes) => Loop.IsAnyKeyDown(keyCodes);
    }
}
