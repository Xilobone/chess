using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace chessPlayer
{
    public class ChessPlayerSettings
    {
        public static ChessPlayerSettings DEFAULT_SETTINGS = new ChessPlayerSettings(false, 0, false, 0, false, 0, true, true);

        public bool limitedTurns;
        public int maxTurns;

        public bool limitedTime;
        public float maxTime;

        public bool limitedTurnTime;
        public float maxTurnTime;

        public bool displayBoards;

        public bool requireInputAfterEachTurn;
        public ChessPlayerSettings()
        {
            this.limitedTurns = false;
            this.maxTurns = 0;
            this.limitedTime = false;
            this.maxTime = 0;
            this.limitedTurnTime = false;
            this.maxTurnTime = 0;
            this.displayBoards = true;
            requireInputAfterEachTurn = false;
        }

        /// <summary>
        /// Creates a new chess player settings objects with the specified values,
        /// if a value is non-positive it is assumed the relevant setting is false
        /// </summary>
        /// <param name="maxTurns">The maximum number of full turns</param>
        /// <param name="maxTime">The maximum time of the game (in ms)</param>
        /// <param name="maxTurnTime">The maximum time of a turn (in ms)</param>
        public ChessPlayerSettings(int maxTurns, float maxTime, float maxTurnTime)
        {
            this.limitedTurns = maxTurns > 0;
            this.maxTurns = maxTurns;

            this.limitedTime = maxTime > 0;
            this.maxTime = maxTime;

            this.limitedTurnTime = maxTurnTime > 0;
            this.maxTurnTime = maxTurnTime;

            this.displayBoards = true;
            requireInputAfterEachTurn = false;
        }

        /// <summary>
        /// Creates a new chess player settings objects with the specified values,
        /// if a value is non-positive it is assumed the relevant setting is false
        /// </summary>
        /// <param name="maxTurns">The maximum number of full turns</param>
        /// <param name="maxTime">The maximum time of the game (in ms)</param>
        /// <param name="maxTurnTime">The maximum time of a turn (in ms)</param>
        /// <param name="displayBoards">Whether to display the board after each move or not</param>
        /// <param name="requireInputAfterEachTurn">True if an input is required to continue to the next move</param>
        public ChessPlayerSettings(int maxTurns, float maxTime, float maxTurnTime, bool displayBoards, bool requireInputAfterEachTurn)
        {
            this.limitedTurns = maxTurns > 0;
            this.maxTurns = maxTurns;

            this.limitedTime = maxTime > 0;
            this.maxTime = maxTime;

            this.limitedTurnTime = maxTurnTime > 0;
            this.maxTurnTime = maxTurnTime;

            this.displayBoards = displayBoards;
        }

        /// <summary>
        /// Creates a new chess player settings objects with the specified values,
        /// </summary>
        /// <param name="limitedTurns">true if the game has a limited number of turns</param>
        /// <param name="maxTurns">The maximum number of turns</param>
        /// <param name="limitedTime">True if the game has a limited total allowed time per player</param>
        /// <param name="maxTime">The total allowed time per player</param>
        /// <param name="limitedTurnTime">True if the game has a limited time per turn</param>
        /// <param name="maxTurnTime">The max time per turn</param>
        /// <param name="displayBoards">True if the board should be displayed after each turn</param>
        /// <param name="requireInputAfterEachTurn">True if an input is required to continue to the next move</param>
        public ChessPlayerSettings(bool limitedTurns, int maxTurns, bool limitedTime, float maxTime, bool limitedTurnTime, float maxTurnTime, bool displayBoards, bool requireInputAfterEachTurn)
        {
            this.limitedTurns = limitedTurns;
            this.maxTurns = maxTurns;
            this.limitedTime = limitedTime;
            this.maxTime = maxTime;
            this.limitedTurnTime = limitedTurnTime;
            this.maxTurnTime = maxTurnTime;
            this.displayBoards = displayBoards;
            this.requireInputAfterEachTurn = requireInputAfterEachTurn;
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

            while(!Regex.IsMatch(input, "^[yn]$"))
            {
                Console.Write("Do you want to change these settings? [y/n]:");

                input = Console.ReadLine();
                if (input == null) input = "";
            }

            if (input.Equals("n")) return DEFAULT_SETTINGS;

            int maxTurns = AskUserForSetting("max turns");
            int maxTime = AskUserForSetting("max time (in ms)");
            int maxTurnTime = AskUserForSetting("max time per turn (in ms)");
            bool displayBoards = AskUserForSettingBool("displaying the board after each turn");
            bool requireInputAfterEachTurn = AskUserForSettingBool("requiring input after each turn");

            ChessPlayerSettings settings = new ChessPlayerSettings(maxTurns, maxTime, maxTurnTime, displayBoards, requireInputAfterEachTurn);

            settings.DisplaySettings();

            return settings;
        }

        private static int AskUserForSetting(string setting)
        {
            string? input = "";

            while(!Regex.IsMatch(input, "^-?[0-9]+$"))
            {
                Console.Write($"Enter the {setting}, a negative value will disable this setting:");

                input = Console.ReadLine();
                if (input == null) input = "";
            }

            return int.Parse(input);
        }

        private static bool AskUserForSettingBool(string setting)
        {
            string? input = "";

            while(!Regex.IsMatch(input, "^[yn]$"))
            {
                Console.Write($"Do you want to enable {setting}? [y/n]:");

                input = Console.ReadLine();
                if (input == null) input = "";

            }

            return input.Equals("y");
        }
    }
}