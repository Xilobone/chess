using chess;

namespace improved_minimax_engine
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
            float eval = 0;

            if (board.isInDraw()) return eval;

            eval += 2 * getPieceValue(board);
            eval += 0.2f * getPawnChain(board);
            eval += 0.5f * getCenterControl(board);
            eval += 0.5f * getCheck(board);
            eval += 100000 * getMate(board);

            return eval;
        }
    }
}