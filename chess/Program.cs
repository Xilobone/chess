using System.Text.RegularExpressions;
using chess;
using chess.engine;
using chessPlayer;
using chessTesting;
using converter;
using parser;

/// <summary>
/// The program reads user input from the console and runs the corresponding functionallity
/// </summary>
public class Program
{
    /// <summary>
    /// Creates a new program
    /// </summary>
    public Program()
    {
        test();
        // UnitTest.Run();
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
            case 0: ChessPlayer.PlayFromUserInput(); break;
            case 1: EngineComparer.CompareFromUserInput(); break;
            case 2: EngineTester.testSinglePosition(); break;
            default: break;
        }
    }

    private static void test()
    {
        ChessPlayerSettings settings = new ChessPlayerSettings();
        settings.displayBoards = true;
        settings.limitedTurns = false;
        settings.limitedTurnTime = true;
        settings.maxTurnTime = 1000;
        settings.limitedTime = false;
        settings.requireInputAfterEachTurn = false;
        ChessPlayer player = new ChessPlayer(PlayerList.whitePlayers[1], PlayerList.blackPlayers[4], settings);
        player.Play();

        List<Move> moves = player.playedMoves;

        Board board = Board.startPosition();
        foreach (Move move in moves)
        {
            Console.WriteLine($"{move} -> {NotationConverter.toAlgebraic(move, board)}");
            board = board.makeMove(move);
        }
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
}