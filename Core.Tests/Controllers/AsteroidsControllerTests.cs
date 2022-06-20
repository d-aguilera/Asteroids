using System;
using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace Asteroids.Core.Tests.Controllers
{

    [TestFixture]
    public class AsteroidsControllerTests
    {
        private TestGameLoop _gameLoop;

        [SetUp]
        public void SetUp()
        {
            _gameLoop = new TestGameLoop();
        }

        [Test]
        public void Damage_WhenInvoked_ReducesHealthBy50Percent()
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            var asteroid = Asteroid.Create(1, 1);
            asteroid.SetHealthPercent(1.0);

            // act
            sut.Damage(asteroid);

            // assert
            Assert.AreEqual(0.5, asteroid.HealthPercent);
        }

        [Test]
        public void HandleInput_WhenInvoked_ShouldNotThrow()
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            // act & assert
            sut.HandleInput();
        }

        [Test]
        public void UpdateModel_WhenElapsedGreaterThanOrEqualTo500_ShouldSpawnNewAsteroid()
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            // act
            _gameLoop.Tick(TimeSpan.FromMilliseconds(500.0));
            sut.UpdateModel();

            // assert
            Assert.AreEqual(1, sut.Asteroids.Count());
            Assert.AreEqual(1, sut.Asteroids.First().Key);
        }

        [Test]
        [TestCase(0.0, -06.0606601717798210, -8.0606601717798227)]
        [TestCase(1.0, +06.0606601717798210, -8.0606601717798227)]
        [TestCase(2.0, +12.0606601717798230, -4.0606601717798210)]
        [TestCase(3.0, +12.0606601717798230, +4.0606601717798210)]
        [TestCase(4.0, +06.0606601717798219, +8.0606601717798227)]
        [TestCase(5.0, -06.0606601717798210, +8.0606601717798227)]
        [TestCase(6.0, -12.0606601717798210, +4.0606601717798219)]
        [TestCase(7.0, -12.0606601717798230, -4.0606601717798210)]
        public void UpdateModel_WhenRespawning_AsteroidsShouldAppearFromAllEdges(
            double origin, double expectedX, double expectedY)
        {
            // arrange
            const double angleDegrees = 30.0;
            const double speed = 1.0;
            const double randomX = 0.5;
            const double randomY = 0.5;
            const double randomAngle = angleDegrees / 60.0;
            const double randomSpeed = (speed - 0.5) / 2.5;

            var randomOrigin = origin / 8.0;

            var random = new FakeRandom(new[]
            {
                0.0, 0.0, 0.0, 0.0, 0.0,
                randomOrigin,
                randomX,
                randomY,
                randomAngle,
                randomSpeed,
            });

            var sut = new AsteroidsController(_gameLoop, random);

            // act
            _gameLoop.Tick(TimeSpan.FromMilliseconds(500.0));
            sut.UpdateModel();

            // assert
            Assert.AreEqual(1, sut.Asteroids.Count());

            var asteroid = sut.Asteroids.First();
            Assert.AreEqual(expectedX, asteroid.PositionInches.X);
            Assert.AreEqual(expectedY, asteroid.PositionInches.Y);
        }

        [Test]
        public void UpdateModel_WhenNewAsteroidCollides_ShouldTryFiveTimesAndThenQuit()
        {
            // arrange
            var random = new FakeRandom(new[]
            {
                // first spawn
                // attempt #1, success
                0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9,
                // second spawn
                // attempt #1, fails
                0.0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9,
                /* attempt #2, fails  */ 0.5, 0.6, 0.7, 0.8, 0.9,
                /* attempt #3, fails  */ 0.5, 0.6, 0.7, 0.8, 0.9,
                /* attempt #4, fails  */ 0.5, 0.6, 0.7, 0.8, 0.9,
                /* attempt #5, fails  */ 0.5, 0.6, 0.7, 0.8, 0.9,
            });

            var sut = new AsteroidsController(_gameLoop, random);

            // act
            // first tick, spawn should succeed
            _gameLoop.Tick(TimeSpan.FromMilliseconds(500.0));
            sut.UpdateModel();
            // second tick, colliding spawn should fail silently
            _gameLoop.Tick(TimeSpan.FromMilliseconds(500.0));
            sut.UpdateModel();

            // assert
            Assert.AreEqual(1, sut.Asteroids.Count());
        }

        [Test]
        public void UpdateModel_WhenAsteroidRotates_ShouldUpdateHeadingAngle()
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            var asteroid1 = Asteroid.Create(1, 0);
            asteroid1.SetHealthPercent(1.0);
            asteroid1.SetHeadingAngle(Angle.Zero);
            asteroid1.SetRotationSpeedRpm(+60.0);
            sut.AddAsteroid(asteroid1);

            var asteroid2 = Asteroid.Create(2, 0);
            asteroid2.SetHealthPercent(1.0);
            asteroid2.SetHeadingAngle(Angle.Zero);
            asteroid2.SetRotationSpeedRpm(-60.0);
            sut.AddAsteroid(asteroid2);

            var asteroid3 = Asteroid.Create(3, 0);
            asteroid3.SetHealthPercent(1.0);
            asteroid3.SetHeadingAngle(Angle.FromDegrees(315.0));
            asteroid3.SetRotationSpeedRpm(+60.0);
            sut.AddAsteroid(asteroid3);

            var asteroid4 = Asteroid.Create(4, 0);
            asteroid4.SetHealthPercent(1.0);
            asteroid4.SetHeadingAngle(Angle.FromDegrees(45.0));
            asteroid4.SetRotationSpeedRpm(-60.0);
            sut.AddAsteroid(asteroid4);

            // act
            // first tick, spawn should succeed
            _gameLoop.Tick(TimeSpan.FromMilliseconds(250.0));
            sut.UpdateModel();

            // assert
            Assert.AreEqual(4, sut.Asteroids.Count());
            Assert.AreEqual(090.0, sut.Asteroids.Skip(0).First().HeadingAngle.Degrees);
            Assert.AreEqual(270.0, sut.Asteroids.Skip(1).First().HeadingAngle.Degrees);
            Assert.AreEqual(045.0, sut.Asteroids.Skip(2).First().HeadingAngle.Degrees);
            Assert.AreEqual(315.0, sut.Asteroids.Skip(3).First().HeadingAngle.Degrees);
        }

        [Test]
        public void UpdateModel_WhenAsteroidHeathIsZero_ShouldExplode()
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            var asteroid = Asteroid.Create(1, 0);
            asteroid.SetHealthPercent(0.0);
            sut.AddAsteroid(asteroid);

            // act
            sut.UpdateModel();

            // assert
            Assert.AreEqual(0, sut.Asteroids.Count());
        }

        [Test]
        [TestCase(1.750, 4)]
        [TestCase(1.749, 2)]
        public void UpdateModel_WhenAsteroidExplodes_ShouldBreakApart(double diameter, int expectedAsteroids)
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            var asteroid = Asteroid.Create(1, 0);
            asteroid.SetHealthPercent(0.0);
            asteroid.SetDiameterInches(diameter);
            sut.AddAsteroid(asteroid);

            // act
            sut.UpdateModel();

            // assert
            Assert.AreEqual(expectedAsteroids, sut.Asteroids.Count());
        }

        [Test]
        public void UpdateModel_WhenAsteroidAstrays_ShouldBeRemoved()
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            var asteroid = Asteroid.Create(1, 0);
            asteroid.SetHealthPercent(1.0);
            asteroid.SetVelocityIps(2000.0 * Vector2D.UnitX);
            sut.AddAsteroid(asteroid);

            // act
            // first tick
            _gameLoop.Tick(TimeSpan.FromMilliseconds(100.0));
            sut.UpdateModel();
            // second tick
            _gameLoop.Tick(TimeSpan.FromMilliseconds(100.0));
            sut.UpdateModel();

            // assert
            Assert.AreEqual(0, sut.Asteroids.Count());
        }

        [Test]
        public void HandleCollisions()
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            var position1 = Vector2D.Zero - 3.0 * Vector2D.UnitX;
            var velocity1 = +10.0 * Vector2D.UnitX;
            var asteroid1 = Asteroid.Create(11, 0);
            asteroid1.SetHealthPercent(1.0);
            asteroid1.SetDiameterInches(2.5);
            asteroid1.SetHitDiameterInches(asteroid1.DiameterInches);
            asteroid1.SetPositionInches(position1);
            asteroid1.SetVelocityIps(velocity1);
            sut.AddAsteroid(asteroid1);

            var position2 = Vector2D.Zero + 3.0 * Vector2D.UnitX;
            var velocity2 = -10.0 * Vector2D.UnitX;
            var asteroid2 = Asteroid.Create(12, 0);
            asteroid2.SetHealthPercent(1.0);
            asteroid2.SetDiameterInches(2.5);
            asteroid2.SetHitDiameterInches(asteroid2.DiameterInches);
            asteroid2.SetPositionInches(position2);
            asteroid2.SetVelocityIps(velocity2);
            sut.AddAsteroid(asteroid2);

            // act
            // first tick, get closer
            _gameLoop.Tick(TimeSpan.FromMilliseconds(100.0));
            sut.UpdateModel();
            sut.HandleCollisions();
            // second tick, hit & bounce
            _gameLoop.Tick(TimeSpan.FromMilliseconds(100.0));
            sut.UpdateModel();
            sut.HandleCollisions();
            // third tick, distantiate
            _gameLoop.Tick(TimeSpan.FromMilliseconds(100.0));
            sut.UpdateModel();
            sut.HandleCollisions();

            // assert
            Assert.AreEqual(2, sut.Asteroids.Count());
            // end positions after colliding and bouncing
            // are the same as starting positions
            Assert.AreEqual(position1, asteroid1.PositionInches);
            Assert.AreEqual(position2, asteroid2.PositionInches);
            // because the objects have exactly opposite velocities
            Assert.AreEqual(velocity2, asteroid1.VelocityIps);
            Assert.AreEqual(velocity1, asteroid2.VelocityIps);
        }

        [Test]
        public void Restart_WhenInvoked_ShouldRemoveAllAsteroids()
        {
            // arrange
            var random = new Randomizer();
            var sut = new AsteroidsController(_gameLoop, random);

            Enumerable.Range(0, 5)
                .ToList()
                .ForEach(i =>
                {
                    var asteroid = Asteroid.Create(i, 0);
                    asteroid.SetHealthPercent(1.0);
                    sut.AddAsteroid(asteroid);
                });

            // act
            sut.Restart();

            // assert
            Assert.AreEqual(0, sut.Asteroids.Count());
        }
    }
}
