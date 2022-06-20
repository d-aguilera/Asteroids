namespace Asteroids.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Drawing;
    using System.Numerics;

    internal class AsteroidsController : ControllerBase, IAsteroidsController
    {
        private const int KeyLimit = 1024;

        private const double MinSizeInches = 1.0;
        private const double MaxSizeInches = 2.0;
        private const double HalfMaxSizeInches = MaxSizeInches / 2.0;
        private const double HalfWidthInches = AsteroidsLoop.ViewportWidthInches / 2.0;
        private const double HalfHeightInches = AsteroidsLoop.ViewportHeightInches / 2.0;
        private const double OriginTopInches = 0.0 - HalfHeightInches - HalfMaxSizeInches;
        private const double OriginRightInches = 0.0 + HalfWidthInches + HalfMaxSizeInches;
        private const double OriginBottomInches = 0.0 + HalfHeightInches + HalfMaxSizeInches;
        private const double OriginLeftInches = 0.0 - HalfWidthInches - HalfMaxSizeInches;

        private static readonly RectangleD InnerViewPort = new(
            -HalfWidthInches,
            -HalfHeightInches,
            AsteroidsLoop.ViewportWidthInches,
            AsteroidsLoop.ViewportHeightInches);

        private static readonly RectangleD InflatedViewPort = RectangleD.Inflate(InnerViewPort, HalfMaxSizeInches, HalfMaxSizeInches);
        private static readonly RectangleD OuterViewPort = RectangleD.Inflate(InnerViewPort, 3.0 * MaxSizeInches, 3.0 * MaxSizeInches);

        private static readonly (double, double, double, double, double)[] OriginInfos =
        {
            (0.0, -1.0, OriginTopInches, 0.0, 0.0),
            (0.0, +1.0, OriginTopInches, 0.0, 90.0),
            (OriginRightInches, 0.0, 0.0, -1.0, 90.0),
            (OriginRightInches, 0.0, 0.0, +1.0, 180.0),
            (0.0, +1.0, OriginBottomInches, 0.0, 180.0),
            (0.0, -1.0, OriginBottomInches, 0.0, 270.0),
            (OriginLeftInches, 0.0, 0.0, +1.0, 270.0),
            (OriginLeftInches, 0.0, 0.0, -1.0, 0.0),
        };

        private readonly List<IAsteroidInternal> _astray = new();
        private readonly List<IAsteroidInternal> _exploded = new();

        // A hitting B is the same as B hitting A, so
        // register handled collisions here to avoid
        // processing the same hit twice
        private readonly HashSet<int> _handledCollisions = new();

        private readonly IRandom _random;

        private long _clock;
        private int _nextKey;

        private List<IAsteroidInternal> Asteroids => (List<IAsteroidInternal>)Loop.State.Asteroids;

        public AsteroidsController(IAsteroidsLoop gameLoop, IRandom random) : base(gameLoop)
        {
            _random = random;
            _nextKey = 1;
        }

        public void AddAsteroid(IAsteroid asteroid)
        {
            Asteroids.Add((IAsteroidInternal)asteroid);
        }

        public void Damage(IAsteroid asteroid)
        {
            ((IAsteroidInternal)asteroid).SetHealthPercent(asteroid.HealthPercent - 0.5);
        }

        public override void HandleInput()
        {
        }

        public override void HandleCollisions()
        {
            _handledCollisions.Clear();

            foreach (var one in Asteroids)
            {
                var key1 = one.Key;

                foreach (var two in Asteroids)
                {
                    var key2 = two.Key;

                    if (key2 == key1)
                    {
                        continue;
                    }

                    if (two.ParentKey != 0 && two.ParentKey == one.ParentKey)
                    {
                        // siblings don't hit each other
                        continue;
                    }

                    var collisionId = key1 < key2 ? key1 * KeyLimit + key2 : key2 * KeyLimit + key1;
                    if (_handledCollisions.Contains(collisionId))
                    {
                        continue;
                    }

                    _handledCollisions.Add(collisionId);

                    var distanceBetween = DistanceBetween(one, two);
                    if (distanceBetween > 0.0)
                    {
                        continue;
                    }

                    var breakApartTime = 0.0;
                    if (distanceBetween < 0.0)
                    {
                        breakApartTime = ApproximateBreakApartTime(one, two);
                        one.SetPositionInches(one.PositionInches + breakApartTime * one.VelocityIps);
                        two.SetPositionInches(two.PositionInches + breakApartTime * two.VelocityIps);
                    }

                    // apply elastic collision formulas to get new velocities
                    // https://en.wikipedia.org/wiki/Elastic_collision
                    // https://www.vobarian.com/collisions/2dcollisions2.pdf
                    var n = two.PositionInches - one.PositionInches;
                    var uNormal = Vector2D.Normalize(n);
                    var uTan = new Vector2D(-uNormal.Y, uNormal.X);
                    var v1Normal = Vector2D.Dot(uNormal, one.VelocityIps);
                    var v1Tan = Vector2D.Dot(uTan, one.VelocityIps);
                    var v2Normal = Vector2D.Dot(uNormal, two.VelocityIps);
                    var v2Tan = Vector2D.Dot(uTan, two.VelocityIps);
                    var v1TanPrime = uTan * v1Tan;
                    var v2TanPrime = uTan * v2Tan;
                    var totalMass = one.Mass + two.Mass;
                    var massDiff = one.Mass - two.Mass;
                    var v1NormalPrime = uNormal * ((+v1Normal * massDiff + 2.0 * two.Mass * v2Normal) / totalMass);
                    var v2NormalPrime = uNormal * ((-v2Normal * massDiff + 2.0 * one.Mass * v1Normal) / totalMass);
                    var v1Prime = v1NormalPrime + v1TanPrime;
                    var v2Prime = v2NormalPrime + v2TanPrime;

                    // set new velocities
                    one.SetVelocityIps(v1Prime);
                    two.SetVelocityIps(v2Prime);

                    // ensure separation
                    var elapsedSeconds = Loop.Elapsed.TotalSeconds + breakApartTime;
                    do
                    {
                        one.SetPositionInches(one.PositionInches + v1Prime * elapsedSeconds);
                        two.SetPositionInches(two.PositionInches + v2Prime * elapsedSeconds);
                    }
                    while (DistanceBetween(one, two) < 0.0);
                }
            }
        }

        public override void Restart()
        {
            Asteroids.Clear();
            _clock = 0L;
        }

        public override void UpdateModel()
        {
            _clock += Loop.Elapsed.Ticks;

            // spawn new asteroids periodically
            if (_clock >= 500L * TimeSpan.TicksPerMillisecond)
            {
                _clock = 0L;
                Spawn();
            }

            _astray.Clear();
            _exploded.Clear();

            foreach (var asteroid in Asteroids)
            {
                // move & rotate asteroid
                asteroid.UpdateModel(Loop.Elapsed);

                if (asteroid.HealthPercent <= 0.0)
                {
                    _exploded.Add(asteroid);
                }
                else if (IsNoLongerVisible(asteroid) || IsBeyondOuterViewPort(asteroid))
                {
                    _astray.Add(asteroid);
                }
            }

            // process asteroids marked for deletion
            foreach (var asteroid in _astray)
            {
                Asteroids.Remove(asteroid);
            }

            // process asteroids marked for explosion
            foreach (var asteroid in _exploded)
            {
                Explode(asteroid);
            }
        }

        private static bool IsBeyondInflatedViewPort(ISprite sprite)
        {
            return !InflatedViewPort.Contains(sprite.PositionInches.X, sprite.PositionInches.Y);
        }

        private static bool IsNoLongerVisible(IAsteroidInternal traveler)
        {
            // mark asteroid as ever visible if it entered the inner viewport
            if (!traveler.WasEverVisible && InnerViewPort.Contains(traveler.PositionInches.X, traveler.PositionInches.Y))
            {
                traveler.WasEverVisible = true;
            }

            return traveler.WasEverVisible && IsBeyondInflatedViewPort(traveler);
        }

        private static bool IsBeyondOuterViewPort(ISprite sprite)
        {
            return !OuterViewPort.Contains(sprite.PositionInches.X, sprite.PositionInches.Y);
        }

        private static double ApproximateBreakApartTime(IRadialSpriteWithHitBox asteroid1, IRadialSpriteWithHitBox asteroid2)
        {
            var p = asteroid1.PositionInches - asteroid2.PositionInches;
            var v = asteroid1.VelocityIps - asteroid2.VelocityIps;
            var r = asteroid1.HitRadiusInches + asteroid2.HitRadiusInches;
            var d = Math.Sqrt(1.0 * p.X * p.X + 1.0 * p.Y * p.Y) - r;
            var s = Math.Sqrt(1.0 * v.X * v.X + 1.0 * v.Y * v.Y);
            var t = d / s;

            return t;
        }

        private static double DistanceBetween(IRadialSpriteWithHitBox one, IRadialSpriteWithHitBox two)
        {
            var centerToCenterDistance = (two.PositionInches - one.PositionInches).Length();
            var hitDistance = one.HitRadiusInches + two.HitRadiusInches;
            var distance = centerToCenterDistance - hitDistance;
            return distance;
        }

        private void Explode(IAsteroidInternal asteroid)
        {
            Asteroids.Remove(asteroid);

            if (asteroid.DiameterInches <= MinSizeInches)
            {
                return;
            }

            var pieces = asteroid.DiameterInches >= 1.75 ? 4 : 2;
            var diameter = asteroid.DiameterInches * 1.5 / pieces;
            var speed = asteroid.VelocityIps.Length() * 1.25;

            var eachAngle = Angle.FromDegrees(60.0);
            var aperture = eachAngle * (pieces - 1);
            var initialAngle = asteroid.CourseAngle - aperture / 2.0;

            for (var i = 0; i < pieces; i += 1)
            {
                var angle = initialAngle + i * eachAngle;
                var velocity = speed * angle.ToVector();

                var asteroid1 = Asteroid.Create(GetNextKey(), asteroid.Type);

                asteroid1.SetDiameterInches(diameter);
                asteroid1.SetHeadingAngle(asteroid.HeadingAngle);
                asteroid1.SetHealthPercent(1.0);
                asteroid1.SetHitDiameterInches(diameter * 0.9);
                asteroid1.SetParentKey(asteroid.Key);
                asteroid1.SetPositionInches(asteroid.PositionInches);
                asteroid1.SetRotationSpeedRpm(asteroid.RotationSpeedRpm * 2.0);
                asteroid1.SetVelocityIps(velocity);

                AddAsteroid(asteroid1);
            }
        }

        private int GetNextKey()
        {
            var key = _nextKey;

            _nextKey = (_nextKey + 1) % KeyLimit;

            return key;
        }

        private void Spawn()
        {
            // get random values
            var randomForType = _random.Next(0, 10);
            var randomForDiameter = _random.NextDouble();
            var randomForRotates = _random.NextDouble();
            var randomForHeadingAngle = _random.Next(0, 360);
            var randomForRotationSpeedRpm = _random.NextDouble();

            // randomize asteroid type, size and rotation
            var diameter = MinSizeInches + (MaxSizeInches - MinSizeInches) * randomForDiameter;
            var hitDiameter = diameter * 0.9;
            var rotates = randomForRotates < 0.1;
            var headingAngle = Angle.FromDegrees(randomForHeadingAngle);
            var rotationSpeedRpm = rotates ? 180.0 * randomForRotationSpeedRpm - 90.0 : 0.0;

            var asteroid = Asteroid.Create(GetNextKey(), randomForType);

            asteroid.SetDiameterInches(diameter);
            asteroid.SetHeadingAngle(headingAngle);
            asteroid.SetHealthPercent(1.0);
            asteroid.SetHitDiameterInches(hitDiameter);
            asteroid.SetRotationSpeedRpm(rotationSpeedRpm);

            bool collides;

            var attempts = 0;
            do
            {
                var randomForOrigin = _random.Next(0, 8);
                var randomForPositionX = _random.NextDouble();
                var randomForPositionY = _random.NextDouble();
                var randomForAngle = _random.NextDouble();
                var randomForSpeed = _random.NextDouble();

                /* randomize which edge of the screen it will appear from (initial position)
                 * there are 8 possible origins (like the edges of a rectangular pizza)
                 * 0 = top edge, left half
                 * 1 = top edge, right half
                 * 2 = right edge, top half
                 * 3 = right edge, bottom half
                 * 4 = bottom edge, right half
                 * 5 = bottom edge, left half
                 * 6 = left edge, bottom half
                 * 7 = left edge, top half
                 */
                var (offsetX, scaleX, offsetY, scaleY, baseAngle) = OriginInfos[randomForOrigin];
                var x = HalfWidthInches * randomForPositionX;
                var y = HalfHeightInches * randomForPositionY;
                var position = new Vector2D(offsetX + scaleX * x, offsetY + scaleY * y);

                // randomize angle and speed, but head towards the center
                var angle = baseAngle + 15.0 + 60.0 * randomForAngle;
                var angleRad = angle * Math.PI / 180.0;
                var speed = 0.5 + randomForSpeed * 2.5;
                var velocity = speed * new Vector2D(Math.Cos(angleRad), Math.Sin(angleRad));

                // move it 2 seconds backwards
                position -= 2.0 * velocity;

                asteroid.SetPositionInches(position);
                asteroid.SetVelocityIps(velocity);

                collides = Asteroids.Any(other => DistanceBetween(asteroid, other) < 0.5);
            }
            while (collides && ++attempts < 5);

            if (collides)
            {
                return;
            }

            AddAsteroid(asteroid);
        }
    }
}
