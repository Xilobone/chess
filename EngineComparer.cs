using System.Runtime.CompilerServices;
using chess;
using chessPlayer;
using parser;

namespace engine_comparer
{
    public class EngineComparer
    {

        // public EngineComparer(Player white, Player black)
        // {
        //     white.engine.isWhite = true;
        //     black.engine.isWhite = false;

        //     ChessPlayerSettings settings = new ChessPlayerSettings(100, 0, 0, false);
        //     ChessPlayer player = new ChessPlayer(white, black, settings);

        //     Console.Write("Enter the starting fen (or leave empty for the standard position):");
        //     string? fen = Console.ReadLine();

        //     if (string.IsNullOrEmpty(fen)) player.Play();
        //     else player.Play(fen);
        // }


        public static void CompareFromUserInput()
        {
            //request players from user
            Player white = PlayerList.selectPlayer(true);
            Player black = PlayerList.selectPlayer(false);

            ChessPlayerSettings settings = new ChessPlayerSettings(10, 0, 0, false);
            //get list of boards to play
            IEvaluator[] evaluators = new IEvaluator[] { new deep_minimax_engine.Evaluator() };
            GamesParser parser = new GamesParser("./lib/chess_games.pgn", evaluators, 1);
            List<Board> boards = parser.parse(300, 50, 0.1f);

            int winsWhite = 0;
            int winsBlack = 0;
            int draws = 0;

            //swap players each turn to eliminate white's base advantage
            //currently not implemented, lead to bugs, lookat another time
            // bool swapped = false;

            foreach(Board board in boards)
            {

                ChessPlayer chessPlayer = new ChessPlayer(white, black, settings);
                
                GameResult gameResult = chessPlayer.Play(board.toFen());

                if (gameResult.result == 1 || gameResult.finalEval > 0.5f)
                {
                    winsWhite++;
                } else if (gameResult.result == -1 || gameResult.finalEval < -0.5f)
                {
                    winsBlack++;
                } else {
                    draws++;
                }
                
                Console.WriteLine($"Played {winsWhite + winsBlack + draws}/{boards.Count} games (white:{winsWhite}, black:{winsBlack}, draw:{draws})");
            }

            Console.WriteLine($"Win rate (white):{(double) winsWhite / (winsWhite + winsBlack)}");
            Console.WriteLine($"Win rate (black):{(double) winsBlack / (winsWhite + winsBlack)}");
        }
    }
}