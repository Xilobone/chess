using System.Text.Json;
using System.Text.Json.Nodes;
using chessPlayer;
using counters;


namespace chess
{
    public abstract class Engine
    {
        public bool isWhite { get; set; }
        public bool displayStats { get; set; }
        public List<ICounter> counters { get; set; }

        protected IEvaluator evaluator;

        protected EngineConfig config;

        public Engine(bool isWhite, IEvaluator evaluator)
        {
            counters = new List<ICounter>();

            this.isWhite = isWhite;
            this.evaluator = evaluator;

            //try to read config file
            string configPath = $"{ChessPlayerSettings.DEFAULT_SETTINGS.configPath}\\engines\\";

            JsonNode? defaultConfig = JsonNode.Parse(File.ReadAllText($"{configPath}engine.json"));
            JsonNode? engineConfig = null;
            try
            {
                engineConfig = JsonNode.Parse(File.ReadAllText($"{configPath}{GetType().Namespace}.json"));
            }
            catch (IOException) { }

            JsonNode mergedJson = MergeJson(defaultConfig, engineConfig);
            config = JsonSerializer.Deserialize<EngineConfig>(mergedJson)!;
        }

        static JsonNode MergeJson(JsonNode? main, JsonNode? implementation)
        {
            if (main is not JsonObject mainObj || implementation is not JsonObject implObj)
                return main ?? implementation ?? new JsonObject();

            JsonObject result = JsonSerializer.Deserialize<JsonObject>(mainObj.ToJsonString())!;

            foreach (var prop in implObj)
            {
                result[prop.Key] = prop.Value?.DeepClone();
            }

            return result;
        }

        public abstract Move makeMove(Board board);
        public abstract Move makeMove(Board board, float maxTime);

        protected void clearCounters()
        {
            foreach (ICounter counter in counters)
            {
                counter.Reset();
            }
        }
        protected long getCurrentTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        public class SearchResult
        {
            public float evaluation;

            public int searchedDepth;
            public ulong hash;
            public Move? move;

            public SearchResult(float evaluation, int searchedDepth, Move move)
            {
                this.evaluation = evaluation;
                this.searchedDepth = searchedDepth;
                this.move = move;
            }

            public SearchResult(float evaluation, int searchedDepth)
            {
                this.evaluation = evaluation;
                this.searchedDepth = searchedDepth;
            }

            public override string ToString()
            {
                return $"evaluation:{evaluation}, depth:{searchedDepth}, best move:{move}";
            }
        }

        protected class EngineConfig
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
        }
    }
}