using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal static class Utils
    {
        public const float XY_RATIO = 0.5f;

        public static Vector2 GetRatiod(Vector2 v)
        {
            return Vector2.Normalize(new Vector2(v.X, v.Y * XY_RATIO));
        }

        public static Vector2 RotateVector2(Vector2 v, float radians)
        {
            float cos = MathF.Cos(radians);
            float sin = MathF.Sin(radians);

            return new Vector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
        }
    }
}
