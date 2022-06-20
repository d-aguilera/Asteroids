using Moq;
using NUnit.Framework;

namespace Asteroids.Core.Tests
{
    internal class GameLoopTests
    {
        [Test]
        public void GetState_WhenInvoked_CallsControllersGetState()
        {
            // arrange
            var asteroidsController = new Mock<IAsteroidsController>();
            asteroidsController.Setup(x => x.UpdateSnapshot()).Verifiable();

            var bulletsController = new Mock<IBulletsController>();
            bulletsController.Setup(x => x.UpdateSnapshot()).Verifiable();

            var shipController = new Mock<IShipController>();
            shipController.Setup(x => x.UpdateSnapshot()).Verifiable();

            var sut = new AsteroidsLoop(
                asteroidsController.Object,
                bulletsController.Object,
                shipController.Object);

            // act
            sut.TakeSnapshot();

            // assert
            Assert.IsNotNull(sut.State);
            asteroidsController.Verify();
            bulletsController.Verify();
            shipController.Verify();
        }

        [Test]
        public void Tick_WhenInvoked_CallsUpdateClock()
        {
            // arrange
            var sut = new AsteroidsLoop();

            // act
            sut.Tick(100L);
            sut.UpdateSnapshot();

            // assert
            Assert.AreEqual(100L, sut.Snapshot.Clock);
        }

        [Test]
        public void Tick_WhenInvoked_CallsHandleCollisions()
        {
            // arrange
            var asteroidsController = new Mock<IAsteroidsController>();
            asteroidsController.Setup(x => x.HandleCollisions()).Verifiable();

            var bulletsController = new Mock<IBulletsController>();
            bulletsController.Setup(x => x.HandleCollisions()).Verifiable();

            var shipController = new Mock<IShipController>();
            shipController.Setup(x => x.HandleCollisions()).Verifiable();

            var sut = new AsteroidsLoop(
                asteroidsController.Object,
                bulletsController.Object,
                shipController.Object);

            // act
            sut.Tick(0L);

            // assert
            asteroidsController.Verify();
            bulletsController.Verify();
            shipController.Verify();
        }

    }
}
