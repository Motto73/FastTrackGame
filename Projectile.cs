using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal class Projectile
    {
        public char Visual;
        public Vector2 Position;
        public Vector2 Velocity;
        public Creature Owner;
        public float Damage;
        public bool IsAffectedByGravity;


        public Projectile(char looks, Vector2 pos, Vector2 velocity, Creature owner, float damage, bool gravity)
        {
            Visual = looks;
            Position = pos;
            Velocity = velocity;
            Owner = owner;
            Damage = damage;
            IsAffectedByGravity = gravity;
        }

        public void Update(float e)
        {
            Position += Velocity * e / 1000f;
        }
    }
}
