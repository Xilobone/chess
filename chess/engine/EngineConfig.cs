using System.Text.Json;
using chessPlayer;

namespace chess.engine
{
    public class EngineConfig
    {
        public int maxDepth { get; private set; }
        public bool hasTranspositionTable { get; private set; }
        public ulong transpositionTableSize { get; private set; }

        public EngineConfig(int maxDepth, bool hasTranspositionTable, ulong transpositionTableSize)
        {
            this.maxDepth = maxDepth;
            this.hasTranspositionTable = hasTranspositionTable;
            this.transpositionTableSize = transpositionTableSize;
        }

        public static EngineConfig GetConfig(string engineName)
        {
            string configPath = $"{ChessPlayerSettings.DEFAULT_SETTINGS.configPath}\\engines\\";
            string engineConfig = $"{configPath}{engineName}.json";

            string json = File.Exists(engineConfig) ? File.ReadAllText(engineConfig) : File.ReadAllText($"{configPath}default.json");

            return JsonSerializer.Deserialize<EngineConfig>(json)!;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}