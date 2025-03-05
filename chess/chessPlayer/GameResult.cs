using chess;

namespace chessPlayer
{   
    /// <summary>
    /// Stores the result of a game of chess
    /// </summary>
    public class GameResult
    {   
        /// <summary>
        /// The possible game results
        /// </summary>
        public enum Result
        {   
            /// <summary>
            /// The game is still ongoing
            /// </summary>
            Ongoing,

            /// <summary>
            /// White has won
            /// </summary>
            WinWhite,

            /// <summary>
            /// Black has won
            /// </summary>
            WinBlack,

            /// <summary>
            /// The game is a draw by repitition
            /// </summary>
            DrawRepitition,

            /// <summary>
            /// The game is a draw by stalemate
            /// </summary>
            DrawStalemate,

            /// <summary>
            /// The game is a draw by the 50 move rule
            /// </summary>
            DrawFiftyMove
        }  
        /// <summary>
        /// The result of the game
        /// </summary>
        public Result result { get; private set; }

        /// <summary>
        /// Whites final evaluation of the board
        /// </summary>
        public float whiteEval { get; private set; }

        /// <summary>
        /// Blacks final evaluation of the board
        /// </summary>
        public float blackEval { get; private set; }

        /// <summary>
        /// The average final evaluation of the board
        /// </summary>
        public float finalEval { get; private set; }

        /// <summary>
        /// Creates a new game result
        /// </summary>
        /// <param name="result">The result of the game</param>
        /// <param name="whiteEval">Whites final evaluation</param>
        /// <param name="blackEval">Blacks final evaluation</param>
        public GameResult(Result result, float whiteEval, float blackEval)
        {
            this.result = result;
            this.whiteEval = whiteEval;
            this.blackEval = blackEval;

            finalEval = (whiteEval + blackEval) / 2;
        }

        /// <summary>
        /// Gets the result from a board
        /// </summary>
        /// <param name="board">The board to get the result from</param>
        /// <param name="white">The white player</param>
        /// <param name="black">The black player</param>
        /// <returns></returns>
        public static GameResult GetResult(Board board, IPlayer white, IPlayer black)
        {
            Result result;
            if (board.isInMate())
            {
                result = board.whiteToMove ? Result.WinBlack : Result.WinWhite;
            }
            else
            {
                result = board.isADraw();
            }

            return new GameResult(result, white.engine.evaluator.evaluate(board), black.engine.evaluator.evaluate(board));
        }

        /// <summary>
        /// Creates a string representation of the game result
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string res = "";
            switch (result)
            {
                case Result.Ongoing: res = "game is ongoing"; break;
                case Result.WinWhite: res = "white won"; break;
                case Result.WinBlack: res = "black won"; break;
                case Result.DrawStalemate: res = "draw (stalemate)"; break;
                case Result.DrawRepitition: res = "raw (repitition)"; break;
                case Result.DrawFiftyMove: res = "raw (50 move rule)"; break;
            }
            return $"{res}, eval: white:{whiteEval}, black:{blackEval}";
        }
    }
}