using System.Text.RegularExpressions;
using chess;
using chessPlayer;
using chessTesting;
using converter;
using parser;

/// <summary>
/// The program reads user input from the console and runs the corresponding functionallity
/// </summary>
public class Program
{
    private ChessPlayer player;

    /// <summary>
    /// Creates a new program
    /// </summary>
    public Program()
    {
        player = new ChessPlayer();
        selectSetup();
    }

    /// <summary>
    /// Entrypoint of the application
    /// </summary>
    /// <param name="args">Arguments to run the application with, can be used to skip parts of the questionnaire</param>
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
        Board board = Board.startPosition();
        Move move = new Move(NotationConverter.toIndex("h2"), NotationConverter.toIndex("h4"));
        board = board.makeMove(move);

        board.display();
        Evaluator evaluator = new iterative_deepening.Evaluator();
        Console.WriteLine(evaluator.evaluate(board));
    }

    private static HashSet<Board> GetBoards(Board board, int depth)
    {
        HashSet<Board> boards = new HashSet<Board>();
        if (depth == 0)
        {
            boards.Add(board);
            return boards;
        }



        List<Move> moves = MoveGenerator.generateAllMoves(board);
        foreach (Move move in moves)
        {
            Board result = board.makeMove(move);

            foreach (Board b in GetBoards(result, depth - 1))
            {
                boards.Add(b);
            }
        }

        return boards;
    }

    private static void test2()
    {
        Board board1 = Board.fromFen("rnbqkbnr/p1p1pppp/1p6/3p4/6P1/2N5/PPPPPP1P/R1BQKBNR w KQkq d6 0 3");
        Board board2 = Board.fromFen("rnbqkbnr/pppp1ppp/8/4p3/7P/P7/1PPPPPP1/RNBQKBNR b KQkq - 0 3");

        board1.display();
        board2.display();
    }
}