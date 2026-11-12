using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{ 
    internal class Player : Creature
    {
        private bool doCameraTracking = false;
        private bool isCrouching = false;
        private float fireTimer = 0;
        //TODO - weapon class
        private float fireDelay = 100f;
        private Vector2 fireDir = new Vector2(1, 0);

        private GameScene gameScene;

        public int Score = 0;

        public Player(GameScene scene) : base(1) 
        {
            gameScene = scene;
        }

        public override void Update(float e, Game game, bool onlyTime = false)
        {
            base.Update(e, game);

            if (IsDead) return;

            var ogPos = Position;

            if (ogPos.Y > game.CameraPosition.Y + game.ScreenResolution.Y + 5)
                Kill(game);

            if(!gameScene.GameStarted)
                if (Input.AnyMatching([Input.Key.Up, Input.Key.Right, Input.Key.Down, Input.Key.Left], [Input.KeyState.Pressed]))
                    gameScene.GameStarted = true;

            if (IsControlledByPlayer)
            {
                //Debug
                if (Input.IsPressed(Input.Key.Debug))
                {
                    var ps = new ParticleSystem(2)
                    {
                        Position = ogPos + new Vector2(0, -3),
                        Display = "1234567890",
                        EmissionRate = 20,
                        ParticleSpeed = 20,
                        ParticleLifetime = 0.5f,
                        EmitForDuration = 2,
                        RandomSpeed = 4,
                        TrailLength = 0.5f
                    };
                    game.LoadedScenes[game.CurrentScene].QueueAdd.Add(ps);
                }

                //Crouch
                isCrouching = Input.IsPressed(Input.Key.Down) || Input.IsHeld(Input.Key.Down);

                //Firing
                fireTimer -= e;
                if(fireTimer < 0) fireTimer = 0;
                if (Input.AnyMatching([Input.Key.FireUp, Input.Key.FireRight, Input.Key.FireDown, Input.Key.FireLeft], [Input.KeyState.Held]))
                {
                    if (fireTimer <= 0)
                    {
                        Vector2 xy = Vector2.Zero;
                        if (Input.IsHeld(Input.Key.FireRight)) xy.X++;
                        if (Input.IsHeld(Input.Key.FireLeft)) xy.X--;
                        if (Input.IsHeld(Input.Key.FireDown)) xy.Y++;
                        if (Input.IsHeld(Input.Key.FireUp)) xy.Y--;
                        //If for some reason the player holds opposing keys, set a default direction (down)
                        if (xy.Length() < 0.1) xy = new Vector2(0, 1);
                        fireDir = xy;
                        xy = Vector2.Normalize(xy);
                        fireTimer = fireDelay;
                        var pos = Position + new Vector2(-1, isCrouching ? -1 : -3) + fireDir;
                        var dir = Utils.GetRatiod(xy);
                        game.LoadedScenes[game.CurrentScene].Projectiles.Add(new Projectile('*', pos, dir * 200 , this, 1, false));
                        var ps = new ParticleSystem(2)
                        {
                            Position = pos,
                            Display = ".*.*.*.*.",
                            EmissionRate = 0,
                            ParticleSpeed = 50,
                            ParticleLifetime = 0.05f,
                            EmitForDuration = 0,
                            RandomSpeed = 1,
                            RandomLifetime = 0.1f,
                            EmissionDirection = dir,
                            RandomDirection = .5f,
                            ParticleDampen = 0.9f
                        };
                        ps.Burst(20);
                        game.LoadedScenes[game.CurrentScene].QueueAdd.Add(ps);
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
                    {
                        yMomentum = -.75f;
                        if (!IsGrounded)
                        {
                            jumps -= 1;

                            var ps = new ParticleSystem(2)
                            {
                                Position = ogPos + new Vector2(0, -3),
                                Display = ".*.*.*.*.",
                                EmissionRate = 0,
                                ParticleSpeed = 20,
                                ParticleLifetime = 0.1f,
                                EmitForDuration = 0,
                                RandomSpeed = 1,
                                RandomLifetime = 0.5f,
                                EmissionDirection = new Vector2(0, 1),
                                RandomDirection = 2f,
                                ParticleDampen = 0.9f
                            };
                            ps.Burst(6);
                            game.LoadedScenes[game.CurrentScene].QueueAdd.Add(ps);
                        }
                    }
                }

                //Apply
                Position += new Vector2(xMomentum, 0);
                if (doCameraTracking)
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
            var num = 0;
            foreach (var s in game.LoadedScenes[game.CurrentScene].Sprites)
                if (s is ParticleSystem)
                    num++;
            game.DebugTexts.Add("Total Particlesystems: " + num, 1);
            game.DebugTexts.Add($"Distance: {(int)Position.X}", 1);

            //Firing animations
            if (fireTimer > 0)
            {
                var str = "fire";
                if (fireDir == new Vector2(1, 0)) str += "Right";
                if (fireDir == new Vector2(-1, 0)) str += "Left";
                if (fireDir == new Vector2(0, 1)) str += "Down";
                if (fireDir == new Vector2(0, -1)) str += "Up";
                if (fireDir == new Vector2(1, 1)) str += "RightDown";
                if (fireDir == new Vector2(1, -1)) str += "RightUp";
                if (fireDir == new Vector2(-1, 1)) str += "LeftDown";
                if (fireDir == new Vector2(-1, -1)) str += "LeftUp";
                if (isCrouching) str += "Crouch";
                SetAnimation(str);
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

        public override void Kill(Game game)
        {
            if (gameScene != null) gameScene.GameOver = true;

            var ps = new ParticleSystem(2)
            {
                Position = Position + new Vector2(0, -3),
                Display = "1234567890/*-+!#¤%&",
                EmissionRate = 0,
                ParticleSpeed = 50,
                ParticleLifetime = 5f,
                EmitForDuration = 0,
                RandomSpeed = 4,
                TrailLength = 0.1f
            };
            ps.Burst(200);
            game.LoadedScenes[game.CurrentScene].QueueAdd.Add(ps);

            Sprite logo = Sprite.GetRandomDeathLogo();
            logo.DoGlitch = true;
            logo.Position = game.CameraPosition + game.ScreenResolution / 2 + logo.GetSize() / 2;

            var pressspace = new Sprite(5, [
                new SpriteFrame([" PRESS SPACE TO RESTART "], 200f),
                new SpriteFrame(["                        "], 200f) ]);
            pressspace.SpaceIsAlpha = false;
            pressspace.Position = game.CameraPosition + game.ScreenResolution / 2 + pressspace.GetSize() / 2 + new Vector2(0, game.ScreenResolution.Y * 0.25f);

            var hiscore = new Sprite(5, [
                new SpriteFrame([" PRESS DOWN FOR HISCORES "], 200f),
                new SpriteFrame(["                         "], 200f) ]);
            hiscore.SpaceIsAlpha = false;
            hiscore.Position = game.CameraPosition + new Vector2((game.ScreenResolution / 2 + hiscore.GetSize() / 2).X, pressspace.Position.Y + 2);


            game.LoadedScenes[game.CurrentScene].QueueAdd.Add(logo);
            game.LoadedScenes[game.CurrentScene].QueueAdd.Add(hiscore);
            game.LoadedScenes[game.CurrentScene].QueueAdd.Add(pressspace);

            //Saving
            Score += (int)Position.X;
            HighScores.AddSave(Score);

            SetAnimation("debuge");

            base.Kill(game);
        }

        //Presets

        public static Player CreatePlayer(GameScene gs)
        {
            var c = new Player(gs);

            c.Animations.Add("idle",
            [
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
            ]);
            c.Animations.Add("debuge",
            [
                new SpriteFrame([
                "xXx",
                "XxX",
                "xXx" ], 50),
                new SpriteFrame([
                "XxX",
                "xXx",
                "XxX" ], 50)
            ]);
            c.Animations.Add("sprintLeft",
            [
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
            ]);
            c.Animations.Add("sprintRight",
            [
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
            ]);
            c.Animations.Add("jumpLeft",
            [
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
            ]);
            c.Animations.Add("jumpRight",
            [
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
            ]);
            c.Animations.Add("fallLeft",
            [
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
            ]);
            c.Animations.Add("fallRight",
            [
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
            ]);
            c.Animations.Add("crouch",
            [
                new SpriteFrame([
                "<o>",
                "< >" ], 10000),
            ]);
            c.Animations.Add("fireRight",
            [
                new SpriteFrame([
                "o__",
                "|' ",
                "/> " ], 10000),
            ]);
            c.Animations.Add("fireRightCrouch",
            [
                new SpriteFrame([
                "o__",
                ">>'" ], 10000),
            ]);
            c.Animations.Add("fireRightUp",
            [
                new SpriteFrame([
                "o/>",
                "|' ",
                "/\\ " ], 10000),
            ]);
            c.Animations.Add("fireRightUpCrouch",
            [
                new SpriteFrame([
                "o/>",
                "<\\ ",], 10000),
            ]);
            c.Animations.Add("fireRightDown",
            [
                new SpriteFrame([
                "o. ",
                "|'\\",
                "/\\ " ], 10000),
            ]);
            c.Animations.Add("fireRightDownCrouch",
            [
                new SpriteFrame([
                "o. ",
                "<'\\"], 10000),
            ]);
            c.Animations.Add("fireLeft",
            [
                new SpriteFrame([
                "__o",
                " '|",
                " <\\" ], 10000),
            ]);
            c.Animations.Add("fireLeftCrouch",
            [
                new SpriteFrame([
                "__o",
                "'<<" ], 10000),
            ]);
            c.Animations.Add("fireLeftUp",
            [
                new SpriteFrame([
                "<\\o",
                " '|",
                " /\\" ], 10000),
            ]);
            c.Animations.Add("fireLeftDown",
            [
                new SpriteFrame([
                " .o",
                "/'|",
                " /\\" ], 10000),
            ]);
            c.Animations.Add("fireLeftUpCrouch",
            [
                new SpriteFrame([
                "<\\o",
                " />"], 10000),
            ]);
            c.Animations.Add("fireLeftDownCrouch",
            [
                new SpriteFrame([
                " .o",
                "/'>"], 10000),
            ]);
            c.Animations.Add("fireUp",
            [
                new SpriteFrame([
                "o|>",
                " |'",
                "/ \\" ], 10000),
            ]);
            c.Animations.Add("fireUpCrouch",
            [
                new SpriteFrame([
                "o|>",
                "< >"], 10000),
            ]);
            c.Animations.Add("fireDown",
            [
                new SpriteFrame([
                " o.",
                " ||",
                "/| " ], 10000),
            ]);
            c.Animations.Add("fireDownCrouch",
            [
                new SpriteFrame([
                ".o.",
                "<|>",], 10000),
            ]);

            c.SetAnimation("idle");
            c.Acceleration = 30;
            c.MoveSpeed = 1.25f;
            c.IsControlledByPlayer = true;
            c.IsAffectedByGravity = true;
            return c;
        }
    }
}
