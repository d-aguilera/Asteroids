namespace Asteroids.Core
{
    internal interface IController
    {
        void HandleCollisions();

        void HandleInput();

        void Restart();

        void UpdateModel();
    }
}
