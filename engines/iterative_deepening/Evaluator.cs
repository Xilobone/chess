using chess;

namespace iterative_deepening
{
    public class Evaluator : chess.Evaluator
    {
        private static ulong centerSquares = 0b0000000000000000000000000001100000011000000000000000000000000000;
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

        /// <summary>
        /// Gets the amount of center control white has more than black
        /// </summary>
        /// <param name="board">The board of which the center control to compute from</param>
        /// <returns>a float indicating the amount of center control white has over black</returns>
        private static new float getCenterControl(Board board)
        {
            ulong whiteControl = BitBoard.GetAny(board, true) & centerSquares;
            ulong blackControl = BitBoard.GetAny(board, false) & centerSquares;

            int value = 0;
            while (whiteControl != 0 || blackControl != 0)
            {
                value += (int) (whiteControl & 1);
                value -= (int) (blackControl & 1);

                whiteControl >>= 1;
                blackControl >>= 1;
            }

            return value;
        }
    }
}