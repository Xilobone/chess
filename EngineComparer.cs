using chess;
using parser;

namespace engine_comparer
{
    public class EngineComparer
    {
        public Player[] whitePlayers = {
            new Player("player", new player.Player(), new player.Evaluator()),
            new Player("minimax", new minimax_engine.Engine(), new minimax_engine.Evaluator()),
            new Player("deep minimax", new deep_minimax_engine.Engine(), new deep_minimax_engine.Evaluator())
        };

        public Player[] blackPlayers = {
            new Player("player", new player.Player(), new player.Evaluator()),
            new Player("minimax", new minimax_engine.Engine(), new minimax_engine.Evaluator()),
            new Player("deep minimax", new deep_minimax_engine.Engine(), new deep_minimax_engine.Evaluator())
        };

        public EngineComparer()
        {

            Player white = selectPlayer(true);
            Player black = selectPlayer(false);

            white.engine.isWhite = true;
            black.engine.isWhite = false;

            ChessPlayerSettings settings = new ChessPlayerSettings(100, 0, 0);
            ChessPlayer player = new ChessPlayer(white, black, settings);

            Console.Write("Enter the starting fen (or leave empty for the standard position):");
            string? fen = Console.ReadLine();

            if (string.IsNullOrEmpty(fen)) player.Play();
            else player.Play(fen);
        }

        private Player selectPlayer(bool isWhite) 
        {
            Player[] players = isWhite ? whitePlayers : blackPlayers;
            string color = isWhite ? "white" : "black";
            Console.WriteLine("Please select a " + color + " player:");

            for (int i = 0; i < players.Length; i++)
            {
                Console.WriteLine(i + ": " + players[i].name);
            }

            int index = int.Parse(Console.ReadLine()!);

            return players[index];
        }
        public static void Main(string[] args)
        {
            new EngineComparer();
            //GamesParser parser = new GamesParser("./lib/chess_games.pgn");
            //parser.parse(10000,10);
        }
    }
}