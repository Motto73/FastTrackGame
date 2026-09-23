using System;
using System.Collections.Generic;
using System.Numerics;

namespace FastTrackGame
{
    internal class ParticleSystem : Sprite
    {
        //X,Y, Strength, Radius
        public List<Vector4> Forces = new List<Vector4>();
        private List<Particle> particles = new List<Particle>();
        private List<Particle> trails = new List<Particle>();

        public bool IsEmitting = true;
        public float EmitForDuration = -1;
        public float EmissionRate = 10;
        private float emissionRate { get { return 1f / EmissionRate; } set; }
        private float emissionTimer = 0;
        public Vector2 EmissionDirection = new Vector2(0, -1);
        public float RandomDirection = MathF.Tau;
        public float ParticleSpeed = 10;
        public float RandomSpeed = 2;
        public float ParticleDampen = 1;
        public float ParticleGravity = 1;
        public float ParticleLifetime = 5;
        public float RandomLifetime = 0.1f;
        public float ParticleNoiseStrength = 1;
        public string Display = "⋅'*'⋅";
        public float TrailLength = 0;

        private static Random rng;


        public ParticleSystem(int depth) : base(depth)
        {
            //Use custom constructor thingamabob
            if(rng == null) rng = new Random();
        }

        public override void Update(float e, Game game, bool onlyTime = true)
        {
            base.Update(e, game, true);

            //Easier to work with seconds here
            float dt = e / 1000;

            //Actually spawn trails
            foreach(var t in trails)
                particles.Add(t);
            trails.Clear();
            //Create particles
            if (IsEmitting)
            {
                emissionTimer += dt;

                //While loop so I can keep accurate track on high rate spawners
                while (emissionTimer > emissionRate)
                {
                    AddParticle();
                }
                
                if(EmitForDuration >= 0)
                {
                    EmitForDuration -= dt;
                    if (EmitForDuration < 0)
                        IsEmitting = false;
                }
            }
            //Move particles | Going backwards should work to avoid having another list to store particles marked for removal. With this many particles it matters
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                var particle = particles[i];

                particle.Update(e, this);

                if (particle.Lifetime <= 0)
                    particles.RemoveAt(i);
            }
            game.DebugTexts.Add($"ID[{ID}] Count: {particles.Count}|Emit:{IsEmitting}", 1);

            //Kill unused particle systems
            if (!IsEmitting && particles.Count <= 0)
                game.LoadedScenes[game.CurrentScene].QueueRemove.Add(this);
        }

        public void Burst(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddParticle();
            }
        }

        private void AddParticle()
        {
            var speed = ParticleSpeed * (1 + RandomSpeed * rng.NextSingle());
            var dir = Utils.RotateVector2(EmissionDirection, (rng.NextSingle() - 0.5f) * RandomDirection);
            dir.X *= 1f / Utils.XY_RATIO;
            var life = ParticleLifetime * (1 + RandomLifetime * rng.NextSingle());
            particles.Add(new Particle(Position, speed * dir, life, true));
            emissionTimer -= emissionRate;
        }

        public List<char> GetDisplays()
        {
            var list = new List<char>();
            foreach (var particle in particles)
                list.Add(particle.EvalDisplay(this));
            return list;
        }
        public List<Vector2> GetPositions()
        {
            var list = new List<Vector2>();
            foreach(var particle in particles)
                list.Add(particle.Position);
            return list;
        }

        public override string[] GetActiveFrame()
        {
            return ["ERROR"];
        }

        private class Particle
        {
            public Vector2 Position;
            public Vector2 Velocity;
            public float Lifetime;
            public bool IsTrail;
            private Vector2 lastPos;

            public Particle(Vector2 pos, Vector2 vel, float life, bool trail = false)
            {
                Position = pos;
                Velocity = vel;
                Lifetime = life;
                IsTrail = trail;
                lastPos = pos;
            }

            public void Update(float e, ParticleSystem system)
            {
                float dt = e / 1000f;
                Lifetime -= dt;
                Position += Velocity * dt;
                Velocity += new Vector2(0, 100 * dt * system.ParticleGravity);
                Velocity *= system.ParticleDampen;
                /*
                foreach (var f in system.Forces)
                    if (Vector2.Distance(Position, new Vector2(f.X, f.Y)) < f.W)
                        Velocity += Vector2.Normalize(new Vector2(f.X, f.Y) - Position) * f.Z * dt;
                */
                //Add trails
                if (system.TrailLength > 0)
                {
                    if(Vector2.DistanceSquared(lastPos, Position) > 1)
                    {
                        //Create a particle with TrailLength as lifetime
                        var p = new Particle(Position, Vector2.Zero, system.TrailLength, true);
                        system.trails.Add(p);
                        lastPos = Position;
                    }
                }
            }

            public char EvalDisplay(ParticleSystem s)
            {
                var f = Lifetime / s.ParticleLifetime;
                f *= s.Display.Length - 1;
                f = Math.Clamp(f, 0, s.Display.Length - 1);
                return s.Display[(int)f];
            }
        }
    }
}
