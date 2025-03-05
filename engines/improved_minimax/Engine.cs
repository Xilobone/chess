using chess;
using chess.engine;

namespace improved_minimax_engine
{
    /// <summary>
    /// Engine that uses an improved version of the minimax algorithm with alpha beta pruning
    /// </summary>
    public class Engine : chess.engine.Engine
    {

        private float remainingTime;

        /// <summary>
        /// Creates a new engine
        /// </summary>
        public Engine() : this(true) { }

        /// <summary>
        /// Creates a new engine
        /// </summary>
        /// <param name="isWhite">Whether the engine is optimizing for white or not</param>
        public Engine(bool isWhite) : base(isWhite, new Evaluator()) { }

        /// <summary>
        /// Computes the best move to make on the given board
        /// </summary>
        /// <param name="board">The board to make a move on</param>
        /// <returns>The best move to make on the board</returns>
        public override Move makeMove(Board board)
        {
            return makeMove(board, float.MaxValue);
        }

        /// <summary>
        /// Computes the best move to make on the given board within the time limit
        /// </summary>
        /// <param name="board">The board to make a move on</param>
        /// <param name="maxTime">The maximum allowed time to compute for</param>
        /// <returns>The best move to make on the board</returns>
        public override Move makeMove(Board board, float maxTime)
        {
            remainingTime = maxTime;
            long startTime = getCurrentTime();

            SearchResult result;
            if (isWhite)
            {
                result = maxi(board, float.MinValue, float.MaxValue, config.maxDepth);
            }
            else
            {
                result = mini(board, float.MinValue, float.MaxValue, config.maxDepth);
            }

            computationTime.Set(getCurrentTime() - startTime);

            clearCounters();

            return result.move!;
        }

        private SearchResult maxi(Board board, float alpha, float beta, int depth)
        {
            long startTime;
            if (depth == 0 || board.isInMate() || remainingTime <= 0)
            {
                evaluatedBoards.Increment();

                startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime.Increment(getCurrentTime() - startTime);

                remainingTime -= getCurrentTime() - startTime;
                return new SearchResult(eval, 0);
            }

            float max = float.MinValue;
            Move? bestMove = null;

            startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);

            remainingTime -= getCurrentTime() - startTime;

            foreach (Move move in moves)
            {
                startTime = getCurrentTime();
                Board resultingBoard = board.makeMove(move);
                remainingTime -= getCurrentTime() - startTime;

                SearchResult result = mini(resultingBoard, alpha, beta, depth - 1);

                if (result.evaluation > max)
                {
                    max = alpha;
                    if (result.evaluation > alpha)
                    {
                        alpha = result.evaluation;
                        bestMove = move;
                    }
                }

                if (result.evaluation >= beta)
                {
                    return new SearchResult(result.evaluation, depth, bestMove!);
                }
            }

            return new SearchResult(max, depth, bestMove!);
        }

        private SearchResult mini(Board board, float alpha, float beta, int depth)
        {
            long startTime;

            if (depth == 0 || board.isInMate() || remainingTime <= 0)
            {
                evaluatedBoards.Increment();

                startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime.Increment(getCurrentTime() - startTime);

                remainingTime -= getCurrentTime() - startTime;
                return new SearchResult(eval, 0);
            }

            float min = float.MaxValue;
            Move? bestMove = null;

            startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);

            remainingTime -= getCurrentTime() - startTime;

            foreach (Move move in moves)
            {
                startTime = getCurrentTime();
                Board resultingBoard = board.makeMove(move);
                remainingTime -= getCurrentTime() - startTime;

                SearchResult result = maxi(resultingBoard, alpha, beta, depth - 1);

                if (result.evaluation < min)
                {
                    min = result.evaluation;
                    if (result.evaluation < beta)
                    {
                        beta = result.evaluation;
                        bestMove = move;
                    }
                }

                if (result.evaluation <= alpha)
                {
                    return new SearchResult(result.evaluation, depth, bestMove!);
                }
            }

            return new SearchResult(min, depth, bestMove!);
        }
    }
}