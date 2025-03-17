using System.Runtime.CompilerServices;
using chess.engine;
using chessPlayer;
using converter;

namespace chess
{
    /// <summary>
    /// Writes logs to the ./logs/games.log file
    /// </summary>
    public static class Logger
    {   
        /// <summary>
        /// The path of the file to log the game to
        /// </summary>
        private static string filePath = "./lib/games.log";
        
        /// <summary>
        /// Logs the game that has been played to the ./logs/games/log
        /// </summary>
        /// <param name="white">The white player</param>
        /// <param name="black">The black player</param>
        /// <param name="startFen">The starting position of the game</param>
        /// <param name="result">The result of the game</param>
        /// <param name="playedMoves">The series of moves played in the game</param>
        /// <param name="settings">The chess player settings the game was played with</param>
        public static void LogGame(IPlayer white, IPlayer black, string startFen, GameResult result, List<Move> playedMoves, ChessPlayerSettings settings)
        {
            string timestamp = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]";

            StreamWriter writer = File.AppendText(filePath);

            //log the players
            string whiteName = white.engine.GetType().Namespace!;
            string blackName = black.engine.GetType().Namespace!;
            writer.WriteLine($"{timestamp} players: {whiteName}, {blackName}");

            //log the settings
            writer.WriteLine($"{timestamp} settings: {settings}");

            //log the player configs
            writer.WriteLine($"{timestamp} white config: ({white.engine.config})");
            writer.WriteLine($"{timestamp} white config: ({black.engine.config})");

            //log the start position
            writer.WriteLine($"{timestamp} starting position: {startFen}");

            //log each move
            int turn = 1;
            Board board = Board.fromFen(startFen);
            string moves = "";

            for (int i = 0; i < playedMoves.Count; i += 2)
            {
                moves += $"{turn}. {NotationConverter.toAlgebraic(playedMoves[i], board)}";
                board = board.makeMove(playedMoves[i]);

                if ((i + 1) < playedMoves.Count)
                {
                    moves += $" {NotationConverter.toAlgebraic(playedMoves[i + 1], board)} ";
                    board = board.makeMove(playedMoves[i + 1]);
                }

                turn++;
                 
            }

            writer.WriteLine($"{timestamp} moves: {moves}");

            //log the game result
            writer.WriteLine($"{timestamp} result: {result}");
            writer.Close();
        }
    }
}