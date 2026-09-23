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
            if (CustomType == CustomSpriteType.Rings)
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
            if (CustomType == CustomSpriteType.Eye)
            {
                //Eye
                string lookup2 = " ..,,ooOO0";
                string lookupRandom = "             *               .                                                       ,                 .                ";
                var height = (int)Size.Y;
                var width = (int)Size.X;
                string[] rows = new string[(int)height];
                for (int y = height - 1; y >= 0; y--)
                {
                    string s = "";
                    for (int x = 0; x < width; x++)
                    {
                        float ts = GetTime() * 0.001f * 2;
                        float u = (float)x / width;
                        float v = (float)y / height;


                        //var scale = 1f + (float)Math.Sin(ts) * 0.5f;
                        //u = scaleUV(u, scale);
                        //v = scaleUV(v, scale);

                        var cu = 0.5f + (float)Math.Sin(ts * 0.25f) * 0.1f;
                        var cv = 0.5f + (float)Math.Cos(ts * 0.25f) * 0.05f;


                        var rand = Math.Abs(Math.Sin(x * 3213 * ts) * Math.Cos(y * 232 * ts) * 4124234);
                        var c = " ";
                        c = "" + lookupRandom[(int)(rand % (lookupRandom.Length - 1))];

                        var mod = 0.4f * (1f - (float)Math.Pow(Math.Sin(ts) * 0.5f + 0.5f, 8));
                        //Clear random from eye
                        if (0.5f - eyeFunc(u, 0.5f) < v && v < eyeFunc(u, 0.5f) + 0.5)
                        {
                            c = " ";
                        }
                        //Iris and pupil
                        string lookupIris = "  .,oO0Oo,.  ";
                        var d = Vector2.Distance(new Vector2(u, v), new Vector2(cu, cv));
                        if (d < 0.25f)
                        {
                            int choose = (int)Math.Floor((lookupIris.Length - 1) * Math.Clamp(d / 0.25f, 0, 1));
                            c = "" + lookupIris[choose];
                        }
                        //Upper eyelid
                        if (eyeFunc(u, mod) + 0.5 - 1f / height < v && v < eyeFunc(u, 0.5f) + 0.5)
                        {
                            var id = (int)Math.Abs((eyeFunc(u, mod) + 0.5 + 1f / height - v) * 20);
                            id = lookup2.Length - id - 1;
                            id = Math.Clamp(id, 0, lookup2.Length - 1);
                            c = "" + lookup2[id];
                        }
                        //Lower lid
                        if (0.5f - eyeFunc(u, 0.5f) < v && v < 0.5f - eyeFunc(u, mod) + 1f / height)
                        {
                            var id = (int)((0.5f - eyeFunc(u, mod) + 1f / height - v) * 20);
                            id = lookup2.Length - id - 1;
                            id = Math.Clamp(id, 0, lookup2.Length - 1);
                            c = "" + lookup2[id];
                        }
                        s += c;
                    }
                    rows[y] = s;
                }
                return rows;
            }
            throw new Exception("What?");
        }

        public override Vector2 GetSize()
        {
            return Size;
        }

        static float eyeFunc(float u, float mod)
        {
            return (float)(Math.Pow(Math.Sin(u * Math.PI), 0.5) * mod);
        }

    }

    public enum CustomSpriteType
    {
        Rings,
        Eye
    }
}
