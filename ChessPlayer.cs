namespace chess
{
    public class ChessPlayer
    {
        private IPlayer white;
        private IPlayer black;
        public ChessPlayer(IPlayer whitePlayer, IPlayer blackPlayer)
        {
            white = whitePlayer;
            black = blackPlayer;
        }

        public void Play()
        {
            Play(Board.START_FEN);
        }

        public void Play(string fen)
        {
            Board board = Board.fromFen(fen);

            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //runs the game
            while (!board.isInMate())
            {
                board.display();

                Move move;
                if (board.whiteToMove)
                {
                    move = white.makeMove(board);
                }
                else
                {
                    move = black.makeMove(board);
                }
                
                board.makeMove(move);
            }

            board.display();
            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            Console.WriteLine("total elapsed time:" + time + "ms");
        }
    }
}