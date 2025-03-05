using chess;

namespace player
{
    /// <summary>
    /// Class that acts as the human player of a game of chess
    /// </summary>
    public class Player : chess.engine.Engine
    {
        /// <summary>
        /// Creates a new player object
        /// </summary>
        public Player() : base(true, new Evaluator()) { }

        /// <summary>
        /// Asks the user to input a legal move for the given board
        /// </summary>
        /// <param name="board">The board to make a move on</param>
        /// <param name="maxTime">The maximum allowed time, unused</param>
        /// <returns>The move the player makes</returns>
        public override Move makeMove(Board board, float maxTime)
        {
            return makeMove(board);
        }

        /// <summary>
        /// Asks the user to input a legal move for the given board
        /// </summary>
        /// <param name="board">The board to make a move on</param>
        /// <returns>The move the player makes</returns>
        public override Move makeMove(Board board)
        {
            Console.Write("Please make a move:");
            string? inp = Console.ReadLine();

            if (!Validator.isValid(inp))
            {
                Console.WriteLine("This is not a valid move");
                return makeMove(board);
            }

            Move move = Move.getMove(inp!);

            Move? validMove = Validator.getValidMove(move, board);

            if (validMove == null)
            {
                Console.WriteLine("This move is not possible on the current board");
                return makeMove(board);
            }

            if (Move.FLAG_PROMOTIONS.Contains(validMove.flag))
            {
                validMove.flag = requestPromotion();
            }

            return validMove;
        }

        private int requestPromotion()
        {
            Console.Write("What do you want to promote to (type q,r,b or n):");
            string? promotion = Console.ReadLine();


            while (!Move.PROMOTION_VALUES.Keys.Contains(promotion!))
            {
                Console.Write("The entered value was not correct, please try again:");
                promotion = Console.ReadLine();
            }

            return Move.PROMOTION_VALUES[promotion!];
        }
    }
}