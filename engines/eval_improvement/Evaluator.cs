using chess;

namespace eval_improvement
{
    /// <summary>
    /// Class used to evaluate positions
    /// </summary>
    public class Evaluator : chess.Evaluator
    {
        private static double[] pawnPSTable = {
            0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, 0.0,
            0.5,  1.0,  1.0, -2.0, -2.0,  1.0,  1.0, 0.5,
            0.5, -0.5, -1.0,  0.0,  0.0, -1.0, -0.5, 0.5,
            0.0,  0.0,  0.0,  2.0,  2.0,  0.0,  0.0, 0.0,
            0.5,  0.5,  1.0,  2.5,  2.5,  1.0,  0.5, 0.5,
            1.0,  1.0,  2.0,  3.0,  3.0,  2.0,  1.0, 1.0,
            5.0,  5.0,  5.0,  5.0,  5.0,  5.0,  5.0, 5.0,
            0.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, 0.0,
        };

        private static double[] knightPSTable = {
            -5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0,
            -4.0, -2.0,  0.0,  0.5,  0.5,  0.0, -2.0, -4.0,
            -3.0,  0.5,  1.0,  1.5,  1.5,  1.0,  0.5, -3.0,
            -3.0,  0.0,  1.5,  2.0,  2.0,  1.5,  0.0, -3.0,
            -3.0,  0.0,  1.5,  2.0,  2.0,  1.5,  0.0, -3.0,
            -3.0,  0.5,  1.0,  1.5,  1.5,  1.0,  0.5, -3.0,
            -4.0, -2.0,  0.0,  0.5,  0.5,  0.0, -2.0, -4.0,
            -5.0, -4.0, -3.0, -3.0, -3.0, -3.0, -4.0, -5.0,
        };

        private static double[] rookPSTable = {
             0.0, 0.0, 0.0, 0.5, 0.5, 0.0, 0.0,  0.0,
            -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5,
            -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5,
            -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5,
            -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5,
            -0.5, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, -0.5,
             0.5, 1.0, 1.0, 1.0, 1.0, 1.0, 1.0,  0.5,
             0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0,  0.0,
        };

        private static double[] bishopPSTable = {
            -2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0,
            -1.0,  0.5,  0.0,  0.0,  0.0,  0.0,  0.5, -1.0,
            -1.0,  1.0,  1.0,  1.0,  1.0,  1.0,  1.0, -1.0,
            -1.0,  0.0,  1.0,  1.0,  1.0,  1.0,  0.0, -1.0,
            -1.0,  0.5,  0.5,  1.0,  1.0,  0.5,  0.5, -1.0,
            -1.0,  0.0,  0.5,  1.0,  1.0,  0.5,  0.0, -1.0,
            -1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -1.0, 
            -2.0, -1.0, -1.0, -1.0, -1.0, -1.0, -1.0, -2.0,
        };

        private static double[] queenPSTable = {
            -2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -0.2,
            -1.0,  0.0,  0.5,  0.0,  0.0,  0.0,  0.0, -1.0,
            -1.0,  0.5,  0.5,  0.5,  0.5,  0.5,  0.0, -1.0,
             0.0,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -0.5,
            -0.5,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -0.5,
            -1.0,  0.0,  0.5,  0.5,  0.5,  0.5,  0.0, -1.0,
            -1.0,  0.0,  0.0,  0.0,  0.0,  0.0,  0.0, -1.0,
            -2.0, -1.0, -1.0, -0.5, -0.5, -1.0, -1.0, -0.2,
        };

        private static double[] kingPSTable = {
             2.0,  3.0,  1.0,  0.0,  0.0,  1.0,  3.0,  2.0,
             2.0,  2.0,  0.0,  0.0,  0.0,  0.0,  2.0,  2.0,
            -1.0, -2.0, -2.0, -2.0, -2.0, -2.0, -2.0, -1.0,
            -2.0, -3.0, -3.0, -4.0, -4.0, -3.0, -3.0, -2.0,
            -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0,
            -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0,
            -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0,
            -3.0, -4.0, -4.0, -5.0, -5.0, -4.0, -4.0, -3.0,
        };
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

            //return 0 if the board is a draw
            if (board.isInDraw()) return eval;

            //return highest or lowest possible eval if board is mate, what could be worse than losing
            //a game of chess?
            if (board.isInMate()) return board.whiteToMove ? -10000 : 10000;

            eval += 5 * getPieceValue(board);

            eval += 0.2f * getPawnChain(board);
            eval += 0.5f * getCenterControl(board);

            eval += 0.2f * getPieceSquareTable(board, pawnPSTable, Piece.WHITE_PAWN, Piece.BLACK_PAWN);
            eval += 0.2f * getPieceSquareTable(board, knightPSTable, Piece.WHITE_KNIGHT, Piece.BLACK_KNIGHT);
            eval += 0.2f * getPieceSquareTable(board, rookPSTable, Piece.WHITE_ROOK, Piece.BLACK_ROOK);
            eval += 0.2f * getPieceSquareTable(board, bishopPSTable, Piece.WHITE_BISHOP, Piece.BLACK_BISHOP);
            eval += 0.2f * getPieceSquareTable(board, queenPSTable, Piece.WHITE_QUEEN, Piece.BLACK_QUEEN);
            eval += 0.2f * getPieceSquareTable(board, kingPSTable, Piece.WHITE_KING, Piece.BLACK_KING);

            eval += 0.5f * getCheck(board);

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
                value += (int)(whiteControl & 1);
                value -= (int)(blackControl & 1);

                whiteControl >>= 1;
                blackControl >>= 1;
            }

            return value;
        }

        private static float getPieceSquareTable(Board board, double[] pieceSquareTable, int whitePiece, int blackPiece)
        {
            double value = 0;
            for (int i = 0; i < 64; i++)
            {
                if (board.getPiece(i) == whitePiece) value += pieceSquareTable[i];
                if (board.getPiece(i) == blackPiece)
                {
                    int flipped = (7 - (i / 8)) * 8 + (i % 8);
                    value -= pieceSquareTable[flipped];
                }
            }

            return (float)value;
        }
    }
}