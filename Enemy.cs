using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal class Enemy : Creature
    {
        private GameScene gameScene;
        private int score = 25;
        private float fireInterval = 1f;
        private float fireTimer = 0;
        Vector2 fireDir;

        public Enemy(GameScene scene) : base(1)
        {
            gameScene = scene;
            fireTimer += (float)new Random().NextDouble();
        }

        public override void Update(float e, Game game, bool onlyTime = false)
        {
            if (!gameScene.GameOver)
            {
                fireTimer += e / 1000;
                if(fireTimer > fireInterval)
                {
                    fireTimer = 0;
                    var target = gameScene.GetPlayerPos();

                    var pos = Position + new Vector2(-1, -1);
                    var dir = target - pos;
                    dir = Vector2.Normalize(dir);
                    fireDir = dir;
                    game.LoadedScenes[game.CurrentScene].Projectiles.Add(new Projectile('+', pos, dir * 50, this, 1, false));
                }

                if (fireTimer < .5 && fireDir.Length() > 0.1f)
                {
                    Vector2[] directions =
                    {
                        new(1, 0),  new(-1, 0), new(0, 1),  new(0, -1), new(1, 1),  new(1, -1), new(-1, 1), new(-1, -1) 
                    };
                    string[] names =
                    {
                        "Right", "Left", "Down", "Up", "RightDown", "RightUp", "LeftDown", "LeftUp"
                    };

                    float highestDot = float.MinValue;
                    string closestDirection = "Right";

                    for (int i = 0; i < directions.Length; i++)
                    {
                        float dot = Vector2.Dot(
                            Vector2.Normalize(directions[i]),
                            fireDir
                        );

                        if (dot > highestDot)
                        {
                            highestDot = dot;
                            closestDirection = names[i];
                        }
                    }

                    SetAnimation("fire" + closestDirection);
                }
                else SetAnimation("idle");
            }
            base.Update(e, game, onlyTime);
        }

        public void KillForMoney(Game game)
        {
            gameScene.AddScore(score);

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
            ps.Burst(40);
            game.LoadedScenes[game.CurrentScene].QueueAdd.Add(ps);
            game.LoadedScenes[game.CurrentScene].QueueRemove.Add(this);
            Kill(game);
        }


        public static Enemy CreateEnemy(GameScene gs)
        {
            var c = new Enemy(gs);

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
            c.Animations.Add("fireRight",
            [
                new SpriteFrame([
                "o__",
                "|' ",
                "/> " ], 10000),
            ]);
            c.Animations.Add("fireRightUp",
            [
                new SpriteFrame([
                "o/>",
                "|' ",
                "/\\ " ], 10000),
            ]);
            c.Animations.Add("fireRightDown",
            [
                new SpriteFrame([
                "o. ",
                "|'\\",
                "/\\ " ], 10000),
            ]);
            c.Animations.Add("fireLeft",
            [
                new SpriteFrame([
                "__o",
                " '|",
                " <\\" ], 10000),
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
            c.Animations.Add("fireUp",
            [
                new SpriteFrame([
                "o|>",
                " |'",
                "/ \\" ], 10000),
            ]);
            c.Animations.Add("fireDown",
            [
                new SpriteFrame([
                " o.",
                " ||",
                "/| " ], 10000),
            ]);

            c.SetAnimation("idle");
            c.Acceleration = 30;
            c.MoveSpeed = 1.25f;
            c.IsAffectedByGravity = true;
            return c;
        }
    }
}
