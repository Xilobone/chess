using chess;
using counters;

namespace improved_minimax_engine
{
    public class Engine : chess.engine.Engine
    {
        public Counter<int> evaluatedBoards { get; private set; }
        public Counter<long> computationTime { get; private set; }
        public Counter<long> evaluationTime { get; private set; }
        public Counter<long> generationTime { get; private set; }

        private float remainingTime;

        public Engine() : this(true) { }


        public Engine(bool isWhite) : base(isWhite, new Evaluator())
        {

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