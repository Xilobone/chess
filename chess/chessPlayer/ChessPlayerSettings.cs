using System.Text.Json;
using System.Text.RegularExpressions;

namespace chessPlayer
{   
    /// <summary>
    /// Holds the settings for the chess player
    /// </summary>
    public class ChessPlayerSettings
    {   
        /// <summary>
        /// The loaded in settings
        /// </summary>
        public static ChessPlayerSettings DEFAULT_SETTINGS { get => GetDefaultSettings(); private set => DEFAULT_SETTINGS = value; }
        private static ChessPlayerSettings? _DEFAULT_SETTINGS;

        /// <summary>
        /// Wheter the game has a limited number of turns or not
        /// </summary>
        public bool limitedTurns { get; set; }

        /// <summary>
        /// The maximum number of turns in the game
        /// </summary>
        public int maxTurns { get; set; }

        /// <summary>
        /// Whether the game has a limited running time or not
        /// </summary>
        public bool limitedTime { get; set; }

        /// <summary>
        /// The maximum time the game is allowed to go on for
        /// </summary>
        public int maxTime { get; set; }

        /// <summary>
        /// Whether the game has a limited allowed time per turn
        /// </summary>
        public bool limitedTurnTime { get; set; }

        /// <summary>
        /// The maximum allowed time per turn
        /// </summary>
        public int maxTurnTime { get; set; }

        /// <summary>
        /// Whether the board is written to the console after each turn or not
        /// </summary>

        public bool displayBoards { get; set; }

        /// <summary>
        /// Whether an input is required before continuing to the next move
        /// </summary>

        public bool requireInputAfterEachTurn { get; set; }

        /// <summary>
        /// The path where all the config files for the engines can be found
        /// </summary>
        public string configPath { get; set; }

        /// <summary>
        /// Creates a new chess player settings object with the default values,
        /// values can be changed afterwards
        /// </summary>
        public ChessPlayerSettings()
        {
            limitedTurns = maxTurns > 0;
            limitedTime = maxTime > 0;
            limitedTurnTime = maxTurnTime > 0;
            configPath = "";
        }

        private static ChessPlayerSettings GetDefaultSettings()
        {
            if (_DEFAULT_SETTINGS != null) return _DEFAULT_SETTINGS;

            string basePath = $"{AppDomain.CurrentDomain.BaseDirectory}/../../..";
            string[] str = File.ReadAllLines($"{basePath}/lib/settings.json");

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

            settings.configPath = DEFAULT_SETTINGS.configPath;
            settings.DisplaySettings();

            //ask if the user wants to make the altered settings the new default
            input = "";

            while (!Regex.IsMatch(input, "^[yn]$"))
            {
                Console.Write("Do you want to change the default settings to this? [y/n]:");

                input = Console.ReadLine();
                if (input == null) input = "";
            }

            if (input.Equals("y")) writeAsDefault(settings);

            return settings;
        }

        /// <summary>
        /// Writes the provided settings to be the default settings
        /// </summary>
        /// <param name="settings">The settings to make the default</param>
        public static void writeAsDefault(ChessPlayerSettings settings)
        {
            string json = JsonSerializer.Serialize(settings);
            File.WriteAllLines("lib/settings.json", [json]);
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

        /// <summary>
        /// Creates a string representation of the chess player settings
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}