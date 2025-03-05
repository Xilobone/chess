using System.Text.RegularExpressions;

namespace chess
{
    /// <summary>
    /// Class used for checking the validity of moves
    /// </summary>
    public class Validator
    {
        /// <summary>
        /// Checks if a move string contains a valid move,
        /// does not check for validity against positions
        /// </summary>
        /// <param name="move_str">String containing the move</param>
        /// <returns>True if the move is valid, false otherwise</returns>
        public static bool isValid(string? move_str)
        {
            if (move_str == null)
            {
                return false;
            }

            string[] move = move_str.Split(' ');

            if (move.Length != 2)
            {
                return false;
            }

            return Regex.IsMatch(move[0], "^[A-Za-z][1-8]$") && Regex.IsMatch(move[1], "^[A-Za-z][1-8]$");
        }

        /// <summary>
        /// Gets the valid conversion from a move on the given board, if it exists
        /// </summary>
        /// <param name="move">The move to validify</param>
        /// <param name="board">The board on which the move is supposed to be made</param>
        /// <returns></returns>
        public static Move? getValidMove(Move move, Board board)
        {
            List<Move> possibleMoves = MoveGenerator.generateMoves(board, move.fr);

            //cannot return move, as the flags may be different
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