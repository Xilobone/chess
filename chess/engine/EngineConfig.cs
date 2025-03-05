using System.Text.Json;
using chessPlayer;

namespace chess.engine
{   
    /// <summary>
    /// Class that stores the configuration of an engine
    /// </summary>
    public class EngineConfig
    {
        /// <summary>
        /// The maximum depth the engine is allowed to search at
        /// </summary>
        public int maxDepth { get; private set; }

        /// <summary>
        /// Whether the engine makes use of a transposition table
        /// </summary>
        public bool hasTranspositionTable { get; private set; }

        /// <summary>
        /// The size of the transposition table
        /// </summary>
        public ulong transpositionTableSize { get; private set; }

        /// <summary>
        /// Creates a new engine config
        /// </summary>
        /// <param name="maxDepth">The maximum search depth of the engine</param>
        /// <param name="hasTranspositionTable">Whether the engine makes use of a transposition table</param>
        /// <param name="transpositionTableSize">The size of the transposition table</param>
        public EngineConfig(int maxDepth, bool hasTranspositionTable, ulong transpositionTableSize)
        {
            this.maxDepth = maxDepth;
            this.hasTranspositionTable = hasTranspositionTable;
            this.transpositionTableSize = transpositionTableSize;
        }

        /// <summary>
        /// Gets the configuration of an engine from the configuration file
        /// </summary>
        /// <param name="engineName">The name of the engine to get the configuration of</param>
        /// <returns>The configuration of the engine</returns>
        public static EngineConfig GetConfig(string engineName)
        {
            string configPath = $"{ChessPlayerSettings.DEFAULT_SETTINGS.configPath}\\engines\\";
            string engineConfig = $"{configPath}{engineName}.json";

            string json = File.Exists(engineConfig) ? File.ReadAllText(engineConfig) : File.ReadAllText($"{configPath}default.json");

            return JsonSerializer.Deserialize<EngineConfig>(json)!;
        }

        /// <summary>
        /// Creates a string representation of the engine configuration
        /// </summary>
        /// <returns>The string representation of the configuration</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}