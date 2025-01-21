using System.Diagnostics.Metrics;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms.VisualStyles;
using chess;
using chessPlayer;
using chessTesting;
using converter;
using counters;
using gui;
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
        string fen = "1n2k2r/2pb1p1p/p6n/3Pp3/4P2b/5P2/PPP4P/RN1K3q w k - 0 16";
        Board board = Board.startPosition();
        board.display();

        Engine engine = new improved_minimax_eval_engine.Engine(true);
        Move move = engine.makeMove(board);
        Console.WriteLine($"move:{move}");
        foreach(ICounter counter in engine.counters)
        {
            counter.DisplayOverview();
        }
        // List<Move> moves = MoveGenerator.generateAllMoves(board);
        // foreach(Move move in moves)
        // {
        //     Console.WriteLine(move);
        // }
    }
}