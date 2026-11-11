using System;
using System.IO;
using System.Linq;

namespace FastTrackGame
{
    internal static class HighScores
    {
        //Save right besides the app itself
        private static readonly string FilePath = "highscores.txt";

        public static void AddSave(int points)
        {
            File.AppendAllText(FilePath, points + Environment.NewLine);
        }


        public static int[] LoadSaves(int num)
        {
            if (!File.Exists(FilePath))
                return Array.Empty<int>();

            //Cool little linq query to take top X scores
            return File.ReadAllLines(FilePath).Select(int.Parse).OrderByDescending(x => x).Take(Math.Max(0, num)).ToArray();
        }

        public static int GetLastSave()
        {
            if (!File.Exists(FilePath))
                return -1;
            var the = File.ReadAllLines(FilePath).Last();
            if (int.TryParse(the, out int num))
                return num;
            return -1;
        }
    }
}
