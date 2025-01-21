using System.Text.RegularExpressions;
using chess;
using chessPlayer;
using chessTesting;
using converter;
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
        Move move = new Move(NotationConverter.toIndex("e5"), NotationConverter.toIndex("c6"));
        board = board.makeMove(move);
        board.display();

        ChessGUI gui = ChessGUI.Create();
        gui.OnChange(null, new ChessEventArgs(board));

        MoveGenerator.perft(board, 1, true);
    }
}