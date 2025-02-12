using System.Text.RegularExpressions;
using chess;
using chessPlayer;
using chessTesting;
using parser;

public class Program
{
    private ChessPlayer player;
    public Program()
    {
        player = new ChessPlayer();
        selectSetup();
    }
    [STAThread]
    public static void Main(string[] args)
    {
        // UnitTest.Run();
        // test();
        new Program();
    }

    private void selectSetup()
    {
        Console.WriteLine("Select one of the following options:");
        Console.WriteLine("0:   Run chess player");
        Console.WriteLine("1:   Run engine comparer");
        Console.WriteLine("2:   Run engine for a single position");
        Console.Write("Type 0, 1, or 2:");

        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            selectSetup();
            return;
        }

        if (!Regex.IsMatch(input, "^[0-3]$"))
        {
            selectSetup();
            return;
        }

        int type = int.Parse(input);

        switch (type)
        {
            case 0: player.PlayFromUserInput(); break;
            case 1: EngineComparer.CompareFromUserInput(); break;
            case 2: EngineTester.testSinglePosition(); break;
            default: break;
        }
    }

    private static void test()
    {
        GamesParser parser = new GamesParser("./lib/chess_games.pgn", [new improved_minimax_eval_engine.Evaluator()], 0);
        List<Board> boards = parser.parse(10000, 1000, 10);

        int tableSize = 300_243;
        bool[] table = new bool[tableSize];

        int nCollisions = 0;
        foreach(Board board in boards)
        {
            int hash = Zobrist.hash(board);

            hash = hash % tableSize;

            if(table[hash])
            {
                nCollisions++;
            }

            table[hash] = true;
        }

        double sizeMb = (double) (sizeof(int) * tableSize) / (1024 * 1024);

        Console.WriteLine($"Number of collisions: {nCollisions}/1000, size:{sizeMb:F2}mb");
    }
}