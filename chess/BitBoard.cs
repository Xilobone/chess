namespace chess
{
    public static class BitBoard
    {
        public const int PAWN = 0;
        public const int KNIGHT = 1;
        public const int BISHOP = 2;
        public const int ROOK = 3;
        public const int QUEEN = 4;
        public const int KING = 5;

        public const int PAWN_ATTACK = 6;
        public const int KNIGHT_ATTACK = 7;
        public const int BISHOP_ATTACK = 8;
        public const int ROOK_ATTACK = 9;
        public const int QUEEN_ATTACK = 10;
        public const int KING_ATTACK = 11;

        public static long[] ComputeAll(Board board, bool forWhite)
        {
            long[] bitboards = new long[12];


            bitboards[PAWN] = Compute(board, PAWN, forWhite);
            bitboards[KNIGHT] = Compute(board, KNIGHT, forWhite);
            bitboards[BISHOP] = Compute(board, BISHOP, forWhite);
            bitboards[ROOK] = Compute(board, ROOK, forWhite);
            bitboards[QUEEN] = Compute(board, QUEEN, forWhite);
            bitboards[KING] = Compute(board, KING, forWhite);

            return bitboards;

        }
        /// <summary>
        /// Computes a specific type of bitboard
        /// </summary>
        /// <param name="board">The board to generate the bitboard from</param>
        /// <param name="type">The type of bitboard to create</param>
        /// <param name="forWhite">True if the bitboard created should be for white, false if for black</param>
        /// <returns>a long containing the bitboard</returns>
        public static long Compute(Board board, int type, bool forWhite)
        {
            if (type <= 6)
            {
                int piece = 0;

                switch (forWhite, type)
                {
                    case (true, PAWN): piece = Piece.WHITE_PAWN; break;
                    case (true, KNIGHT): piece = Piece.WHITE_KNIGHT; break;
                    case (true, BISHOP): piece = Piece.WHITE_BISHOP; break;
                    case (true, ROOK): piece = Piece.WHITE_ROOK; break;
                    case (true, QUEEN): piece = Piece.WHITE_QUEEN; break;
                    case (true, KING): piece = Piece.WHITE_KING; break;
                    case (false, PAWN): piece = Piece.BLACK_PAWN; break;
                    case (false, KNIGHT): piece = Piece.BLACK_KNIGHT; break;
                    case (false, BISHOP): piece = Piece.BLACK_BISHOP; break;
                    case (false, ROOK): piece = Piece.BLACK_ROOK; break;
                    case (false, QUEEN): piece = Piece.BLACK_QUEEN; break;
                    case (false, KING): piece = Piece.BLACK_KING; break;
                    default: break;
                }

                return ComputePieceBitboard(board, piece);
            }

            return 0;
        }

        private static long ComputePieceBitboard(Board board, int piece)
        {
            long bitboard = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board.pieces[x, y] != piece)
                    {
                        continue;
                    }

                    int bit = x + y * 8;

                    bitboard = bitboard | (1L << bit);
                }
            }

            return bitboard;
        }
    }


}