using System.DirectoryServices;
using System.Text.RegularExpressions;
using chess;
using counters;

namespace chessTesting 
{
    public class EngineTester
    {   
        public static void testSinglePosition()
        {
            Player player = PlayerList.selectPlayer(true);
            int repetitions = getRepetitions();

            Console.Write("Enter the starting fen (or leave empty for the standard position):");
            string? fen = Console.ReadLine();

            Board board;
            if (string.IsNullOrEmpty(fen)) board = Board.startPosition();
            else board = Board.fromFen(fen);

            player.engine.isWhite = board.whiteToMove;
            player.engine.displayStats = true;

            board.display();

            Move? move = null;
            for(int i = 0; i < repetitions; i++)
            {
                move = player.engine.makeMove(board);
            }

            Console.WriteLine($"Selected move: {move}");

            //show counter with differences from baseline
            foreach(ICounter counter in player.engine.counters)
            {
                // counter.write();
                counter.DisplayOverview(true);
            }

            //show transposition table info
            if (player.engine is transposition_table.Engine)
            {
                transposition_table.Engine engine = (transposition_table.Engine)player.engine;

                int transpositionTableSize = 0;
                foreach(Engine.SearchResult item in engine.transpositionTable)
                {
                    if (item != null) transpositionTableSize++;
                }

                Console.WriteLine($"transposition table: {transpositionTableSize}/{engine.transpositionTable.Length} used indexes");
                Console.WriteLine($"{engine.overwrittenSearchResults} results are overwritten");
            }
        }

        private static int getRepetitions()
        {
            Console.Write("How often do you want to run the engine?:");

            string? input = Console.ReadLine();

            if (string.IsNullOrEmpty(input))
            {
                return getRepetitions();
            }

            if (!Regex.IsMatch(input, "^[0-9]+$"))
            {
                return getRepetitions();
            }

            return int.Parse(input);
        }
    }
}