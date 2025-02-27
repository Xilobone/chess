using System.Security.Cryptography.X509Certificates;
using chess;
using counters;

namespace chessPlayer
{
    public class ChessPlayer
    {
        private Player? white;
        private Player? black;

        public Board? board { get; private set; }

        private ChessPlayerSettings? settings;
        private long runningTime;

        private int turnsAtStart;
        private bool whiteStarted;

        public EventHandler<ChessEventArgs>? onChange;

        private bool isRunning;

        public ChessPlayer()
        {

        }
        public ChessPlayer(Player white, Player black) : this(white, black, ChessPlayerSettings.DEFAULT_SETTINGS) { }

        public ChessPlayer(Player white, Player black, ChessPlayerSettings settings)
        {
            this.white = white;
            this.black = black;
            this.settings = settings;

            this.white.engine.isWhite = true;
            this.black.engine.isWhite = false;

            isRunning = false;

        }

        public GameResult Play()
        {
            return Play(Board.START_FEN);
        }

        public GameResult Play(string fen)
        {
            if (white == null || black == null || settings == null)
            {
                Console.WriteLine("Not all players or settings have been set, game will not be played");
                return new GameResult(0, 0,0);
            }

            isRunning = true;
            board = Board.fromFen(fen);
            onChange?.Invoke(this, new ChessEventArgs(board));

            turnsAtStart = board.fullMoves;
            whiteStarted = board.whiteToMove;
            runningTime = 0;
            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            List<Move> playedMoves = new List<Move>();
            //runs the game
            while (!stopConditionMet())
            {
                if (settings.displayBoards)
                {
                    board.display();
                    Console.WriteLine("White's evaluation: " + white.evaluator.evaluate(board));
                    Console.WriteLine("Black's evaluation: " + black.evaluator.evaluate(board));
                }

                if (settings.requireInputAfterEachTurn)
                {
                    Console.Write("Press enter to continue to next move:");
                    Console.ReadLine();
                }
                Move move;

                long moveStartTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
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
                playedMoves.Add(move);

                onChange?.Invoke(this, new ChessEventArgs(board));

                runningTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            }

            //game has finished
            board.display();
            Console.WriteLine("White's evaluation: " + white.evaluator.evaluate(board));
            Console.WriteLine("Black's evaluation: " + black.evaluator.evaluate(board));


            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            if (settings.displayBoards) Console.WriteLine($"total elapsed time: {time}ms");

            GameResult result = GameResult.GetResult(board, white, black);
            Logger.LogGame(white, black, fen, result, playedMoves, settings);
            return result;
        }

        private bool stopConditionMet()
        {
            if (board == null)
            {
                return false;
            }

            if (!isRunning)
            {
                return true;
            }

            if (board.isInMate() || board.isInDraw())
            {
                return true;
            }

            if (settings!.limitedTurns && board.fullMoves - turnsAtStart >= settings.maxTurns && whiteStarted == board.whiteToMove)
            {
                return true;
            }

            if (settings.limitedTime && runningTime > settings.maxTime)
            {
                return true;
            }

            return false;
        }

        public void PlayFromUserInput()
        {
            white = PlayerList.selectPlayer(true);
            black = PlayerList.selectPlayer(false);

            settings = ChessPlayerSettings.AskUserForSettings();

            Console.Write("Enter the starting fen (or leave empty for the standard position):");
            string? fen = Console.ReadLine();

            if (string.IsNullOrEmpty(fen)) Play();
            else Play(fen);

        }

        public void Stop()
        {
            isRunning = false;
        }
    }

    public class ChessEventArgs : EventArgs
    {
        public Board board { get; private set; }
        public ChessEventArgs(Board board)
        {
            this.board = board;
        }
    }
}