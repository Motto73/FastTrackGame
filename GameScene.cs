using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal class GameScene : Scene
    {

        private float spawnCursor = 0;
        private float gameSpeed = 20;
        private float moveCam = 0.01f;

        public bool GameOver = false;
        public bool GameStarted = false;

        Player player1;
        private float height;


        public GameScene(string name, Game game, ScenePreset preset) : base(name, game, preset)
        {
            spawnCursor = game.CameraPosition.X - game.ScreenResolution.X;
            height = game.ScreenResolution.Y / 4 * 3;
        }

        public override void Update(float elapsed, Game game)
        {
            //Gameover check
            if (GameOver)
            {
                if (Input.IsPressed(Input.Key.Down) || Input.IsHeld(Input.Key.Down))
                {
                    game.RefreshHighScores();
                    game.LoadSceneNextFrame(3, true);
                }
                if (Input.IsPressed(Input.Key.Space) || Input.IsHeld(Input.Key.Space))
                {
                    game.LoadSceneNextFrame(0, true);
                }
                gameSpeed = 0;
            }

            if (GameStarted)
            {
                //Camera
                var dt = elapsed / 1000;
                gameSpeed += dt;
                game.CameraPosition.X += moveCam * gameSpeed;
                //Spawning
                var rng = new Random();
                while(spawnCursor < game.CameraPosition.X + game.ScreenResolution.X)
                {
                    spawnCursor += 8;
                    height += rng.Next(-1, 2);
                    if (height > game.ScreenResolution.Y - 1) height = game.ScreenResolution.Y - 1;
                    if (height < 4) height = 4; 

                    var box = new Sprite(1, [
                    new SpriteFrame(["12345678"], 500f),
                    new SpriteFrame(["87654321"], 500f) ]);
                    box.IsSolid = true;
                    box.Position = new Vector2(spawnCursor, height);
                    Sprites.Add(box);
                }
            }
            
            base.Update(elapsed, game);
        }

        public override void LoadPreset(ScenePreset preset, Game game)
        {
            GameOver = false;
            GameStarted = false;

            player1 = Player.CreatePlayer(this);
            player1.Position = new Vector2(game.ScreenResolution.X / 2, game.ScreenResolution.Y / 2);
            Sprites.Add(player1);

            var box = new Sprite(1, [
                new SpriteFrame(["WASD to fire - Arrows to move"], 500f)]);
            box.Position = new Vector2(game.ScreenResolution.X / 2, 4);


            var box3 = new Sprite(1, [
                new SpriteFrame(["XXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXX"], 500f),
                    new SpriteFrame(["xXxXxXxXxXxXxXxXxXxXxXxX", "xXxXxXxXxXxXxXxXxXxXxXxX", "xXxXxXxXxXxXxXxXxXxXxXxX", "xXxXxXxXxXxXxXxXxXxXxXxX"], 500f) ]);
            box3.IsSolid = true;
            box3.Position = new Vector2(game.ScreenResolution.X / 3 + 20, game.ScreenResolution.Y / 4 * 3);

            Sprites.Add(box);
            Sprites.Add(box3);
        }

        public void AddScore(int num)
        {
            player1.Score += num;
        }
    }
}
