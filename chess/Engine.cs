using System.Text.Json;
using System.Text.Json.Nodes;
using chessPlayer;
using counters;


namespace chess.engine
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

            //read config file
            string configPath = $"{ChessPlayerSettings.DEFAULT_SETTINGS.configPath}\\engines\\";
            string engineConfig = $"{configPath}{GetType().Namespace}.json";

            string json = File.Exists(engineConfig) ? File.ReadAllText(engineConfig) : File.ReadAllText($"{configPath}default.json");

            config = JsonSerializer.Deserialize<EngineConfig>(json)!;
        }

        public abstract Move makeMove(Board board);
        public abstract Move makeMove(Board board, float maxTime);

        /// <summary>
        /// Clears the value of all counters of the engine
        /// </summary>
        protected void clearCounters()
        {
            foreach (ICounter counter in counters)
            {
                counter.Reset();
            }
        }

        /// <summary>
        /// Gets the current time (in ms)
        /// </summary>
        /// <returns>A long indicating the current time from the Epoch</returns>
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