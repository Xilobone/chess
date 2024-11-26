using chess;

namespace player
{
    public class Player : IEngine
    {
        public bool isWhite { get; set; }

        public Move makeMove(Board board, float maxTime)
        {
            return makeMove(board);
        }
        public Move makeMove(Board board)
        {
            Console.Write("Please make a move:");
            string? inp = Console.ReadLine();

            if (!Move.isValid(inp))
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