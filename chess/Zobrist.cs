namespace chess
{
    public static class Zobrist
    {
        private static ulong[,] pieceHash = new ulong[64, 12];
        private static ulong[] conditionsHash = new ulong[13];

        static Zobrist()
        {
            Random random = new Random(2);
            for (int pos = 0; pos < 64; pos++)
            {
                for (int piece = 0; piece < 12; piece++)
                {
                    pieceHash[pos, piece] = (ulong)random.NextInt64();
                    // Console.WriteLine($"hashing value ({pos},{piece}):{pieceHash[pos, piece]}");
                }
            }

            for (int condition = 0; condition < 13; condition++)
            {
                conditionsHash[condition] = (ulong)random.NextInt64();
                // Console.WriteLine($"hashing value ({condition}):{conditionsHash[condition]}");
                
            }
        }

        /// <summary>
        /// Creates a unique hash of the board
        /// </summary>
        /// <param name="board">The board to hash</param>
        /// <returns>a hash of the board</returns>
        public static ulong hash(Board board)
        {
            ulong hash = 0;

            for (int index = 0; index < 64; index++)
            {
                int piece = board.getPiece(index);

                if (piece == Piece.EMPTY) continue;

                //XOR hash and random value corresponding to piece on specified position
                hash ^= pieceHash[index, piece - 1];
            }

            if (board.whiteToMove)
            {
                hash ^= conditionsHash[0];
            }

            for (int i = 0; i < board.castlingOptions.Length; i++)
            {
                if (board.castlingOptions[i])
                {
                    hash ^= conditionsHash[i + 1];
                }
            }

            if (board.enpassantIndex != -1)
            {
                int file = Index.GetFile(board.enpassantIndex);

                hash ^= conditionsHash[file + 5];
            }

            return hash;
        }
    }
}