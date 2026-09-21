using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    //Creature extends sprite, it can have multiple animations instead of just a repeating one. Also houses hitboxes, HP, AI
    //Abtract since every creature will have custom behavior and stuff idk
    internal abstract class Creature : Sprite
    {
        public Dictionary<string, SpriteFrame[]> Animations = new Dictionary<string, SpriteFrame[]>();
        private string currentAnim = "";
        public bool IsAffectedByGravity = true;
        public bool IsControlledByPlayer = false;
        public float Acceleration = 40;
        public float MoveSpeed = 1;
        public bool IsGrounded = false;
        public float yMomentum = 0;
        public float xMomentum = 0;
        public Vector2 lastMove;
        public int coyote = 0;
        public int jumps = 0;

        public Creature(int depth) : base(depth, null)
        {

        }

        //TODO - this shit is messy, separate player and creature?
        public override void Update(float e, Game game, bool onlyTime = false)
        {
            base.Update(e, game);
            var ogPos = Position;
            if (!IsControlledByPlayer)
            {
                if (IsAffectedByGravity)
                {
                    Position += new Vector2(0, yMomentum);
                    yMomentum += e / 1000f * 4f;
                }
                CheckCollision(game, ogPos);
            }

            lastMove = Position - ogPos;
        }

        public void SetAnimation(string name, float speed = 1)
        {
            //Don't reset animation if already running
            if (currentAnim == name) return;
            if (Animations.ContainsKey(name))
            {
                var anim = Animations[name];
                foreach(var f in anim)
                {
                    f.Duration *= speed;
                }
                Frames = anim;
                CurrentFrame = 0;
                currentAnim = name;
            }
            else throw new ArgumentException($"Animation {name} does not exist!!");
        }

        //I used AI for this part, after failing miserably at making discrete collision detection. I have added comments myself to show that I understand the code
        public void CheckCollision(Game game, Vector2 oldPosition)
        {
            if (IsAffectedByGravity)
            {
                foreach (var s in game.GetLoadedSprites())
                {
                    //Don't collide with self, or non-solids
                    if (s == this || !s.IsSolid)
                        continue;

                    //These are written out for clarity cus collision detection is kinda hard
                    float myLeft = Position.X - GetSize().X;
                    float myRight = Position.X;
                    float myTop = Position.Y - GetSize().Y;
                    float myBottom = Position.Y;

                    float otherLeft = s.Position.X - s.GetSize().X;
                    float otherRight = s.Position.X;
                    float otherTop = s.Position.Y - s.GetSize().Y;
                    float otherBottom = s.Position.Y;

                    bool overlap =
                        myRight > otherLeft &&
                        myLeft < otherRight &&
                        myBottom > otherTop &&
                        myTop < otherBottom;

                    //If there is no overlap, check next
                    if (!overlap)
                        continue;

                    //Where were we relative to the object BEFORE moving? (this was a key part, apparently current momentum was not enough)
                    float oldLeft = oldPosition.X - GetSize().X;
                    float oldRight = oldPosition.X;
                    float oldTop = oldPosition.Y - GetSize().Y;
                    float oldBottom = oldPosition.Y;

                    //Came from left
                    if (oldRight <= otherLeft)
                    {
                        Position.X = otherLeft;
                        xMomentum = 0;
                    }
                    //Came from right
                    else if (oldLeft >= otherRight)
                    {
                        Position.X = otherRight + GetSize().X;
                        xMomentum = 0;
                    }
                    //Came from above
                    else if (oldBottom <= otherTop)
                    {
                        Position.Y = otherTop;
                        yMomentum = 0;
                        coyote = 2;
                        jumps = 1;
                    }
                    //Came from below
                    else if (oldTop >= otherBottom)
                    {
                        Position.Y = otherBottom + GetSize().Y;
                        yMomentum = 0;
                    }
                }

                //Coyote timer to allow player to still get a free jump if it's right at the edge.
                IsGrounded = coyote > 0;
                coyote--;
            }
        }

        //Presets

       
    }
}
