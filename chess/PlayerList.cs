namespace chess
{
    public static class PlayerList
    {
        public static Player[] whitePlayers = {
            new Player("player", new player.Player(), new player.Evaluator()),
            new Player("minimax", new minimax_engine.Engine(), new minimax_engine.Evaluator()),
            new Player("deep minimax", new deep_minimax_engine.Engine(), new deep_minimax_engine.Evaluator()),
            new Player("improved minimax", new improved_minimax_engine.Engine(), new improved_minimax_engine.Evaluator()),
            new Player("improved eval minimax", new improved_minimax_eval_engine.Engine(), new improved_minimax_eval_engine.Evaluator()),
            new Player("iterative deepening", new transposition_table.Engine(), new transposition_table.Evaluator())
        };

        public static Player[] blackPlayers = {
            new Player("player", new player.Player(), new player.Evaluator()),
            new Player("minimax", new minimax_engine.Engine(), new minimax_engine.Evaluator()),
            new Player("deep minimax", new deep_minimax_engine.Engine(), new deep_minimax_engine.Evaluator()),
            new Player("improved minimax", new improved_minimax_engine.Engine(), new improved_minimax_engine.Evaluator()),
            new Player("improved eval minimax", new improved_minimax_eval_engine.Engine(), new improved_minimax_eval_engine.Evaluator()),
            new Player("iterative deepening", new transposition_table.Engine(), new transposition_table.Evaluator())
        };

        public static Player selectPlayer(bool isWhite) 
        {
            Player[] players = isWhite ? whitePlayers : blackPlayers;
            string color = isWhite ? "white" : "black";
            Console.WriteLine("Please select a " + color + " player:");

            for (int i = 0; i < players.Length; i++)
            {
                Console.WriteLine(i + ": " + players[i].name);
            }

            int index = int.Parse(Console.ReadLine()!);

            Player player = players[index];

            player.engine.isWhite = isWhite;
            return player;
        }
    }
}
