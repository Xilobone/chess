using chess;

namespace iterative_deepening
{
    public class Evaluator : IEvaluator
    {
        private static Dictionary<int, int> PIECE_VALUES = new Dictionary<int, int>
    {
        { Piece.WHITE_PAWN, 1 },
        { Piece.WHITE_ROOK, 5 },
        { Piece.WHITE_KNIGHT, 3 },
        { Piece.WHITE_BISHOP, 3 },
        { Piece.WHITE_QUEEN, 9 },
        { Piece.WHITE_KING, 0 },
        { Piece.BLACK_PAWN, -1 },
        { Piece.BLACK_ROOK, -5 },
        { Piece.BLACK_KNIGHT, -3 },
        { Piece.BLACK_BISHOP, -3 },
        { Piece.BLACK_QUEEN, -9 },
        { Piece.BLACK_KING, 0 },
        { Piece.EMPTY, 0 },

    };

        private static ulong centerSquares = 0b0000000000000000000000000001100000011000000000000000000000000000;
        public float evaluate(Board board)
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

        private static float getPieceValue(Board board)
        {
            float value = 0;

            for (int i = 0; i < 64; i++)
            {
                int piece = board.getPiece(i);

                value += PIECE_VALUES[piece];
            }

            return value;
        }

        private static float getPawnChain(Board board)
        {
            float value = 0;

            Position whiteSupport1 = new Position(-1, -1);
            Position whiteSupport2 = new Position(1, -1);

            Position blackSupport1 = new Position(-1, 1);
            Position blackSupport2 = new Position(1, 1);

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Position pos = new Position(x, y);
                    int piece = board.getPiece(pos);

                    if (piece == Piece.WHITE_PAWN)
                    {
                        Position[] supports = new Position[] { pos + whiteSupport1, pos + whiteSupport2 };

                        foreach (Position sup in supports)
                        {
                            if (sup.x >= 0 && sup.x <= 7)
                            {
                                if (board.getPiece(sup) == Piece.WHITE_PAWN)
                                {
                                    value++;
                                }
                            }
                        }
                    }

                    if (piece == Piece.BLACK_PAWN)
                    {
                        Position[] supports = new Position[] { pos + blackSupport1, pos + blackSupport2 };

                        foreach (Position sup in supports)
                        {
                            if (sup.x >= 0 && sup.x <= 7)
                            {
                                if (board.getPiece(sup) == Piece.BLACK_PAWN)
                                {
                                    value--;
                                }
                            }
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the amount of center control white has more than black
        /// </summary>
        /// <param name="board">The board of which the center control to compute from</param>
        /// <returns>a float indicating the amount of center control white has over black</returns>
        private static float getCenterControl(Board board)
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

        private static float getCheck(Board board)
        {
            if (!board.isInCheck()) return 0;

            return board.whiteToMove ? -1 : 1;
        }

        private static float getMate(Board board)
        {
            if (!board.isInMate()) return 0;

            return board.whiteToMove ? -1 : 1;
        }
    }
}