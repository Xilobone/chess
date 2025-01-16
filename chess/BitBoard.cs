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

        private static ulong aFile = 0b1000000010000000100000001000000010000000100000001000000010000000;
        private static ulong abFile = 0b1100000011000000110000001100000011000000110000001100000011000000;
        private static ulong ghFile = 0b0000001100000011000000110000001100000011000000110000001100000011;
        private static ulong hFile = 0b0000000100000001000000010000000100000001000000010000000100000001;

        public static ulong[] ComputeAll(Board board, bool forWhite)
        {
            ulong[] bitboards = new ulong[6];

            bitboards[PAWN] = Compute(board, PAWN, forWhite);
            bitboards[KNIGHT] = Compute(board, KNIGHT, forWhite);
            bitboards[BISHOP] = Compute(board, BISHOP, forWhite);
            bitboards[ROOK] = Compute(board, ROOK, forWhite);
            bitboards[QUEEN] = Compute(board, QUEEN, forWhite);
            bitboards[KING] = Compute(board, KING, forWhite);

            return bitboards;
        }

        public static ulong[] ComputeAllAttack(Board board, bool forWhite)
        {

            ulong[] bitboards = new ulong[6];

            bitboards[PAWN] = ComputePawnAttackingBitboard(board, forWhite);
            bitboards[KNIGHT] = ComputeKnightAttackingBitboard(board, forWhite);
            bitboards[KING] = ComputeKingAttackingBitboard(board, forWhite);

            return bitboards;
        }
        /// <summary>
        /// Computes a specific type of bitboard
        /// </summary>
        /// <param name="board">The board to generate the bitboard from</param>
        /// <param name="type">The type of bitboard to create</param>
        /// <param name="forWhite">True if the bitboard created should be for white, false if for black</param>
        /// <returns>a long containing the bitboard</returns>
        public static ulong Compute(Board board, int type, bool forWhite)
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

        private static ulong ComputePieceBitboard(Board board, int piece)
        {
            ulong bitboard = 0;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (board.pieces[x, y] != piece)
                    {
                        continue;
                    }

                    int bit = x + y * 8;

                    bitboard = bitboard | (1uL << bit);
                }
            }

            return bitboard;
        }

        private static ulong ComputePawnAttackingBitboard(Board board, bool forWhite)
        {
            ulong pawnBitboard = forWhite ? board.bitboardsWhite[PAWN] : board.bitboardsBlack[PAWN];

            ulong left;
            ulong right;
            if (forWhite)
            {
                left = (pawnBitboard << 7) & ~aFile;
                right = (pawnBitboard << 9) & ~hFile;
            }
            else
            {
                left = (pawnBitboard >> 9) & ~aFile;
                right = (pawnBitboard >> 7) & ~hFile;
            }

            //set bits of right and left attack, unless wrapping around the board

            return left | right;
        }

        private static ulong ComputeKnightAttackingBitboard(Board board, bool forWhite)
        {
            ulong knightBitboard = forWhite ? board.bitboardsWhite[KNIGHT] : board.bitboardsBlack[KNIGHT];

            ulong attackBitboard = 0;

            attackBitboard |= (knightBitboard >> 6) & ~ghFile;
            attackBitboard |= (knightBitboard >> 10) & ~abFile;
            attackBitboard |= (knightBitboard >> 15) & ~hFile;
            attackBitboard |= (knightBitboard >> 17) & ~aFile;

            attackBitboard |= (knightBitboard << 6) & ~abFile;
            attackBitboard |= (knightBitboard << 10) & ~ghFile;
            attackBitboard |= (knightBitboard << 15) & ~aFile;
            attackBitboard |= (knightBitboard << 17) & ~hFile;

            return attackBitboard;
        }

        private static ulong ComputeKingAttackingBitboard(Board board, bool forWhite)
        {
            ulong kingBitboard = forWhite ? board.bitboardsWhite[KING] : board.bitboardsBlack[KING];

            ulong attackBitboard = 0;

            attackBitboard |= (kingBitboard >> 9) & ~aFile;
            attackBitboard |= (kingBitboard >> 8);
            attackBitboard |= (kingBitboard >> 7) & ~hFile;

            attackBitboard |= (kingBitboard >> 1) & ~aFile;
            attackBitboard |= (kingBitboard << 1) & ~hFile;

            attackBitboard |= (kingBitboard << 7) & ~aFile;
            attackBitboard |= (kingBitboard << 8);
            attackBitboard |= (kingBitboard << 9) & ~hFile;

            return attackBitboard;
        }
    }


}