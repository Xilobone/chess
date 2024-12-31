namespace chess
{
    public class ChessPlayer
    {
        private static ChessPlayerSettings DEFAULT_SETTINGS = new ChessPlayerSettings(false, 0, false, 0, false,0);
        private Player white;
        private Player black;

        private Board? board;

        private ChessPlayerSettings settings;
        private long runningTime;

        public ChessPlayer(Player white, Player black) : this(white, black, DEFAULT_SETTINGS) { }

        public ChessPlayer(Player white, Player black, ChessPlayerSettings settings)
        {
            this.white = white;
            this.black = black;
            this.settings = settings;
        }

        public void Play()
        {
            Play(Board.START_FEN);
        }

        public void Play(string fen)
        {
            board = Board.fromFen(fen);
            runningTime = 0;
            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //runs the game
            while (!stopConditionMet())
            {
                board.display();
                Console.WriteLine("White's evaluation: " + white.evaluator.evaluate(board));
                Console.WriteLine("Black's evaluation: " + black.evaluator.evaluate(board));

                Move move;

                //select correct player to make a move
                switch((board.whiteToMove, settings.limitedTurnTime))
                {
                    case (true, true): move = white.engine.makeMove(board, settings.maxTurnTime); break;
                    case (true, false): move = white.engine.makeMove(board); break;
                    case (false, true): move = black.engine.makeMove(board, settings.maxTurnTime); break;
                    case (false, false): move = black.engine.makeMove(board); break;
                }

                board = board.makeMove(move);
                runningTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            }

            board.display();
            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            Console.WriteLine("total elapsed time:" + time + "ms");
        }

        private bool stopConditionMet() 
        {
            if (board == null)
            {
                return false;
            }

            if (board.isInMate()) 
            {
                return true;
            }

            if (settings.limitedTurns && board.fullMoves > settings.maxTurns)
            {
                return true;
            }

            if (settings.limitedTime && runningTime > settings.maxTime)
            {
                return true;
            }

            return false;
        }
    }
}