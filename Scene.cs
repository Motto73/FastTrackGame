using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal class Scene
    {

        //TODO!!! Replace the current framebuffer system!!!
        // - Only initialize once?
        // - Use char[][] instead for performance
        // - Instead of replaceat just buffer[y][x]
        // - StringBuilder apparently works great with this

        public string Name { get; private set; }
        public List<Sprite> Sprites = new List<Sprite>();
        public List<Sprite> QueueAdd = new List<Sprite>();
        public List<Sprite> QueueRemove = new List<Sprite>();
        public List<Projectile> Projectiles = new List<Projectile>();
        private ScenePreset ogPreset;

        public Scene(string name, Game game, ScenePreset preset = ScenePreset.None)
        {
            Name = name;
            ogPreset = preset;

            LoadPreset(preset, game);
            game.DebugTexts.Add("Loaded " + name, 500);
        }

        public void Update(float elapsed, Game game)
        {
            //Complete the queue first
            foreach(var sprite in QueueAdd)
                Sprites.Add(sprite);
            QueueAdd.Clear();

            //Update sprites second
            foreach (var sprite in Sprites)
            {
                sprite.Update(elapsed, game);
            }

            //Then projectiles
            foreach (var projectile in Projectiles)
                projectile.Update(elapsed);

            //TODO - maybe make custom class for different scene types to have custom scripts running in them
            if (ogPreset == ScenePreset.Intro)
                if (Input.IsPressed(Input.Key.Space))
                    game.LoadSceneNextFrame(1);

            //Last but not least, complete removals
            foreach (var sprite in QueueRemove)
                if (Sprites.Contains(sprite))
                    Sprites.Remove(sprite);
            QueueRemove.Clear();
        }

        public void Draw(Game game)
        {
            //Prepare a buffer and apply any amount of changes to it
            string[] buffer = new string[(int)game.ScreenResolution.Y];
            //Init whitespace
            for (int y = 0; y < buffer.Length; y++)
            {
                var s = "";
                for (int x = 0; x < game.ScreenResolution.X; x++)
                    s += " ";
                buffer[y] = s;
            }
            //Draw all sprites. In the future I should automatically cull ones not near screen.
            foreach (var sprite in Sprites)
            {
                if (sprite is ParticleSystem)
                {
                    var ps = (ParticleSystem)sprite;
                    int i = 0;
                    var d = ps.GetDisplays();
                    foreach(var p in ps.GetPositions())
                    {
                        var finalX = (int)(p - game.CameraPosition).X;
                        var finalY = (int)(p - game.CameraPosition).Y;
                        if (finalX < game.ScreenResolution.X && finalY < game.ScreenResolution.Y && finalX >= 0 && finalY >= 0)
                        {
                            buffer[finalY] = ReplaceAt(buffer[finalY], finalX, d[i]);
                        }
                        i++;
                    }
                }
                else
                {
                    var texts = sprite.GetActiveFrame();
                    var offset = sprite.Position - game.CameraPosition;
                    var size = sprite.GetSize();

                    int yPos = (int)(offset.Y - size.Y);
                    int xPos = (int)(offset.X - size.X);

                    for (int y = 0; y < size.Y; y++)
                    {
                        for (int x = 0; x < size.X; x++)
                        {
                            var ch = texts[y][x];
                            if (ch == ' ' && sprite.SpaceIsAlpha) continue;
                            var finalY = yPos + y;
                            var finalX = xPos + x;
                            if (finalX < game.ScreenResolution.X && finalY < game.ScreenResolution.Y && finalX >= 0 && finalY >= 0)
                            {
                                buffer[finalY] = ReplaceAt(buffer[finalY], finalX, ch);
                            }
                        }
                    }
                }
            }
            //Draw projectiles on top
            foreach(var p in Projectiles)
            {
                var offset = p.Position - game.CameraPosition;
                if (offset.X < game.ScreenResolution.X && offset.Y < game.ScreenResolution.Y && offset.X >= 0 && offset.Y >= 0)
                {
                    int index = (int)offset.Y;
                    buffer[index] = ReplaceAt(buffer[index], (int)offset.X, p.Visual);
                }
            }
            //Draw debug texts too
            int num = 1;
            foreach((var k, var v) in game.DebugTexts)
            {
                //Only draw as many lines as can fit
                if (num > buffer.Length) break;
                var index = buffer.Length - num;
                buffer[index] = buffer[index].Substring(0, buffer[index].Length - k.Length) + k;
                num++;
            }
            //Print the buffer
            //Console.Clear(); //Clear was the naive solution, and made the console always show one blank frame which was super jarring
            Console.SetCursorPosition(0, 0);
            foreach (var s in buffer)
            {
                Console.WriteLine(s);
            }
        }

        private void LoadPreset(ScenePreset preset, Game game)
        {
            //TODO save in separate files?
            if(preset == ScenePreset.Intro)
            {
                var funky = new CustomSprite(8, CustomSpriteType.Rings, game.ScreenResolution);
                funky.Position = game.ScreenResolution;

                var logo = Sprite.Logo2();
                logo.SpaceIsAlpha = false;
                logo.Position = game.ScreenResolution / 2 + logo.GetSize() / 2;

                var pressspace = new Sprite(5, [
                    new SpriteFrame([" PRESS SPACE TO PLAY "], 200f),
                    new SpriteFrame(["                     "], 200f) ]);
                pressspace.SpaceIsAlpha = false;
                pressspace.Position = game.ScreenResolution / 2 + pressspace.GetSize() / 2 + new Vector2(0, game.ScreenResolution.Y * 0.25f);

                var hiscore = new Sprite(5, [
                    new SpriteFrame([" PRESS DOWN FOR HISCORES "], 200f),
                    new SpriteFrame(["                         "], 200f) ]);
                hiscore.SpaceIsAlpha = false;
                hiscore.Position = game.ScreenResolution / 2 + hiscore.GetSize() / 2 + new Vector2(0, game.ScreenResolution.Y * 0.25f);
                hiscore.Position = new Vector2((game.ScreenResolution / 2 + hiscore.GetSize() / 2).X, pressspace.Position.Y + 2);

                Sprites.Add(funky);
                Sprites.Add(logo);
                Sprites.Add(pressspace);
                Sprites.Add(hiscore);
            }
            if(preset == ScenePreset.Level1)
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

        //Draw helpers
        private string ReplaceAt(string text, int index, char ch)
        {
            var chars = text.ToCharArray();
            chars[index] = ch;
            return new string(chars);
        }
    }

    public enum ScenePreset
    {
        None,
        Intro,
        Level1,
        Pause,
    }
}
