using chess;

namespace transposition_table
{
    /// <summary>
    /// Class used to evaluate positions
    /// </summary>
    public class Evaluator : chess.Evaluator
    {
        private static ulong centerSquares = 0b0000000000000000000000000001100000011000000000000000000000000000;
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

        private new static float getCenterControl(Board board)
        {
            ulong whiteControl = BitBoard.GetAny(board, true) & centerSquares;
            ulong blackControl = BitBoard.GetAny(board, false) & centerSquares;

            int value = 0;
            while (whiteControl != 0 || blackControl != 0)
            {
                value += (int)(whiteControl & 1);
                value -= (int)(blackControl & 1);

                whiteControl >>= 1;
                blackControl >>= 1;
            }

            return value;
        }
    }
}