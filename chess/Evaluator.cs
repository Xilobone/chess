namespace chess
{
    public abstract class Evaluator
    {
        protected static Dictionary<int, int> PIECE_VALUES = new Dictionary<int, int>
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
        { Piece.BLACK_KING, 0},
        { Piece.EMPTY, 0}
    };

        protected static int[] CENTER_SQUARES = { 27, 28, 35, 36 };

        /// <summary>
        /// Evaluated the given board
        /// </summary>
        /// <param name="board">The board to evaluate</param>
        /// <returns>The evaluation of the board, higher values indicate white has the adventage,
        /// lower values indicate black has the advantage</returns>
        public abstract float evaluate(Board board);

        protected static float getPieceValue(Board board)
        {
            float value = 0;

            for (int i = 0; i < 64; i++)
            {
                int piece = board.getPiece(i);

                value += PIECE_VALUES[piece];

            }

            return value;
        }

        protected static float getPawnChain(Board board)
        {
            float value = 0;

            for (int i = 0; i < 64; i++)
            {
                int piece = board.getPiece(i);
                int rank = chess.Index.GetRank(i);

                if (piece == Piece.WHITE_PAWN)
                {
                    if (rank != 0 && board.getPiece(i - 9) == Piece.WHITE_PAWN) value++;
                    if (rank != 7 && board.getPiece(i - 7) == Piece.WHITE_PAWN) value++;
                }
                else if (piece == Piece.BLACK_PAWN)
                {
                    if (rank != 0 && board.getPiece(i + 7) == Piece.BLACK_PAWN) value--;
                    if (rank != 7 && board.getPiece(i + 9) == Piece.BLACK_PAWN) value--;

                }
            }

            return value;
        }

        protected static float getCenterControl(Board board)
        {
            float value = 0;

            foreach (int index in CENTER_SQUARES)
            {
                int piece = board.getPiece(index);

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

        /// <summary>
        /// Checks if the given board is in check
        /// </summary>
        /// <param name="board">The board to check if it is in check</param>
        /// <returns>1 if black is in check, -1 if white is in check and 0 otherwise</returns>
        protected static float getCheck(Board board)
        {
            if (!board.isInCheck()) return 0;

            return board.whiteToMove ? -1 : 1;
        }

        /// <summary>
        /// Checks if the given board is in mate
        /// </summary>
        /// <param name="board">The board to check if it is in mate</param>
        /// <returns>1 if black is in mate, -1 if white is in mate and 0 otherwise</returns>
        protected static float getMate(Board board)
        {
            if (!board.isInMate()) return 0;

            return board.whiteToMove ? -1 : 1;
        }
    }
}