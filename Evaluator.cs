namespace chess
{
    public class Evaluator
    {
        private static Dictionary<int, int> PIECE_VALUES = new Dictionary<int, int>
    {
        { Piece.WHITE_PAWN, 1 },
        { Piece.WHITE_ROOK, 5 },
        { Piece.WHITE_KNIGHT, 3 },
        { Piece.WHITE_BISHOP, 3 },
        { Piece.WHITE_QUEEN, 9 },
        { Piece.BLACK_PAWN, -1 },
        { Piece.BLACK_ROOK, -5 },
        { Piece.BLACK_KNIGHT, -3 },
        { Piece.BLACK_BISHOP, -3 },
        { Piece.BLACK_QUEEN, -9 },
    };

        private static Position[] CENTER_SQUARES = new Position[] { new Position(3, 3), new Position(3, 4), new Position(4, 3), new Position(4, 4) };
        public static float evaluate(Board board)
        {
            float eval = 0;

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

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    int piece = board.getPiece(new Position(x, y));

                    if (PIECE_VALUES.Keys.Contains(piece))
                    {
                        value += PIECE_VALUES[piece];
                    }
                }
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

        private static float getCenterControl(Board board)
        {
            float value = 0;

            foreach (Position pos in CENTER_SQUARES)
            {
                int piece = board.getPiece(pos);

                if (Piece.isWhite(piece))
                {
                    value++;
                }

                if (Piece.isBlack(piece))
                {
                    value--;
                }
            }

            return value;
        }

        private static float getCheck(Board board)
        {
            if (board.isInCheck())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private static float getMate(Board board)
        {
            if (board.isInMate())
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}