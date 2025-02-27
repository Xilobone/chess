using counters;

namespace chess.engine
{
    public abstract class TTEngine : Engine
    {
        private SearchResult[] transpositionTable;

        public Counter<int> assignedTranspositionTableIndexes { get; private set; }
        public Counter<int> hashCollisions { get; private set; }

        protected TTEngine(bool isWhite, IEvaluator evaluator) : base(isWhite, evaluator)
        {
            //create counters
            assignedTranspositionTableIndexes = new Counter<int>("items added to transpositiontable");
            hashCollisions = new Counter<int>("hash collisions");
            counters.AddRange(assignedTranspositionTableIndexes, hashCollisions);

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
            ulong hash = Zobrist.hash(board);
            ulong index = hash % config.transpositionTableSize;
            result.hash = hash;
            //there is already a result stored at the given index
            if (transpositionTable[index] != null)
            {
                //hash collision
                if (transpositionTable[index].hash != result.hash) hashCollisions.Increment();

                //the stored result has a greater depth than the new result, do not overrwide result
                if (transpositionTable[index].searchedDepth < result.searchedDepth) return;
            }

            assignedTranspositionTableIndexes.Increment();
            transpositionTable[index] = result;
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