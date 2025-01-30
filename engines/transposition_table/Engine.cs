using chess;
using counters;

namespace transposition_table
{
    public class Engine : chess.Engine
    {
        private const int MAX_DEPTH = 1;

        private int depth;

        public Counter<int> evaluatedBoards { get; private set; }
        public Counter<long> computationTime { get; private set; }
        public Counter<long> evaluationTime { get; private set; }
        public Counter<long> generationTime { get; private set; }


        private float remainingTime;

        public Engine() : this(true, MAX_DEPTH) { }

        public Engine(bool isWhite) : this(isWhite, MAX_DEPTH) { }

        public Engine(bool isWhite, int depth) : base(isWhite, new Evaluator())
        {
            this.depth = depth;

            evaluatedBoards = new Counter<int>("Evaluated boards");
            computationTime = new Counter<long>("Computation time", "ms");
            evaluationTime = new Counter<long>("Evaluation time", "ms");
            generationTime = new Counter<long>("Generation time", "ms");
            counters.AddRange(evaluatedBoards, computationTime, evaluationTime, generationTime);
        }

        public override Move makeMove(Board board)
        {
            return makeMove(board, float.MaxValue);
        }

        public override Move makeMove(Board board, float maxTime)
        {
            long startTime = getCurrentTime();
            remainingTime = maxTime;

            Move? bestMove = null;
            float bestValue = board.whiteToMove ? float.MinValue : float.MaxValue;

            List<Move> moves = MoveGenerator.generateAllMoves(board);
            List<MovePair> sorted = new List<MovePair>();

            foreach (Move move in moves)
            {
                float eval = Minimax(board.makeMove(move), depth - 1, float.MinValue, float.MaxValue, !board.whiteToMove);
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

        public float maxi(Board board, int depth, float alpha, float beta)
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

        public float mini(Board board, int depth, float alpha, float beta)
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

        public float Minimax(Board board, int depth, float alpha, float beta, bool isMaximizingPlayer)
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

        private class MovePair
        {
            public Move move;
            public float eval;

            public MovePair(Move move, float eval)
            {
                this.move = move;
                this.eval = eval;
            }
        }
    }
}