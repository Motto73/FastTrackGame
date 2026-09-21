using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace FastTrackGame
{
    internal class Game
    {
        //Settings and values
        private float frameTime;
        public float FrameRate { 
            get { return 1000f / frameTime; }
            set { frameTime = 1000f / value; }
        }

        private float timeElapsed = 0;
        public float Time
        {
            get { return timeElapsed; }
            private set;
        }
        public float GameSpeed = 1f;

        public Vector2 CameraPosition;
        public Vector2 ScreenResolution;

        private bool terminate = false;

        public List<Scene> LoadedScenes = new List<Scene>();
        private int nextScene = -1;
        private int currentScene = -1;
        public int CurrentScene { get { return currentScene; } private set; }

        public Dictionary<string, float> DebugTexts = new Dictionary<string, float>();

        //Consttuctor
        public Game(float frameRate, int sizeX, int sizeY)
        {
            FrameRate = frameRate;
            ScreenResolution = new Vector2(sizeX, sizeY);
        }

        //Control
        public void Start()
        {
            LoadedScenes.Add(new Scene("Intro", this, ScenePreset.Intro));
            LoadedScenes.Add(new Scene("Level1", this, ScenePreset.Level1));
            LoadedScenes.Add(new Scene("Pause", this, ScenePreset.Pause));

            currentScene = 0;

            while (!terminate)
            {
                Update();
                Thread.Sleep((int)frameTime);
            }
        }

        public void Update()
        {
            //Add elapsed time
            timeElapsed += frameTime;

            //Update debug texts
            List<string> removeKeys = new List<string>(); 
            foreach((var k,var v) in DebugTexts)
            {
                DebugTexts[k] = v - frameTime;
                if(v-frameTime < 0)
                    removeKeys.Add(k);
            }
            foreach(var k in removeKeys)
                DebugTexts.Remove(k);

            //Check if a new scene should be set active
            if(nextScene != -1)
            {
                currentScene = nextScene;
                nextScene = -1;
            }

            //Update input
            Input.Update();
            if (Input.IsPressed(Input.Key.Escape))
                terminate = true;

            //Do stuff
            if (currentScene != -1 && currentScene < LoadedScenes.Count)
            {
                LoadedScenes[currentScene].Update(frameTime, this);

                //Finally, draw it all
                LoadedScenes[currentScene].Draw(this);
            }
        }

        public void LoadSceneNextFrame(int scene)
        {
            if (scene >= 0 && scene < LoadedScenes.Count)
                nextScene = scene;
            else throw new ArgumentException($"Tried to change to nonexisting scene: {scene}");
        }

        public List<Sprite> GetLoadedSprites()
        {
            if (currentScene != -1 && currentScene < LoadedScenes.Count)
            {
                return LoadedScenes[currentScene].Sprites;
            }
            return null;
        }
    }
}
