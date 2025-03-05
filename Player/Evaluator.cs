using chess;

namespace player
{
    /// <summary>
    /// Class used to evaluate positions
    /// </summary>
    public class Evaluator : chess.Evaluator
    {
        /// <summary>
        /// Evaluates a given board, a higher evaluation indicates white has the advantage,
        /// a lower evaluation indicates black has the advantage
        /// </summary>
        /// <param name="board">The board to evaluate</param>
        /// <returns>The evaluation of the board</returns>
        public override float evaluate(Board board)
        {
            return 0;
        }
    }
}