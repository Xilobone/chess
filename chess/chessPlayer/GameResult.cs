using chess;

namespace chessPlayer
{   
    /// <summary>
    /// Stores the result of a game of chess
    /// </summary>
    public class GameResult
    {      
        /// <summary>
        /// The result of the game, 1: white won, -1: black won, 0: draw
        /// </summary>
        public float result { get; private set; }

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
        public GameResult(float result, float whiteEval, float blackEval)
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
            float result = 0;
            if (board.isInMate())
            {
                result = board.whiteToMove ? -1 : 1;
            }

            return new GameResult(result, white.engine.evaluator.evaluate(board), black.engine.evaluator.evaluate(board));
        }

        /// <summary>
        /// Creates a string representation of the game result
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string winner = "";
            switch (result)
            {
                case 0: winner = "draw"; break;
                case 1: winner = "white won"; break;
                case -1: winner = "black won"; break;
            }
            return $"{winner}, eval: white:{whiteEval}, black:{blackEval}";
        }
    }
}