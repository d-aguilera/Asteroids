namespace Asteroids.Core
{
    using System.Collections.Generic;
    using GameLoop;

    public class AsteroidsLoop : GameLoopBase<IGameState>, IAsteroidsLoop
    {
        public const bool CollisionDetection = true;
        public const double ViewportWidthInches = 20.0;
        public const double ViewportHeightInches = 12.0;
        public const double ShipDiameterInches = 170.0 / 96.0;
        public const double ExplosionDiameterInches = 170.0 / 96.0;
        public const double BulletDiameterInches = 16.0 / 96.0;

        private readonly GameState _state;
        private readonly IAsteroidsController _asteroidsController;
        private readonly IBulletsController _bulletsController;
        private readonly IShipController _shipController;

        public AsteroidsLoop()
        {
            _state = new GameState(Ship.Create());
            _asteroidsController = new AsteroidsController(this, new Randomizer());
            _bulletsController = new BulletsController(this);
            _shipController = new ShipController(this);

            RestartInternal();
        }

        internal AsteroidsLoop(
            IAsteroidsController asteroidsController,
            IBulletsController bulletsController,
            IShipController shipController)
        {
            _state = new GameState(Ship.Create());
            _asteroidsController = asteroidsController;
            _bulletsController = bulletsController;
            _shipController = shipController;

            RestartInternal();
        }

        // ReSharper disable once ConvertToAutoProperty
        public override IGameState State => _state;

        protected override void HandleCollisions()
        {
            _asteroidsController.HandleCollisions();
            _bulletsController.HandleCollisions();
            _shipController.HandleCollisions();
        }

        protected override void HandleInput()
        {
            if (IsKeyDown(LoopKeys.R))
            {
                Restart();
                return;
            }

            _asteroidsController.HandleInput();
            _bulletsController.HandleInput();
            _shipController.HandleInput();
        }

        protected override void Restart()
        {
            RestartInternal();
        }

        protected override void UpdateModel()
        {
            _asteroidsController.UpdateModel();
            _bulletsController.UpdateModel();
            _shipController.UpdateModel();
        }

        private void RestartInternal()
        {
            _asteroidsController.Restart();
            _bulletsController.Restart();
            _shipController.Restart();
        }

        #region IAsteroidsLoop
        IAsteroidsController IAsteroidsLoop.AsteroidsController => _asteroidsController;
        IBulletsController IAsteroidsLoop.BulletsController => _bulletsController;

        bool IAsteroidsLoop.IsAnyKeyDown(IEnumerable<LoopKeys> keyCodes) => IsAnyKeyDown(keyCodes);
        #endregion
    }
}
