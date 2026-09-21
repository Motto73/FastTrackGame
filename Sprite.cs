using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;


namespace FastTrackGame
{
    //Sprite is used for simple object with simple to no animations.
    internal class Sprite
    {
        public Vector2 Position;
        public SpriteFrame[] Frames;
        public int FrameCount { get { return Frames.Length; } private set; }
        public int CurrentFrame { get; set; }
        private float time;
        public bool IsSolid = false;
        public bool SpaceIsAlpha = true;

        public Sprite(SpriteFrame[] frames = null)
        {
            Frames = frames;
        }

        public Sprite Clone()
        {
            var c = new Sprite
            {
                Frames = Frames,
                Position = Position,
                CurrentFrame = CurrentFrame,
                time = time,
                IsSolid = IsSolid
            };
            return c;
        }

        public virtual void Update(float elapsed, Game game, bool onlyAddTime = false)
        {
            time += elapsed;

            if (onlyAddTime) return;

            if (Frames[CurrentFrame].Duration <= time)
            {
                time = 0;
                CurrentFrame++;
                CurrentFrame %= Frames.Length;
            }
        }

        public void SetFrame(int frame)
        {
            FrameCount = frame;
        }

        public virtual string[] GetActiveFrame()
        {
            return Frames[CurrentFrame].Pixels;
        }

        public virtual Vector2 GetSize()
        {
            return Frames[CurrentFrame].Size;
        }

        public float GetTime()
        {
            return time;
        }

        //Some preset sprites
        //TODO

        public static Sprite Logo0()
        {
            return new Sprite([
                new SpriteFrame([
                    "                                                                                      ",
                    " 88888888888                     888888888888                             88          ",
                    " 88                             ,d    88                                  88          ",
                    " 88                             88    88                                  88          ",
                    " 88aaaaa ,adPPYYba, ,adPPYba, MM88MMM 88 8b,dPPYba, ,adPPYYba,  ,adPPYba, 88   ,d8    ",
                    " 88\"\"\"\"\" \"\"     `Y8 I8[    \"\"   88    88 88P'   \"Y8 \"\"     `Y8 a8\"     \"\" 88 ,a8\"     ",
                    " 88      ,adPPPPP88  `\"Y8ba,    88    88 88         ,adPPPPP88 8b         8888[       ",
                    " 88      88,    ,88 aa    ]8I   88,   88 88         88,    ,88 \"8a,   ,aa 88`\"Yba,    ",
                    " 88      `\"8bbdP\"Y8 `\"YbbdP\"'   \"Y888 88 88         `\"8bbdP\"Y8  `\"Ybbd8\"' 88   `Y8a   ",
                    "                                                                                      "
            ], 500f),
            ]);
        }

        public static Sprite Logo1()
        {
            return new Sprite([
                new SpriteFrame([
                    "                                                                               ",
                    " O))))))))                   O))  O))) O))))))                        O))      ",
                    " O))                         O))       O))                            O))      ",
                    " O))         O))     O)))) O)O) O)     O))    O) O)))   O))       O)))O))  O)) ",
                    " O))))))   O))  O)) O))      O))       O))     O))    O))  O))  O))   O)) O))  ",
                    " O))      O))   O))   O)))   O))       O))     O))   O))   O)) O))    O)O))    ",
                    " O))      O))   O))     O))  O))       O))     O))   O))   O))  O))   O)) O))  ",
                    " O))        O)) O)))O)) O))   O))      O))    O)))     O)) O)))   O)))O))  O)) ",
                    "                                                                               "
        ], 500f),
        ]);
        }

        public static Sprite Logo2()
        {
            return new Sprite([
                new SpriteFrame([
                    "                                                                                  ",
                    "  ________                     __   ________                            __        ",
                    " /        |                   /  | /        |                          /  |       ",
                    " $$$$$$$$/______    _______  _$$ |_$$$$$$$$/______   ______    _______ $$ |   __  ",
                    " $$ |__  /      \\  /       |/ $$   |  $$ | /      \\ /      \\  /       |$$ |  /  | ",
                    " $$    | $$$$$$  |/$$$$$$$/ $$$$$$/   $$ |/$$$$$$  |$$$$$$  |/$$$$$$$/ $$ |_/$$/  ",
                    " $$$$$/  /    $$ |$$      \\   $$ | __ $$ |$$ |  $$/ /    $$ |$$ |      $$   $$<   ",
                    " $$ |   /$$$$$$$ | $$$$$$  |  $$ |/  |$$ |$$ |     /$$$$$$$ |$$ \\_____ $$$$$$  \\  ",
                    " $$ |   $$    $$ |/     $$/   $$  $$/ $$ |$$ |     $$    $$ |$$       |$$ | $$  | ",
                    " $$/     $$$$$$$/ $$$$$$$/     $$$$/  $$/ $$/       $$$$$$$/  $$$$$$$/ $$/   $$/  ",
                    "                                                                                  "
                    ], 500f),
                ]);
        }
        public static Sprite Logo3()
        {
            return new Sprite([
                new SpriteFrame([
                    "    ,                                                                                           ",
                    "    Et                                                                                          ",
                    "    E#t                           .                                                   .,G:      ",
                    "    E##t                         ;W                    j.                            ,WtE#,    :",
                    "    E#W#t             ..        f#E GEEEEEEEL GEEEEEEELEW,                   ..     i#D.E#t  .GE",
                    "    E#tfL.           ;W,      .E#f  ,;;L#K;;. ,;;L#K;;.E##j                 ;W,    f#f  E#t j#K;",
                    "    E#t             j##,     iWW;      t#E       t#E   E###D.              j##,  .D#i   E#GK#f  ",
                    " ,ffW#Dffj.        G###,    L##Lffi    t#E       t#E   E#jG#W;            G###, :KW,    E##D.   ",
                    "  ;LW#ELLLf.     :E####,   tLLG##L     t#E       t#E   E#t t##f         :E####, t#f     E##Wi   ",
                    "    E#t         ;W#DG##,     ,W#i      t#E       t#E   E#t  :K#E:      ;W#DG##,  ;#G    E#jL#D: ",
                    "    E#t        j###DW##,    j#E.       t#E       t#E   E#KDDDD###i    j###DW##,   :KE.  E#t ,K#j",
                    "    E#t       G##i,,G##,  .D#j         t#E       t#E   E#f,t#Wi,,,   G##i,,G##,    .DW: E#t   jD",
                    "    E#t     :K#K:   L##, ,WK,          t#E       t#E   E#t  ;#W:   :K#K:   L##,      L#,j#t     ",
                    "    E#t    ;##D.    L##, EG.            fE        fE   DWi   ,KK: ;##D.    L##,       jt ,;     ",
                    "    ;#t    ,,,      .,,  ,               :         :              ,,,      .,,                  ",
                    "     :;                                                                                         ",], 500f),
                ]);
        }
    }
}
