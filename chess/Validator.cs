namespace chess
{
    public class Validator
    {
        public static Move? getValidMove(Move move, Board board)
        {
            List<Move> possibleMoves = MoveGenerator.generateMoves(board, move.fr);

            foreach (Move m in possibleMoves)
            {
                if (m.frIndex == move.frIndex && m.toIndex == move.toIndex)
                {
                    return m;
                }
            }

            return null;
        }
    }
}