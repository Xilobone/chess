using chess;

namespace chessPlayer
{
    public class ChessPlayer
    {
        private Player white;
        private Player black;

        private Board? board;

        private ChessPlayerSettings settings;
        private long runningTime;

        private int turnsAtStart;
        private bool whiteStarted;

        public ChessPlayer(Player white, Player black) : this(white, black, ChessPlayerSettings.DEFAULT_SETTINGS) { }

        public ChessPlayer(Player white, Player black, ChessPlayerSettings settings)
        {
            this.white = white;
            this.black = black;
            this.settings = settings;

            this.white.engine.displayStats = settings.displayBoards;
            this.black.engine.displayStats = settings.displayBoards;

            this.white.engine.isWhite = true;
            this.black.engine.isWhite = false;
        }

        public GameResult Play()
        {
            return Play(Board.START_FEN);
        }

        public GameResult Play(string fen)
        {
            board = Board.fromFen(fen);

            turnsAtStart = board.fullMoves;
            whiteStarted = board.whiteToMove;
            runningTime = 0;
            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //runs the game
            while (!stopConditionMet())
            {
                if (settings.displayBoards)
                {
                    board.display();
                    Console.WriteLine("White's evaluation: " + white.evaluator.evaluate(board));
                    Console.WriteLine("Black's evaluation: " + black.evaluator.evaluate(board));
                }
                Move move;

                //select correct player to make a move
                switch ((board.whiteToMove, settings.limitedTurnTime))
                {
                    case (true, true): move = white.engine.makeMove(board, settings.maxTurnTime); break;
                    case (true, false): move = white.engine.makeMove(board); break;
                    case (false, true): move = black.engine.makeMove(board, settings.maxTurnTime); break;
                    case (false, false): move = black.engine.makeMove(board); break;
                }

                if (settings.displayBoards) Console.WriteLine($"move: {move}");
                board = board.makeMove(move);
                runningTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            }

            if (settings.displayBoards) board.display();

            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            if (settings.displayBoards) Console.WriteLine($"total elapsed time: {time}ms");

            return GameResult.GetResult(board, white, black);
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

            if (settings.limitedTurns && board.fullMoves - turnsAtStart > settings.maxTurns && whiteStarted == board.whiteToMove)
            {
                return true;
            }

            if (settings.limitedTime && runningTime > settings.maxTime)
            {
                return true;
            }

            return false;
        }

        public static void PlayFromUserInput()
        {
            Player white = PlayerList.selectPlayer(true);
            Player black = PlayerList.selectPlayer(false);

            ChessPlayerSettings settings = ChessPlayerSettings.AskUserForSettings();
            ChessPlayer player = new ChessPlayer(white, black, settings);

            Console.Write("Enter the starting fen (or leave empty for the standard position):");
            string? fen = Console.ReadLine();

            if (string.IsNullOrEmpty(fen)) player.Play();
            else player.Play(fen);

        }
    }
}