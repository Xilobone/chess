using System.Text.RegularExpressions;
using chessPlayer;
using chessTesting;

public class Program
{
    public static void Main(string[] args)
    {
        selectSetup();
    }

    private static void selectSetup()
    {
        Console.WriteLine("Select one of the following options:");
        Console.WriteLine("0:   Run chess player");
        Console.WriteLine("1:   Run engine comparer");
        Console.WriteLine("2:   Run engine for a single position");
        Console.Write("Type 0, 1 or 2:");

        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            selectSetup();
            return;
        }

        if (!Regex.IsMatch(input, "^[0-2]$"))
        {
            selectSetup();
            return;
        }

        int type = int.Parse(input);

        switch(type)
        {
            case 0: ChessPlayer.PlayFromUserInput(); break;
            case 1: EngineComparer.CompareFromUserInput(); break;
            case 2: EngineTester.testSinglePosition(); break;
            default: break;
        }
    }
}