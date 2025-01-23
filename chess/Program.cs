using System.Text.RegularExpressions;
using chess;
using chessPlayer;
using chessTesting;
using converter;
using counters;
using gui;

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
        UnitTest.Run();
        test();
        // new Program();
    }

    private void selectSetup()
    {
        Console.WriteLine("Select one of the following options:");
        Console.WriteLine("0:   Run chess player");
        Console.WriteLine("1:   Run engine comparer");
        Console.WriteLine("2:   Run engine for a single position");
        Console.WriteLine("3:   Launch the gui");
        Console.Write("Type 0, 1, 2 or 3:");

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
            case 3: ChessGUI.Create(player); selectSetup(); break;
            default: break;
        }
    }

    private static void test()
    {
        Board board = Board.fromFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
        board.display();

        // ChessGUI.Create().OnChange(null, new ChessEventArgs(board));

        Counter<long> counter = new Counter<long>("Move generation", "ms");

        for (int i = 0; i < 10; i++)
        {
            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            MoveGenerator.perft(board, 4, false);

            counter.Set(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime);
            counter.Reset();

            Console.WriteLine($"done {i + 1}/10");
        }

        counter.DisplayOverview(true);
    }

    private static void test2()
    {
        Board board = Board.fromFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
        Move move = new Move(NotationConverter.toIndex("a1"), NotationConverter.toIndex("b1"));
        board = board.makeMove(move);

        move = new Move(NotationConverter.toIndex("e8"), NotationConverter.toIndex("g8"), 1);
        board = board.makeMove(move);

        // move = new Move(NotationConverter.toIndex("h6"), NotationConverter.toIndex("g8"));
        // board = board.makeMove(move);

        board.display();
        ChessGUI.Create().OnChange(null, new ChessEventArgs(board));

        MoveGenerator.perft(board, 2, true);

    }

    private static void test3()
    {
        int[] KNIGHT_OFFSETS2 = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };

        foreach (int offset in KNIGHT_OFFSETS2)
        {
            Position pos = Position.toPosition(offset);
            Console.WriteLine($"{offset}: {pos.x}, {pos.y}");
        }
    }
}