using System.Security.Cryptography.X509Certificates;
using chess;
using counters;

namespace chessPlayer
{
    /// <summary>
    /// Plats a game of chess between two engines, configuration depends on the settings object
    /// </summary>
    public class ChessPlayer
    {
        private Player? white;
        private Player? black;

        /// <summary>
        /// The current board of the game that is being played
        /// </summary>
        public Board? board { get; private set; }

        private ChessPlayerSettings? settings;
        private long runningTime;

        private int turnsAtStart;
        private bool whiteStarted;

        /// <summary>
        /// Gets invoked whenever the state of the game changes
        /// </summary>
        public EventHandler<ChessEventArgs>? onChange;

        private bool isRunning;

        /// <summary>
        /// Creates a new chess player
        /// </summary>
        public ChessPlayer() { }

        /// <summary>
        /// Creates a new chess player, with default settings
        /// </summary>
        /// <param name="white">The engine that will play for white</param>
        /// <param name="black">The engine that will play for black</param>
        public ChessPlayer(Player white, Player black) : this(white, black, ChessPlayerSettings.DEFAULT_SETTINGS) { }

        /// <summary>
        /// Creates a new chess player, with custom settings
        /// </summary>
        /// <param name="white">The engine that will play for white</param>
        /// <param name="black">The engine that will play for black</param>
        /// <param name="settings">The settings to play the game with</param>
        public ChessPlayer(Player white, Player black, ChessPlayerSettings settings)
        {
            this.white = white;
            this.black = black;
            this.settings = settings;

            this.white.engine.isWhite = true;
            this.black.engine.isWhite = false;

            isRunning = false;

        }

        /// <summary>
        /// Plays a game of chess from the regular starting position
        /// </summary>
        /// <returns>The result of the game</returns>
        public GameResult Play()
        {
            return Play(Board.START_FEN);
        }

        /// <summary>
        /// Plays a game of chess from a custom starting position
        /// </summary>
        /// <param name="fen">The fen string of the starting position</param>
        /// <returns>The result of the game</returns>
        public GameResult Play(string fen)
        {
            if (white == null || black == null || settings == null)
            {
                Console.WriteLine("Not all players or settings have been set, game will not be played");
                return new GameResult(0, 0, 0);
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

        /// <summary>
        /// Asks the user to select a black and white engine, then plays a game of chess with these engines
        /// </summary>
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

        /// <summary>
        /// Stops the game that is playing 
        /// </summary>
        public void Stop()
        {
            isRunning = false;
        }
    }
    
    /// <summary>
    /// Event arguments that gets passed along when an event is evoked
    /// </summary>
    public class ChessEventArgs : EventArgs
    {   
        /// <summary>
        /// The new board of the game
        /// </summary>
        public Board board { get; private set; }

        /// <summary>
        /// Creates a new chess event arguments object
        /// </summary>
        /// <param name="board">The new board of the game</param>
        public ChessEventArgs(Board board)
        {
            this.board = board;
        }
    }
}