using chess;

namespace minimax_engine
{
    public class Engine : chess.engine.Engine
    {

        // private const int MAX_DEPTH = 2;
        private int evaluatedBoards;
        private long computationTime;
        private long evaluationTime;
        private long generationTime;
        private int prunedBranches;

        private float remainingTime;        
        public Engine() : this(true) { }

        public Engine(bool isWhite) : base(isWhite, new Evaluator())
        {
            evaluatedBoards = 0;
            computationTime = 0;
            evaluationTime = 0;
            generationTime = 0;
            prunedBranches = 0;
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

            computationTime = getCurrentTime() - startTime;

            return result.move!;
        }

        private SearchResult maxi(Board board, float alpha, float beta, int depth)
        {
            long startTime;
            if (depth == 0 || board.isInMate() || remainingTime <= 0)
            {
                evaluatedBoards++;

                startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime += getCurrentTime() - startTime;

                remainingTime -= getCurrentTime() - startTime;
                return new SearchResult(eval, 0);
            }

            float max = float.MinValue;
            Move? bestMove = null;

            startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime += getCurrentTime() - startTime;

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
                evaluatedBoards++;

                startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime += getCurrentTime() - startTime;

                remainingTime -= getCurrentTime() - startTime;
                return new SearchResult(eval, 0);
            }

            float min = float.MaxValue;
            Move? bestMove = null;

            startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime += getCurrentTime() - startTime;

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
                    return new SearchResult(result.evaluation, depth, bestMove!);
                }
            }

            return new SearchResult(min, depth, bestMove!);
        }

        private void displayOverview()
        {
            Console.WriteLine("-- Computation time: " + computationTime + "ms");
            Console.WriteLine("-- Time spent generating moves: " + generationTime + "ms");
            Console.WriteLine("-- Time spent evaluating: " + evaluationTime + "ms");
            Console.WriteLine("-- Boards evaluated: " + evaluatedBoards + " ( " + prunedBranches + " branches pruned )");
        }

        private void clearIntCounters()
        {
            computationTime = 0;
            evaluationTime = 0;
            generationTime = 0;
            evaluatedBoards = 0;
            prunedBranches = 0;
        }
    }
}