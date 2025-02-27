using chess;
using chess.engine;
using counters;

namespace transposition_table
{
    /// <summary>
    /// Engine that makes use of a minimax algorithm and a transposition table in order to find the best
    /// moves on a given board
    /// </summary>
    public class Engine : TTEngine
    {
        /// <summary>
        /// Counter that stores the amount of boards that are evaluated for each move
        /// </summary>
        public Counter<int> evaluatedBoards { get; private set; }

        /// <summary>
        /// Counter that stores the computation time (in ms) for each move
        /// </summary>
        public Counter<long> computationTime { get; private set; }

        /// <summary>
        /// Counter that stores the time (in ms) spent evaluating for each move
        /// </summary>
        public Counter<long> evaluationTime { get; private set; }

        /// <summary>
        /// Counter that stored the time (in ms) spent generating moves for each move
        /// </summary>
        public Counter<long> generationTime { get; private set; }
        private long moveEndTime;

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
            evaluatedBoards = new Counter<int>("Evaluated boards");
            computationTime = new Counter<long>("Computation time", "ms");
            evaluationTime = new Counter<long>("Evaluation time", "ms");
            generationTime = new Counter<long>("Generation time", "ms");
            counters.AddRange(evaluatedBoards, computationTime, evaluationTime, generationTime);
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

            SearchResult bestResult = new SearchResult(board.whiteToMove ? float.MinValue : float.MaxValue, -1);
            Move? bestMove = null;

            List<Move> moves = MoveGenerator.generateAllMoves(board);
            foreach (Move move in moves)
            {
                Board resultingBoard = board.makeMove(move);

                SearchResult result = Minimax(resultingBoard, config.maxDepth - 1, float.MinValue, float.MaxValue, !board.whiteToMove);

                if (board.whiteToMove && result.evaluation > bestResult.evaluation)
                {
                    bestResult = result;
                    bestMove = move;
                }
                else if (!board.whiteToMove && result.evaluation < bestResult.evaluation)
                {
                    bestResult = result;
                    bestMove = move;
                }
            }

            //add board to transposition table
            addToTranspositionTable(board, new SearchResult(bestResult.evaluation, config.maxDepth, bestMove!));

            computationTime.Set(getCurrentTime() - startTime);
            clearCounters();

            return bestMove!;
        }

        private SearchResult maxi(Board board, int depth, float alpha, float beta)
        {
            float maxEval = float.MinValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);

            Move? bestMove = null;

            foreach (Move move in moves)
            {
                startTime = getCurrentTime();
                Board resultingBoard = board.makeMove(move);

                SearchResult result = Minimax(resultingBoard, depth - 1, alpha, beta, false);

                if (result.evaluation > maxEval)
                {
                    maxEval = result.evaluation;
                    bestMove = result.move;
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

            Move? bestMove = null;

            foreach (Move move in moves)
            {
                startTime = getCurrentTime();
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

        private SearchResult Minimax(Board board, int depth, float alpha, float beta, bool isMaximizingPlayer)
        {
            long startTime;

            // check if this board has been stored in the transposition table, and if the board has the correct hash
            SearchResult? storedResult = getFromTranspositionTable(board);

            if (storedResult != null && storedResult.searchedDepth >= depth)
            {
                return storedResult;
            }

            if (depth == 0 || board.isInMate() || getCurrentTime() >= moveEndTime)
            {
                evaluatedBoards.Increment();

                startTime = getCurrentTime();
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
    }
}