using chess;
using counters;

namespace iterative_deepening
{
    /// <summary>
    /// Engine that makes use of a minimax algorithm and a transposition table in order to find the best
    /// moves on a given board
    /// </summary>
    public class Engine : chess.engine.TTEngine
    {
        private long moveEndTime;

        /// <summary>
        /// Creates a new transposition table engine that optimizes moves for the white player
        /// </summary>
        public Engine() : this(true) { }

        /// <summary>
        /// Createst a new transposition table engine that optimizes for the selected player
        /// </summary>
        /// <param name="isWhite">true if optimizing for white, false for black</param>
        public Engine(bool isWhite) : base(isWhite, new Evaluator()) { }

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

            SearchResult result = Minimax(board, 1, float.MinValue, float.MaxValue, board.whiteToMove);
            addToTranspositionTable(board, result);

            computationTime.Set(getCurrentTime() - startTime);
            clearCounters();

            return result.move!;
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

            if (isMaximizingPlayer)
            {
                return maxi(board, depth, alpha, beta);
            }
            else
            {
                return mini(board, depth, alpha, beta);
            }
        }

        private SearchResult maxi(Board board, int depth, float alpha, float beta)
        {
            float maxEval = float.MinValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);

            IEnumerable<Move> sortedMoves = moves.OrderByDescending(move => getMovePriority(move, board));
            Move? bestMove = null;

            foreach (Move move in sortedMoves)
            {

                Board resultingBoard = board.makeMove(move);

                SearchResult result = Minimax(resultingBoard, depth - 1, alpha, beta, false);
                Console.WriteLine($"searching move {move}, result: {result}");

                if (result.evaluation > maxEval)
                {
                    maxEval = result.evaluation;
                    bestMove = move;
                }

                alpha = Math.Max(alpha, result.evaluation);

                if (beta <= alpha)
                {
                    break;
                }
            }

            SearchResult best = new SearchResult(maxEval, depth, bestMove!);
            addToTranspositionTable(board, best);
            return best;
        }

        private SearchResult mini(Board board, int depth, float alpha, float beta)
        {
            float minEval = float.MaxValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);

            IEnumerable<Move> sortedMoves = moves.OrderByDescending(move => getMovePriority(move, board));
            Move? bestMove = null;

            foreach (Move move in sortedMoves)
            {
                // Console.WriteLine($"searching move {move}");

                Board resultingBoard = board.makeMove(move);

                SearchResult result = Minimax(resultingBoard, depth - 1, alpha, beta, true);

                if (result.evaluation < minEval)
                {
                    minEval = result.evaluation;
                    bestMove = move;
                }

                beta = Math.Min(beta, result.evaluation);

                if (beta <= alpha)
                {
                    break;
                }
            }

            SearchResult best = new SearchResult(minEval, depth, bestMove!);
            addToTranspositionTable(board, best);
            return best;
        }

        private double getMovePriority(Move move, Board board)
        {
            return 0;
        }

    }
}