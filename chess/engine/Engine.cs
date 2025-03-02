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

        /// <summary>
        /// Counter that stores the amount of boards that are evaluated for each move
        /// </summary>
        protected Counter<int> evaluatedBoards { get; private set; }

        /// <summary>
        /// Counter that stores the computation time (in ms) for each move
        /// </summary>
        protected Counter<long> computationTime { get; private set; }

        /// <summary>
        /// Counter that stores the time (in ms) spent evaluating for each move
        /// </summary>
        protected Counter<long> evaluationTime { get; private set; }

        /// <summary>
        /// Counter that stores the amount of boards that are evaluated for each move
        /// </summary>
        protected Counter<long> generationTime { get; private set; }

        protected Evaluator evaluator;

        public EngineConfig config { get; private set; }

        public Engine(bool isWhite, Evaluator evaluator)
        {   
            //add all default counters
            counters = new List<ICounter>();
            evaluatedBoards = new Counter<int>("Evaluated boards");
            computationTime = new Counter<long>("Computation time", "ms");
            evaluationTime = new Counter<long>("Evaluation time", "ms");
            generationTime = new Counter<long>("Generation time", "ms");
            counters.AddRange(evaluatedBoards, computationTime, evaluationTime, generationTime);

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
    }
}