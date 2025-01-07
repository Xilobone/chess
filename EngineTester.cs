using chess;

namespace chessTesting 
{
    public class EngineTester
    {
        public static void testSinglePosition()
        {
            Player player = PlayerList.selectPlayer(true);

            Console.Write("Enter the starting fen (or leave empty for the standard position):");
            string? fen = Console.ReadLine();

            Board board;
            if (string.IsNullOrEmpty(fen)) board = Board.startPosition();
            else board = Board.fromFen(fen);

            player.engine.isWhite = board.whiteToMove;
            player.engine.displayStats = true;

            board.display();
            Move move = player.engine.makeMove(board);
            board.makeMove(move).display();
        }
    }
}