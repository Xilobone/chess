using chess;
using counters;

namespace improved_minimax_engine
{
    public class Engine : chess.Engine
    {
        private const int MAX_DEPTH = 3;

        private int depth;

        public Counter<int> evaluatedBoards { get; private set; }
        public Counter<long> computationTime { get; private set; }
        public Counter<long> evaluationTime { get; private set; }
        public Counter<long> generationTime { get; private set; }
        private int prunedBranches;

        private Evaluator evaluator;

        private float remainingTime;

        public Engine() : this(true, MAX_DEPTH) { }

        public Engine(bool isWhite) : this(isWhite, MAX_DEPTH) { }

        public Engine(bool isWhite, int depth)
        {
            this.isWhite = isWhite;
            this.depth = depth;

            evaluatedBoards = new Counter<int>("Evaluated boards");
            computationTime = new Counter<long>("Computation time", "ms");
            evaluationTime = new Counter<long>("Evaluation time", "ms");
            generationTime = new Counter<long>("Generation time", "ms");
            counters.AddRange(evaluatedBoards, computationTime, evaluationTime, generationTime);

            prunedBranches = 0;

            evaluator = new Evaluator();
        }

        public override Move makeMove(Board board)
        {
            return makeMove(board, float.MaxValue);
        }

        public override Move makeMove(Board board, float maxTime)
        {
            remainingTime = maxTime;
            long startTime = getCurrentTime();

            SearchResult result;
            if (isWhite)
            {
                result = maxi(board, float.MinValue, float.MaxValue, depth);
            }
            else
            {
                result = mini(board, float.MinValue, float.MaxValue, depth);
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
                return new SearchResult(eval, null);
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
                    prunedBranches += moves.Count() - moves.IndexOf(move) - 1;
                    return new SearchResult(result.evaluation, bestMove);
                }
            }

            return new SearchResult(max, bestMove);
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
                return new SearchResult(eval, null);
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
                    prunedBranches += moves.Count() - moves.IndexOf(move) - 1;
                    return new SearchResult(result.evaluation, bestMove);
                }
            }

            return new SearchResult(min, bestMove);
        }

        private void clearCounters()
        {
            computationTime.Reset();
            evaluationTime.Reset();
            generationTime.Reset();
            evaluatedBoards.Reset();
            prunedBranches = 0;
        }

        private long getCurrentTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private class SearchResult
        {
            public float evaluation;
            public Move? move;

            public SearchResult(float evaluation, Move? move)
            {
                this.evaluation = evaluation;
                this.move = move;
            }
        }
    }
}