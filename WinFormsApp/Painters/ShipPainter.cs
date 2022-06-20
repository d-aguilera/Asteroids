namespace Asteroids.WinFormsApp
{
    using System;
    using System.Drawing;
    using Core;
    using GameLoop;
    using Properties;

    internal class ShipPainter : IPainter
    {
        private IGameLoopBase<IGameState> Loop { get; }
        private Settings Settings { get; }
        private Images Images { get; }

        public ShipPainter(IGameLoopBase<IGameState> loop, Settings settings, Images images)
        {
            Loop = loop;
            Settings = settings;
            Images = images;
        }

        public void Draw(GraphicsD g)
        {
            var state = Loop.State.Ship;

            if (state is null) return;

            if (state.Status == ShipStatus.Dead) return;

            if (state.Status == ShipStatus.Exploding)
            {
                DrawExplosion(g);
                return;
            }

            if (state.IsColliding)
            {
                if (SoundPlayers.Collision.Tag is null)
                {
                    SoundPlayers.Collision.Tag = state.LastCollisionTimestamp;

                    if (Settings.SoundEffects)
                    {
                        SoundPlayers.Collision.PlayLooping();
                    }
                }
                else
                {
                    var prev = (long)SoundPlayers.Collision.Tag;
                    if (state.LastCollisionTimestamp > prev)
                    {
                        SoundPlayers.Collision.Tag = state.LastCollisionTimestamp;

                        if (Settings.SoundEffects)
                        {
                            SoundPlayers.Collision.Stop();
                            SoundPlayers.Collision.PlayLooping();
                        }
                    }
                }
            }
            else if (SoundPlayers.Collision.Tag is not null)
            {
                var prev = (long)SoundPlayers.Collision.Tag;
                var elapsed = TimeSpan.FromTicks(Loop.Clock - prev);
                if (elapsed > Util.OneSecond)
                {
                    SoundPlayers.Collision.Tag = null;

                    if (Settings.SoundEffects)
                    {
                        SoundPlayers.Collision.Stop();
                    }
                }
            }

            if (!state.IsVisible)
            {
                return;
            }

            if (Settings.HitBoxMode)
            {
                DrawShipHitBox(g);
            }
            else
            {
                if (state.Accel > 0.0)
                {
                    DrawShipBurning(g);
                }
                else
                {
                    DrawShipIdle(g);
                }
            }
        }

        private void DrawShipBurning(GraphicsD g)
        {
            if (SoundPlayers.Burn.Tag is null)
            {
                // engine burning but not sounding yet, start sound
                SoundPlayers.Burn.Tag = new object();

                if (Settings.SoundEffects)
                {
                    SoundPlayers.Burn.PlayLooping();
                }
            }

            DrawShipImage(g, Images.SpaceshipThrust);
        }

        private void DrawShipIdle(GraphicsD g)
        {
            if (SoundPlayers.Burn.Tag is not null)
            {
                // engine no longer burning but still sounding, stop sound
                SoundPlayers.Burn.Tag = null;
                
                if (Settings.SoundEffects)
                {
                    SoundPlayers.Burn.Stop();
                }
            }

            DrawShipImage(g, Images.Spaceship);
        }

        private void DrawShipImage(GraphicsD g, Image shipImage)
        {
            var state = Loop.State.Ship;
            var pos = state.PositionInches;
            var ctx0 = (g, state, x: pos.X, y: pos.Y, shipImage);

            Util.UndoTransform(g, ctx0, ctx1 =>
            {
                ctx1.g.TranslateTransform(ctx1.x, ctx1.y);
                ctx1.g.RotateTransform(ctx1.state.HeadingAngle.Degrees);
                ctx1.g.TranslateTransform(-ctx1.x, -ctx1.y);
                ctx1.g.DrawImage(ctx1.shipImage, ctx1.x - ctx1.state.RadiusInches, ctx1.y - ctx1.state.RadiusInches);
            });
        }

        private void DrawShipHitBox(GraphicsD g)
        {
            var state = Loop.State.Ship;

            if (state.Status != ShipStatus.Ok && state.Status != ShipStatus.Respawning || !state.IsVisible)
            {
                return;
            }

            using var pen = new Pen(Settings.ShipHitBoxColor, Settings.ShipHitBoxWidth / (float)g.DpiX);

            g.DrawLines(pen, state.HitBoxInches);
        }

        private void DrawExplosion(GraphicsD g)
        {
            var state = Loop.State.Ship;
            var seq = state.StatusSequence - 1;

            if (seq < 0)
            {
                return;
            }

            if (seq == 0)
            {
                // just exploded, play sound
                if (Settings.SoundEffects)
                {
                    SoundPlayers.Explosion.Play();
                }
            }

            var x = state.PositionInches.X - state.RadiusInches;
            var y = state.PositionInches.Y - state.RadiusInches;
            g.DrawImage(Images.Explosions[seq], x, y);
        }
    }
}
