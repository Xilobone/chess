using System.Text.Json;
using System.Text.RegularExpressions;
using chess;
using chessPlayer;
using chessTesting;
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
        new Program();
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

        switch(type)
        {
            case 0: player.PlayFromUserInput(); break;
            case 1: EngineComparer.CompareFromUserInput(); break;
            case 2: EngineTester.testSinglePosition(); break;
            case 3: ChessGUI.Create(player); selectSetup(); break;
            default: break;
        }
    }
}