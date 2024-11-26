namespace chess
{
    public class Validator
    {
        public static Move? getValidMove(Move move, Board board)
        {
            List<Move> possibleMoves = MoveGenerator.generateMoves(board, move.fr);

            foreach (Move m in possibleMoves)
            {
                if (m.fr == move.fr && m.to == move.to)
                {
                    return m;
                }
            }

            return null;
        }
    }
}