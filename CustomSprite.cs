using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal class CustomSprite : Sprite
    {
        public CustomSpriteType CustomType { get; private set; }
        private Vector2 Size;

        public override void Update(float elapsed, Game game, bool onlyTime = true)
        {
            base.Update(elapsed, game, true);
        }

        public CustomSprite(int depth, CustomSpriteType type, Vector2 size) : base (depth, null)
        {
            CustomType = type;
            Size = size;
        }

        public override string[] GetActiveFrame()
        {
            if(CustomType == CustomSpriteType.Rings)
            {
                //Circle spin
                var height = Size.Y;
                var width = Size.X;
                string lookup = " .,oO0Oo,.";
                string[] rows = new string[(int)height];
                for (int y = 0; y < height; y++)
                {
                    string s = "";
                    for (int x = 0; x < width; x++)
                    {
                        float ts = GetTime() * 0.001f * 2;
                        float u = (float)x / width;
                        float v = (float)y / height;

                        float radius = 0.25f;
                        float offset_x = (float)Math.Sin(ts) * radius + 0.5f;
                        float offset_y = (float)Math.Cos(ts) * radius + 0.5f;

                        float val = Vector2.Distance(new Vector2(u, v), new Vector2(offset_x, offset_y));
                        val *= 2;
                        val += ts;
                        val %= 1.0f;
                        val = Math.Clamp(val, 0, 1);

                        int choose = (int)Math.Floor((lookup.Length - 1) * val);
                        s += lookup[choose];
                    }
                    rows[y] = s;
                }
                return rows;
            }
            return new string[] { "ERROR" };
        }

        public override Vector2 GetSize()
        {
            return Size;
        }

    }

    public enum CustomSpriteType
    {
        Rings
    }
}
