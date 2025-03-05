namespace chess
{   
    /// <summary>
    /// Class containing lists of all players that are configured
    /// </summary>
    public static class PlayerList
    {
        /// <summary>
        /// List of all white players that are configured
        /// </summary>
        public static IPlayer[] whitePlayers = {
            new Player<player.Player>(),
            new Player<minimax_engine.Engine>(),
            new Player<improved_minimax_engine.Engine>(),
            new Player<improved_minimax_eval_engine.Engine>(),
            new Player<transposition_table.Engine>(),
            new Player<iterative_deepening.Engine>(),

        };

        /// <summary>
        /// List of all black players that are configured
        /// </summary>
        public static IPlayer[] blackPlayers = {
            new Player<player.Player>(),
            new Player<minimax_engine.Engine>(),
            new Player<improved_minimax_engine.Engine>(),
            new Player<improved_minimax_eval_engine.Engine>(),
            new Player<transposition_table.Engine>(),
            new Player<iterative_deepening.Engine>(),
        };

        /// <summary>
        /// Asks the user to select a player from the corresponding list
        /// </summary>
        /// <param name="isWhite">The color of the player to select</param>
        /// <returns>The selected player</returns>
        public static IPlayer selectPlayer(bool isWhite)
        {
            IPlayer[] players = isWhite ? whitePlayers : blackPlayers;
            string color = isWhite ? "white" : "black";
            Console.WriteLine("Please select a " + color + " player:");

            for (int i = 0; i < players.Length; i++)
            {
                Console.WriteLine(i + ": " + players[i].name);
            }

            int index = int.Parse(Console.ReadLine()!);

            IPlayer player = players[index];

            player.engine.isWhite = isWhite;
            return player;
        }
    }
}
