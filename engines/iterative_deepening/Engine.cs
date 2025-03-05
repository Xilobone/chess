using counters;
using chess;
using chess.engine;

namespace iterative_deepening
{
    /// <summary>
    /// Engine that makes use of a minimax algorithm and a transposition table in order to find the best
    /// moves on a given board
    /// </summary>
    public class Engine : TTEngine
    {
        private long moveEndTime;
        private Counter<float> sortTime;
        private Counter<float> moveMakingTime;

        /// <summary>
        /// Creates a new transposition table engine that optimizes moves for the white player
        /// </summary>
        public Engine() : this(true) { }

        /// <summary>
        /// Createst a new transposition table engine that optimizes for the selected player
        /// </summary>
        /// <param name="isWhite">true if optimizing for white, false for black</param>
        public Engine(bool isWhite) : base(isWhite, new Evaluator())
        {
            sortTime = new Counter<float>("sorting time", "ms");
            moveMakingTime = new Counter<float>("move making time", "ms");
            counters.AddRange(sortTime, moveMakingTime);
        }

        /// <summary>
        /// Computes the best move to make for the given board
        /// </summary>
        /// <param name="board">The board to compute the best possible move for</param>
        /// <returns>The best possible move on the board</returns>
        public override Move makeMove(Board board)
        {
            return makeMove(board, float.MaxValue);
        }

        /// <summary>
        /// Computes the best move to make for the current board, spends at most maxTime time computing
        /// </summary>
        /// <param name="board">The board to compute the best possible move for</param>
        /// <param name="maxTime">The maximum amount of allowed computation time (in ms)</param>
        /// <returns>The best found move</returns>
        public override Move makeMove(Board board, float maxTime)
        {
            long startTime = getCurrentTime();
            moveEndTime = maxTime == float.MaxValue ? long.MaxValue : getCurrentTime() + (long)maxTime;
            SearchResult? result = null;
            for (int i = 1; i <= config.maxDepth; i++)
            {
                SearchResult newResult = Minimax(board, i, float.MinValue, float.MaxValue, board.whiteToMove);

                //accept new result if search was completed, otherwise break and use previous result
                if (getCurrentTime() < moveEndTime) result = newResult;
                else break;
            }
            addToTranspositionTable(board, result!);

            computationTime.Set(getCurrentTime() - startTime);
            clearCounters();

            return result!.move!;
        }

        private SearchResult Minimax(Board board, int depth, float alpha, float beta, bool isMaximizingPlayer)
        {
            // check if this board has been stored in the transposition table
            SearchResult? storedResult = getFromTranspositionTable(board);
            if (storedResult != null && storedResult.searchedDepth >= depth)
            {
                return storedResult;
            }

            //return this board if reached end of search tree, or if out of time
            if (depth == 0 || board.isInMate() || getCurrentTime() >= moveEndTime)
            {
                evaluatedBoards.Increment();

                long startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime.Increment(getCurrentTime() - startTime);
                
                SearchResult result = new SearchResult(eval, 0);
                addToTranspositionTable(board, result);
                return result;
            }

            SearchResult? res = null;
            if (isMaximizingPlayer)
            {
                res = maxi(board, depth, alpha, beta);

            }
            else
            {
                res = mini(board, depth, alpha, beta);
            }

            return res;
        }

        private SearchResult maxi(Board board, int depth, float alpha, float beta)
        {
            float maxEval = float.MinValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);

            startTime = getCurrentTime();
            IEnumerable<SortedItem> sortedBoards = getSortedBoards(board, moves, true);
            sortTime.Increment(getCurrentTime() - startTime);
            Move? bestMove = null;

            foreach (SortedItem item in sortedBoards)
            {
                // Console.WriteLine(item.move);

                SearchResult result = Minimax(item.board, depth - 1, alpha, beta, false);

                if (result.evaluation > maxEval)
                {
                    maxEval = result.evaluation;
                    bestMove = item.move;
                }

                alpha = Math.Max(alpha, result.evaluation);

                if (beta <= alpha)
                {
                    break;
                }
            }

            SearchResult best = new SearchResult(maxEval, depth, bestMove!);
            addToTranspositionTable(board, best);
            // Console.WriteLine($"added: {best} to tt");
            return best;
        }

        private SearchResult mini(Board board, int depth, float alpha, float beta)
        {
            float minEval = float.MaxValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);

            startTime = getCurrentTime();
            IEnumerable<SortedItem> sortedBoards = getSortedBoards(board, moves, false);
            // IEnumerable<Move> sortedMoves = moves.OrderByDescending(move => getMovePriority(move, board, false));
            sortTime.Increment(getCurrentTime() - startTime);

            Move? bestMove = null;

            foreach (SortedItem item in sortedBoards)
            {
                
                // Console.WriteLine(item.move);

                SearchResult result = Minimax(item.board, depth - 1, alpha, beta, true);

                if (result.evaluation < minEval)
                {
                    minEval = result.evaluation;
                    bestMove = item.move;
                }

                beta = Math.Min(beta, result.evaluation);

                if (beta <= alpha)
                {
                    break;
                }
            }

            SearchResult best = new SearchResult(minEval, depth, bestMove!);
            addToTranspositionTable(board, best);
            // Console.WriteLine($"added: {best} to tt");
            return best;
        }

        private IEnumerable<SortedItem> getSortedBoards(Board board, List<Move> moves, bool isMaximizing)
        {
            List<SortedItem> items = new List<SortedItem>();
            foreach (Move move in moves)
            {
                long startTime = getCurrentTime();
                items.Add(new SortedItem(board.makeMove(move), move));
                moveMakingTime.Increment(getCurrentTime() - startTime);
            }

            IEnumerable<SortedItem> sorted = items.OrderByDescending((item) => getMovePriority(item.board, isMaximizing));

            return sorted;
        }

        private double getMovePriority(Board board, bool isMaximizing)
        {
            SearchResult? result = getFromTranspositionTable(board);

            //return lowest possible priority if no result has been found, moves will be searched last
            if (result == null) return double.MinValue;

            //prioritize moves with higher eval is maximizing, or with lower eval if minimizing
            return isMaximizing ? result.evaluation : -result.evaluation;
        }

        private class SortedItem
        {
            public Board board;
            public Move move;

            public SortedItem(Board board, Move move)
            {
                this.board = board;
                this.move = move;
            }
        }

    }
}