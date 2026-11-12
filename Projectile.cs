using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    //Could possibly extend sprite?
    internal class Projectile
    {
        public char Visual;
        public Vector2 Position;
        public Vector2 Velocity;
        public Creature Owner;
        public float Damage { get; private set; }
        public bool IsAffectedByGravity { get; private set; }
        public bool Finished {  get; private set; }
        private int ticks = 200;


        public Projectile(char looks, Vector2 pos, Vector2 velocity, Creature owner, float damage, bool gravity)
        {
            Visual = looks;
            Position = pos;
            Velocity = velocity;
            Owner = owner;
            Damage = damage;
            IsAffectedByGravity = gravity;
        }

        public void Update(float e, Game game)
        {
            Position += Velocity * e / 1000f;
            foreach (var s in game.GetLoadedSprites())
            {
                if(s is Creature && s != Owner && !(s as Creature).IsDead)
                {
                    float otherLeft = s.Position.X - s.GetSize().X;
                    float otherRight = s.Position.X;
                    float otherTop = s.Position.Y - s.GetSize().Y;
                    float otherBottom = s.Position.Y;

                    bool overlap =
                        Position.X > otherLeft &&
                        Position.X < otherRight &&
                        Position.Y >= otherTop &&
                        Position.Y <= otherBottom;

                    if (overlap)
                    {
                        if (s is Enemy)
                        {
                            (s as Enemy).KillForMoney(game);
                            Finished = true;
                            return;
                        }
                        if (s is Player)
                            (s as Player).Kill(game);
                    }
                }
            }
            ticks--;
            if (ticks <= 0)
                Finished = true;
        }
    }
}
