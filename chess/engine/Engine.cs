using counters;

namespace chess.engine
{   
    /// <summary>
    /// Base class for all engines
    /// </summary>
    public abstract class Engine
    {
        public bool isWhite { get; set; }
        public List<ICounter> counters { get; set; }

        protected Evaluator evaluator;

        public EngineConfig config { get; private set; }

        public Engine(bool isWhite, Evaluator evaluator)
        {
            counters = new List<ICounter>();

            this.isWhite = isWhite;
            this.evaluator = evaluator;

            config = EngineConfig.GetConfig(GetType().Namespace!);
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
    }
}