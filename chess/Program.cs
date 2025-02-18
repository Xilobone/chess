using System.CodeDom.Compiler;
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
        UnitTest.Run();
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
        List<Board> boards = parser.parse(50000, 6000, 9999999999);

        // HashSet<Board> boards = GetBoards(Board.startPosition(), 3);
        ulong tableSize = 32_000_000;
        Board[] table = new Board[tableSize];
        long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        int nCollisions = 0;
        foreach (Board board in boards)
        {
            ulong hash = Zobrist.hash(board);

            ulong index = hash % tableSize;

            if (table[index] != null && Zobrist.hash(table[index]) == hash)
            {
                nCollisions++;
                Console.WriteLine("------------------\nCollision\n--------------------------");
                table[index].display();
                board.display();
            }

            table[index] = board;
        }

        double sizeMb = (double)(sizeof(int) * tableSize) / (1024 * 1024);

        Console.WriteLine($"Number of collisions: {nCollisions}/{boards.Count}, size:{sizeMb:F2}mb");
        Console.WriteLine($"Average hashing time:{(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime) / boards.Count}ms");
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