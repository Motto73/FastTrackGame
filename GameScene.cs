using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal class GameScene : Scene
    {

        private float spawnCursor = 0;


        public GameScene(string name, Game game, ScenePreset preset) : base(name, game, preset)
        {
            spawnCursor = game.CameraPosition.X + game.ScreenResolution.X;
        }

        public override void Update(float elapsed, Game game)
        {

            if (spawnCursor < game.CameraPosition.X + game.ScreenResolution.X)
            {
                spawnCursor = game.CameraPosition.X + game.ScreenResolution.X * 1.5f;
                var box = new Sprite(1, [
                new SpriteFrame(["XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"], 500f),
                    new SpriteFrame(["xXxXxXxXxXxXxXxXxXxXxXxXxXxXxXxXx"], 500f) ]);
                box.IsSolid = true;
                box.Position = new Vector2(spawnCursor + game.ScreenResolution.X, game.ScreenResolution.Y / 4 * 3);
                Sprites.Add(box);
            }
            
            base.Update(elapsed, game);
        }

        public override void LoadPreset(ScenePreset preset, Game game)
        {
            var player = Player.CreatePlayer();
            player.Position = new Vector2(game.ScreenResolution.X / 2, game.ScreenResolution.Y / 2);
            Sprites.Add(player);

            var box = new Sprite(1, [
                new SpriteFrame(["XXXXXXXXXXXXXXXXXXXXXXXX"], 500f),
                    new SpriteFrame(["xXxXxXxXxXxXxXxXxXxXxXxX"], 500f) ]);
            box.IsSolid = true;
            box.Position = new Vector2(game.ScreenResolution.X / 2, game.ScreenResolution.Y / 4 * 3);

            var box2 = box.Clone();
            box2.Position += new Vector2(50, 2);

            var box3 = new Sprite(1, [
                new SpriteFrame(["XXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXX", "XXXXXXXXXXXXXXXXXXXXXXXX"], 500f),
                    new SpriteFrame(["xXxXxXxXxXxXxXxXxXxXxXxX", "xXxXxXxXxXxXxXxXxXxXxXxX", "xXxXxXxXxXxXxXxXxXxXxXxX", "xXxXxXxXxXxXxXxXxXxXxXxX"], 500f) ]);
            box3.IsSolid = true;
            box3.Position = new Vector2(game.ScreenResolution.X / 3 + 20, game.ScreenResolution.Y / 4 * 3);

            Sprites.Add(box);
            Sprites.Add(box2);
            Sprites.Add(box3);
        }
    }
}
