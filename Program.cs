namespace FastTrackGame
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Set constants
            const int SizeX = 100;
            const int SizeY = 26;
            const int FrameRate = 100;
            Game.DebugPrint = false;

            //Console setup
            Console.ForegroundColor = ConsoleColor.Green;
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);

            //Start the game
            Game game = new Game(FrameRate, SizeX, SizeY);
            game.Start();
            Environment.Exit(0);
        }
    }
}
