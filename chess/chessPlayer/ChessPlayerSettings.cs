using System.Drawing.Text;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace chessPlayer
{
    public class ChessPlayerSettings
    {
        public static ChessPlayerSettings DEFAULT_SETTINGS { get => GetDefaultSettings(); private set => DEFAULT_SETTINGS = value; }
        private static ChessPlayerSettings? _DEFAULT_SETTINGS;

        public bool limitedTurns { get; set; }
        public int maxTurns { get; set; }

        public bool limitedTime { get; set; }
        public float maxTime { get; set; }
        public bool limitedTurnTime { get; set; }
        public float maxTurnTime { get; set; }

        public bool displayBoards { get; set; }

        public bool requireInputAfterEachTurn { get; set; }

        /// <summary>
        /// Creates a new chess player settings object with the default values,
        /// values can be changed afterwards
        /// </summary>
        public ChessPlayerSettings()
        {
            limitedTurns = maxTurns > 0;
            limitedTime = maxTime > 0;
            limitedTurnTime = maxTurnTime > 0;
        }

        private static ChessPlayerSettings GetDefaultSettings()
        {
            if (_DEFAULT_SETTINGS != null) return _DEFAULT_SETTINGS;

            string[] str = File.ReadAllLines("lib/settings.json");

            string json = "";

            foreach (string s in str)
            {
                json += s;
            }

            _DEFAULT_SETTINGS = JsonSerializer.Deserialize<ChessPlayerSettings>(json)!;

            return _DEFAULT_SETTINGS;
        }

        private void DisplaySettings()
        {
            Console.WriteLine("Current chess player settings:");
            Console.WriteLine("-----------------------");

            if (limitedTurns) Console.WriteLine($"Max number of turns: {maxTurns}");
            else Console.WriteLine("No max number of turns");

            if (limitedTime) Console.WriteLine($"Allowed total time per player: {maxTime}ms");
            else Console.WriteLine("No max allowed total time per player");

            if (limitedTurnTime) Console.WriteLine($"Allowed time per move: {maxTurnTime}");
            else Console.WriteLine("No max allowed time per turn");

            Console.WriteLine($"Board is {(displayBoards ? "" : "not ")}displayed after each turn");
            Console.WriteLine($"Input is {(requireInputAfterEachTurn ? "" : "not ")}reqired after each turn");

            Console.WriteLine("-----------------------");
        }

        /// <summary>
        /// Asks the user of the program to enter the settings values
        /// </summary>
        /// <returns>The generated settings</returns>
        public static ChessPlayerSettings AskUserForSettings()
        {
            DEFAULT_SETTINGS.DisplaySettings();

            string? input = "";

            while (!Regex.IsMatch(input, "^[yn]$"))
            {
                Console.Write("Do you want to change these settings? [y/n]:");

                input = Console.ReadLine();
                if (input == null) input = "";
            }

            if (input.Equals("n")) return DEFAULT_SETTINGS;

            ChessPlayerSettings settings = new ChessPlayerSettings();

            settings.maxTurns = AskUserForSetting("max turns");
            settings.maxTime = AskUserForSetting("max time (in ms)");
            settings.maxTurnTime = AskUserForSetting("max time per turn (in ms)");
            settings.displayBoards = AskUserForSettingBool("displaying the board after each turn");
            settings.requireInputAfterEachTurn = AskUserForSettingBool("requiring input after each turn");

            settings.limitedTurns = settings.maxTurns > 0;
            settings.limitedTime = settings.maxTime > 0;
            settings.limitedTurnTime = settings.maxTurnTime > 0;

            settings.DisplaySettings();

            //ask if the user wants to make the altered settings the new default
            input = "";

            while (!Regex.IsMatch(input, "^[yn]$"))
            {
                Console.Write("Do you want to change the default settings to this? [y/n]:");

                input = Console.ReadLine();
                if (input == null) input = "";
            }

            if (input.Equals("y"))
            {
                string json = JsonSerializer.Serialize(settings);

                File.WriteAllLines("lib/settings.json", [json]);
            }
            return settings;
        }

        private static int AskUserForSetting(string setting)
        {
            string? input = "";

            while (!Regex.IsMatch(input, "^-?[0-9]+$"))
            {
                Console.Write($"Enter the {setting}, a negative value will disable this setting:");

                input = Console.ReadLine();
                if (input == null) input = "";
            }

            int result = int.Parse(input);
            return result > 0 ? result : 0;
        }

        private static bool AskUserForSettingBool(string setting)
        {
            string? input = "";

            while (!Regex.IsMatch(input, "^[yn]$"))
            {
                Console.Write($"Do you want to enable {setting}? [y/n]:");

                input = Console.ReadLine();
                if (input == null) input = "";

            }

            return input.Equals("y");
        }
    }
}