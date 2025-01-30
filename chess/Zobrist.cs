namespace chess
{
    public static class Zobrist
    {
        private static int[,] pieceHash = new int[64,12];
        private static int[] conditionsHash = new int[13];

        static Zobrist()
        {
            Random random = new Random(0);
            for(int pos = 0; pos < 64; pos++)
            {
                for (int piece = 0; piece < 12; piece++)
                {
                    pieceHash[pos, piece] = random.Next();
                }
            }

            for(int condition = 0; condition < 13; condition++)
            {
                conditionsHash[condition] = random.Next();
            }
        }

        /// <summary>
        /// Creates a unique hash of the board
        /// </summary>
        /// <param name="board">The board to hash</param>
        /// <returns>a hash of the board</returns>
        public static int hash(Board board)
        {
            int hash = 0;

            for(int index = 0; index < 64; index++)
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

            for(int i = 0; i < board.castlingOptions.Length; i++)
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