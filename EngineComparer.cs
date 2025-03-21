using chess;
using chessPlayer;
using parser;

namespace chessTesting
{
    /// <summary>
    /// Class used to compare two different engines by letting them play multiple games
    /// </summary>
    public static class EngineComparer
    {   
        /// <summary>
        /// Asks the user to input the engines to compare and the amount of games to play,
        /// Then plays the games and logs the results
        /// </summary>
        public static void CompareFromUserInput()
        {
            //request players from user
            IPlayer white = PlayerList.selectPlayer(true);
            IPlayer black = PlayerList.selectPlayer(false);

            ChessPlayerSettings settings = ChessPlayerSettings.AskUserForSettings();
            //get list of boards to play
            Evaluator[] evaluators = new Evaluator[] { new minimax_engine.Evaluator() };
            GamesParser parser = new GamesParser("./lib/chess_games.pgn", evaluators, 1);
            List<Board> boards = parser.parse(5000, 10, 0.1f);

            int winsWhite = 0;
            int winsBlack = 0;
            int draws = 0;

            //swap players each turn to eliminate white's base advantage
            //currently not implemented, lead to bugs, lookat another time
            // bool swapped = false;
            foreach (Board board in boards)
            {
                ChessPlayer chessPlayer = new ChessPlayer(white, black, settings);

                GameResult gameResult = chessPlayer.Play(board.toFen());

                if (gameResult.result == GameResult.Result.WinWhite || gameResult.finalEval > 0.5f)
                {
                    winsWhite++;
                }
                else if (gameResult.result == GameResult.Result.WinBlack || gameResult.finalEval < -0.5f)
                {
                    winsBlack++;
                }
                else
                {
                    draws++;
                }

                Console.WriteLine($"Played {winsWhite + winsBlack + draws}/{boards.Count} games (white:{winsWhite}, black:{winsBlack}, draw:{draws})");
            }

            Console.WriteLine($"Win rate (white):{(double)winsWhite / (winsWhite + winsBlack)}");
            Console.WriteLine($"Win rate (black):{(double)winsBlack / (winsWhite + winsBlack)}");
        }
    }
}