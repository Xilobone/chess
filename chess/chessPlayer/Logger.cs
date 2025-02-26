using chessPlayer;

namespace chess
{
    /// <summary>
    /// Writes logs to the ./logs/games.log file
    /// </summary>
    public static class Logger
    {
        private static string filePath = "./lib/games.log";
        
        /// <summary>
        /// Logs the game that has been played to the ./logs/games/log
        /// </summary>
        /// <param name="white">The white player</param>
        /// <param name="black">The black player</param>
        /// <param name="startFen">The starting position of the game</param>
        /// <param name="result">The result of the game</param>
        /// <param name="playedMoves">The series of moves played in the game</param>
        public static void LogGame(Player white, Player black, string startFen, GameResult result, List<Move> playedMoves)
        {
            string timestamp = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]";

            StreamWriter writer = File.AppendText(filePath);
            writer.Write($"{timestamp} players: {white.GetType().Namespace}, {black.GetType().Namespace}");
            writer.Write($"{timestamp} starting position: {startFen}");
            writer.Close();
        }
    }
}