using System.Text.RegularExpressions;
using chessPlayer;
using engine_comparer;

public class Program
{
    public static void Main(string[] args)
    {
        selectSetup();
    }

    private static void selectSetup()
    {
        Console.Write("Type 0 to run the chess player, or type 1 to run the engine comparer:");
        string? input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            selectSetup();
            return;
        }

        if (!Regex.IsMatch(input, "^[01]$"))
        {
            selectSetup();
            return;
        }

        int type = int.Parse(input);

        switch(type)
        {
            case 0: ChessPlayer.PlayFromUserInput(); break;
            case 1: EngineComparer.CompareFromUserInput(); break;
            default: break;
        }
    }
}