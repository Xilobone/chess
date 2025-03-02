using counters;

namespace chess.engine
{
    /// <summary>
    /// Base class for all engines with a transposition table
    /// </summary>
    public abstract class TTEngine : Engine
    {
        private SearchResult[] transpositionTable;

        public Counter<int> assignedTranspositionTableIndexes { get; private set; }
        public Counter<int> hashCollisions { get; private set; }

        /// <summary>
        /// Amount of time spend writing to the transposition table
        /// </summary>
        protected Counter<float> ttWriteTime { get; private set; }

        protected TTEngine(bool isWhite, Evaluator evaluator) : base(isWhite, evaluator)
        {
            //create counters
            assignedTranspositionTableIndexes = new Counter<int>("items added to transpositiontable");
            hashCollisions = new Counter<int>("hash collisions");
            ttWriteTime = new Counter<float>("Transposition table write time", "ms");
            counters.AddRange(assignedTranspositionTableIndexes, hashCollisions, ttWriteTime);

            //create transpositiontable
            transpositionTable = new SearchResult[config.transpositionTableSize];
        }

        /// <summary>
        /// Adds a given searchresult to the transposition table, if the index has not already been assigned,
        /// overwrites assigned indexes if the new result has the same or a deeper depth
        /// </summary>
        /// <param name="board">The board to create an index from</param>
        /// <param name="result">The search result to store in the transposition table</param>
        protected void addToTranspositionTable(Board board, SearchResult result)
        {
            long startTime = getCurrentTime();

            ulong hash = Zobrist.hash(board);
            ulong index = hash % config.transpositionTableSize;
            result.hash = hash;
            //there is already a result stored at the given index
            if (transpositionTable[index] != null)
            {
                //hash collision
                if (transpositionTable[index].hash != result.hash) hashCollisions.Increment();

                //the stored result has a greater depth than the new result, do not overrwide result
                if (transpositionTable[index].searchedDepth < result.searchedDepth)
                {
                    ttWriteTime.Increment(getCurrentTime() - startTime);
                    return;
                }
            }

            assignedTranspositionTableIndexes.Increment();
            transpositionTable[index] = result;
            ttWriteTime.Increment(getCurrentTime() - startTime);

        }

        /// <summary>
        /// Gets the stored result for the given board from the transposition table, if any has been stored,
        /// returns null otherwise
        /// </summary>
        /// <param name="board">The board to get the result from</param>
        /// <returns>The searchresult from the transposition table, if any</returns>
        protected SearchResult? getFromTranspositionTable(Board board)
        {
            ulong hash = Zobrist.hash(board);
            ulong index = hash % config.transpositionTableSize;

            if (transpositionTable[index] == null || transpositionTable[index].hash != hash) return null;

            return transpositionTable[index];
        }

        /// <summary>
        /// Clears all previously stored search results from the transposition table
        /// </summary>
        public void clearTranspositionTable()
        {
            transpositionTable = new SearchResult[config.transpositionTableSize];
        }
    }
}