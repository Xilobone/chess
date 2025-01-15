namespace chess
{
    public static class BitBoard
    {
        public static long ComputeInitial(Board board)
        {
            long bitBoard = 0b0L;

            List<Move> moves = MoveGenerator.generateAllMoves(board);

            foreach(Move move in moves)
            {   
                //skip over pawn advancements, as they cannot capure
                int piece = board.getPiece(move.fr);
                if((piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN) && move.fr.x == move.to.x)
                {
                    continue;
                }

                int bit = move.to.x + (7 - move.to.y) * 8;

                bitBoard = bitBoard | (1L << bit);
            }

            return bitBoard;

        }
    }
}