using chess;
using counters;

namespace improved_minimax_eval_engine
{
    /// <summary>
    /// Engine that makes use of a minimax algorithm with alpha beta pruning, with an improved evaluation function
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
            Console.WriteLine(config.maxDepth);
            long startTime = getCurrentTime();
            remainingTime = maxTime;

            Move? bestMove = null;
            float bestValue = board.whiteToMove ? float.MinValue : float.MaxValue;

            foreach (Move move in MoveGenerator.generateAllMoves(board))
            {
                float eval = Minimax(board.makeMove(move), config.maxDepth - 1, float.MinValue, float.MaxValue, !board.whiteToMove);
                if (board.whiteToMove && eval > bestValue)
                {
                    bestValue = eval;
                    bestMove = move;
                }
                else if (!board.whiteToMove && eval < bestValue)
                {
                    bestValue = eval;
                    bestMove = move;
                }
            }

            computationTime.Set(getCurrentTime() - startTime);
            clearCounters();
            return bestMove!;
        }

        private float maxi(Board board, int depth, float alpha, float beta)
        {
            float maxEval = float.MinValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);
            remainingTime -= getCurrentTime() - startTime;

            foreach (Move move in moves)
            {
                startTime = getCurrentTime();
                Board resultingBoard = board.makeMove(move);
                remainingTime -= getCurrentTime() - startTime;

                float eval = Minimax(resultingBoard, depth - 1, alpha, beta, false);
                maxEval = Math.Max(maxEval, eval);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha)
                {
                    break;
                }
            }

            return maxEval;
        }

        private float mini(Board board, int depth, float alpha, float beta)
        {
            float minEval = float.MaxValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);
            remainingTime -= getCurrentTime() - startTime;

            foreach (Move move in moves)
            {
                startTime = getCurrentTime();
                Board resultingBoard = board.makeMove(move);
                remainingTime -= getCurrentTime() - startTime;

                float eval = Minimax(resultingBoard, depth - 1, alpha, beta, true);

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);

                if (beta <= alpha)
                {
                    break;
                }
            }
            return minEval;
        }

        private float Minimax(Board board, int depth, float alpha, float beta, bool isMaximizingPlayer)
        {
            long startTime;

            if (depth == 0 || board.isInMate() || remainingTime <= 0)
            {
                evaluatedBoards.Increment();

                startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime.Increment(getCurrentTime() - startTime);

                remainingTime -= getCurrentTime() - startTime;

                return eval;
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