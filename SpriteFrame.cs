using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal class SpriteFrame
    {
        public Vector2 Size { get; private set; }
        public string[] Pixels { get; private set; }

        public float Duration { get; set; }

        public SpriteFrame(string[] text, float duration)
        {
            //Make sure the sprite is a rectangle
            int x = text[0].Length;
            foreach (string s in text)
            {
                if (s.Length != x)
                {
                    throw new ArgumentException("Sprites must be rectangular! Every row must be the same length.");
                }
            }

            Duration = duration;
            Pixels = text;
            Size = new Vector2(x, text.Length);
        }
    }
}
