using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    //Creature extends sprite, it can have multiple animations instead of just a repeating one. Also houses hitboxes, HP, AI
    internal class Player : Creature
    {
        private bool isCrouching = false;
        private float fireTimer = 0;
        //TODO - weapon class
        private float fireDelay = 100f;
        private int fireDir = 1;

        public override void Update(float e, Game game, bool onlyTime = false)
        {
            base.Update(e, game);
            var ogPos = Position;

            if (IsControlledByPlayer)
            {
                //Crouch
                isCrouching = Input.IsPressed(Input.Key.Down) || Input.IsHeld(Input.Key.Down);

                //Firing
                fireTimer -= e;
                if(fireTimer <= 0)
                {
                    if (Input.IsPressed(Input.Key.FireLeft))
                    {
                        fireTimer = fireDelay;
                        fireDir = -1;
                        game.LoadedScenes[game.CurrentScene].Projectiles.Add(new Projectile('*', Position + new Vector2(-3, isCrouching ? -1 : -3), new Vector2(-100, 0), this, 1, false));
                    } else if (Input.IsPressed(Input.Key.FireRight))
                    {
                        fireTimer = fireDelay;
                        fireDir = 1;
                        game.LoadedScenes[game.CurrentScene].Projectiles.Add(new Projectile('*', Position + new Vector2(1, isCrouching ? -1 : -3), new Vector2(100, 0), this, 1, false));
                    }
                }

                //Movement
                float move = 0;
                if (Input.IsHeld(Input.Key.Left) || Input.IsPressed(Input.Key.Left))
                    move += -e / 1000f * Acceleration * (isCrouching ? 0.1f : 1.0f);
                if (Input.IsHeld(Input.Key.Right) || Input.IsPressed(Input.Key.Right))
                    move += e / 1000f * Acceleration * (isCrouching ? 0.1f : 1.0f);
                xMomentum += move;
                if (move < .1)
                    xMomentum *= 0.9f;
                xMomentum = Math.Clamp(xMomentum, -MoveSpeed, MoveSpeed);

                //Jump
                if (Input.IsPressed(Input.Key.Up))
                {
                    if (jumps > 0 || IsGrounded)
                        yMomentum = -.75f;
                    if (!IsGrounded)
                        jumps -= 1;
                }

                //Apply
                Position += new Vector2(xMomentum, 0);
                game.CameraPosition.X = Position.X - game.ScreenResolution.X / 2;
            }
            if (IsAffectedByGravity)
            {
                Position += new Vector2(0, yMomentum);
                yMomentum += e / 1000f * 4f;
            }
            CheckCollision(game, ogPos);

            game.DebugTexts.Add("Grounded: " + IsGrounded, 1);
            game.DebugTexts.Add("Jumps   : " + jumps, 1);
            game.DebugTexts.Add($"Fire:{fireTimer} {fireDir}", 1);

            //Firing animations
            if (fireTimer > 0)
            {
                if (isCrouching)
                {
                    if (fireDir > 0)
                        SetAnimation("fireRightCrouch");
                    else
                        SetAnimation("fireLeftCrouch");
                }
                else
                {
                    if (fireDir > 0)
                        SetAnimation("fireRight");
                    else
                        SetAnimation("fireLeft");
                }
            }
            else if (isCrouching)
                SetAnimation("crouch");
            //Animations - ground
            else if (yMomentum < 0.1f && yMomentum > -0.1f)
            {
                if (xMomentum < -0.25f)
                    SetAnimation("sprintLeft");
                else if (xMomentum > 0.25f)
                    SetAnimation("sprintRight");
                else SetAnimation("idle");
            }
            //Animations - air
            else
            {
                if (yMomentum > 0)
                {
                    if (xMomentum < -0.25f)
                        SetAnimation("fallLeft");
                    else
                        SetAnimation("fallRight");
                }
                else if (xMomentum < -0.25f)
                    SetAnimation("jumpRight");  //No left anim yet
                else SetAnimation("jumpRight");
            }

            lastMove = Position - ogPos;
        }

        //Presets

        public static Creature CreatePlayer()
        {
            var idleAnim = new SpriteFrame[]
            {
                new SpriteFrame([
                " o ",
                "(|)",
                "/ \\" ], 500),
                new SpriteFrame([
                " o ",
                "/|\\",
                "/ \\" ], 1500),
                new SpriteFrame([
                " o ",
                "(|)",
                "/ \\" ], 500),
                new SpriteFrame([
                "o  ",
                "(|)",
                "/ \\" ], 100),
                new SpriteFrame([
                " o ",
                "(|)",
                "/ \\" ], 100),
                new SpriteFrame([
                "  o",
                "(|)",
                "/ \\" ], 100),
            };
            var sprintRight = new SpriteFrame[]
            {
                new SpriteFrame([
                "_ o",
                " /L",
                "/ >" ], 100),
                new SpriteFrame([
                "  o",
                "//-",
                ">\\ " ], 100),
                new SpriteFrame([
                " o ",
                "<|-",
                "/> " ], 100),
            };
            var sprintLeft = new SpriteFrame[]
            {
                new SpriteFrame([
                "o _",
                "J\\ ",
                "< \\" ], 100),
                new SpriteFrame([
                "o  ",
                "-\\\\",
                " /<" ], 100),
                new SpriteFrame([
                " o ",
                "-|>",
                " <\\" ], 100),
            };
            var jumpRight = new SpriteFrame[]
            {
                new SpriteFrame([
                " \\o",
                "  /",
                " | " ], 50),
                new SpriteFrame([
                " _o",
                "/|/",
                "   " ], 50),
                new SpriteFrame([
                ">> ",
                "\\|o",
                "   " ], 50),
                new SpriteFrame([
                "/\\/",
                "o\\ ",
                "   " ], 50),
                new SpriteFrame([
                "o__",
                "| |",
                "   " ], 50),
                new SpriteFrame([
                "_o ",
                "  \\",
                " <<" ], 50),
            };
            var jumpLeft = new SpriteFrame[]
            {
                new SpriteFrame([
                "\\o/",
                "  \\",
                " ((" ], 100),
                new SpriteFrame([
                "\\o/",
                "  \\",
                " <<" ], 100),
                new SpriteFrame([
                "\\o/",
                " | ",
                "(( " ], 100000),
            };
            var fallRight = new SpriteFrame[]
            {
                new SpriteFrame([
                "\\o/",
                "/  ",
                ")) " ], 100),
                new SpriteFrame([
                "\\o/",
                "/  ",
                ">> " ], 100),
                new SpriteFrame([
                "\\o/",
                " | ",
                " ))" ], 100000),
            };
            var fallLeft = new SpriteFrame[]
            {
                new SpriteFrame([
                "\\o/",
                "  \\",
                " ((" ], 100),
                new SpriteFrame([
                "\\o/",
                "  \\",
                " <<" ], 100),
                new SpriteFrame([
                "\\o/",
                " | ",
                "(( " ], 100000),
            };
            var crouch = new SpriteFrame[]
            {
                new SpriteFrame([
                "<o>",
                "< >" ], 10000),
            };
            var fireRight = new SpriteFrame[]
            {
                new SpriteFrame([
                "o__",
                "|' ",
                "/> " ], 10000),
            };
            var fireRightC = new SpriteFrame[]
            {
                new SpriteFrame([
                "o__",
                ">>'" ], 10000),
            };
            var fireLeft = new SpriteFrame[]
            {
                new SpriteFrame([
                "__o",
                " '|",
                " <\\" ], 10000),
            };
            var fireLeftC = new SpriteFrame[]
            {
                new SpriteFrame([
                "__o",
                "'<<" ], 10000),
            };
            var debugAnim = new SpriteFrame[]
            {
                new SpriteFrame([
                "xXx",
                "XxX",
                "xXx" ], 50),
                new SpriteFrame([
                "XxX",
                "xXx",
                "XxX" ], 50)
            };

            var c = new Player();

            c.Animations.Add("idle", idleAnim);
            c.Animations.Add("debuge", debugAnim);
            c.Animations.Add("sprintLeft", sprintLeft);
            c.Animations.Add("sprintRight", sprintRight);
            c.Animations.Add("jumpLeft", jumpLeft);
            c.Animations.Add("jumpRight", jumpRight);
            c.Animations.Add("fallLeft", fallLeft);
            c.Animations.Add("fallRight", fallRight);
            c.Animations.Add("crouch", crouch);
            c.Animations.Add("fireRight", fireRight);
            c.Animations.Add("fireRightCrouch", fireRightC);
            c.Animations.Add("fireLeft", fireLeft);
            c.Animations.Add("fireLeftCrouch", fireLeftC);

            c.SetAnimation("idle");
            c.Acceleration = 30;
            c.MoveSpeed = 1.25f;
            c.IsControlledByPlayer = true;
            c.IsAffectedByGravity = true;
            return c;
        }
    }
}
