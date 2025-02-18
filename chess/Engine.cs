using System.Text.Json;
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
            string configPath = $"{ChessPlayerSettings.DEFAULT_SETTINGS.configPath}\\engines\\{GetType().Namespace}.json";
            try
            {
                string configJson = File.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<EngineConfig>(configJson)!;
            }
            catch (IOException)
            {
         
                config = new EngineConfig(1, 1);
            }

            // Deserialize JSON into a C# object
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
            public ulong transpositionTableSize { get; private set; }

            public EngineConfig(int maxDepth, ulong transpositionTableSize)
            {
                this.maxDepth = maxDepth;
                this.transpositionTableSize = transpositionTableSize;
            }
        }
    }
}