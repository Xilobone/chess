using counters;

namespace chess.engine
{
    /// <summary>
    /// Base class for all engines
    /// </summary>
    public abstract class Engine
    {
        /// <summary>
        /// Determines whether the engine is optimizing for white or for black
        /// </summary>
        public bool isWhite { get; set; }

        /// <summary>
        /// A list of all counters that the engine uses for debugging and statistics
        /// </summary>
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

        /// <summary>
        /// The evaluator to use for evaluating positions
        /// </summary>
        public Evaluator evaluator { get; protected set; }

        /// <summary>
        /// The configuration the engine is using
        /// </summary>
        public EngineConfig config { get; private set; }

        /// <summary>
        /// Creates a new engine object
        /// </summary>
        /// <param name="isWhite">Whether the engine is optimizing for white or not</param>
        /// <param name="evaluator">The evaluator to use for evaluating positions</param>
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

        /// <summary>
        /// Computes the best move to make on the given board
        /// </summary>
        /// <param name="board">The board to make a move on</param>
        /// <returns>The best move to make on the board</returns>
        public abstract Move makeMove(Board board);

        /// <summary>
        /// Computes the best move to make on the given board within the time limit
        /// </summary>
        /// <param name="board">The board to make a move on</param>
        /// <param name="maxTime">The maximum allowed time to compute for</param>
        /// <returns>The best move to make on the board</returns>
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