using chess;

namespace minimax_engine
{
    public class Engine : IEngine
    {

        private const int MAX_DEPTH = 2;
        private int depth;
        private int evaluatedBoards;
        private long computationTime;
        private long evaluationTime;
        private long generationTime;
        private int prunedBranches;

        private Evaluator evaluator;

        public bool isWhite { get; set; }

        public Engine() : this(true, MAX_DEPTH) { }

        public Engine(bool isWhite) : this(isWhite, MAX_DEPTH) { }

        public Engine(bool isWhite, int depth)
        {
            this.isWhite = isWhite;
            this.depth = depth;

            evaluatedBoards = 0;
            computationTime = 0;
            evaluationTime = 0;
            generationTime = 0;
            prunedBranches = 0;

            evaluator = new Evaluator();
        }

        public Move makeMove(Board board, float maxTime)
        {
            return makeMove(board);
        }
        public Move makeMove(Board board)
        {
            long startTime = getCurrentTime();

            SearchResult result;
            if (isWhite)
            {
                //Console.WriteLine("running maxi");
                result = maxi(board, float.MinValue, float.MaxValue, depth);
            }
            else
            {
                result = mini(board, float.MinValue, float.MaxValue, depth);
            }

            computationTime = getCurrentTime() - startTime;

            displayOverview();
            clearCounters();

            //Console.WriteLine(result.move);
            return result.move!;
        }

        private SearchResult maxi(Board board, float alpha, float beta, int depth)
        {
            long startTime;
            if (depth == 0 || board.isInMate())
            {
                evaluatedBoards++;

                startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime += getCurrentTime() - startTime;

                return new SearchResult(eval, null);
            }

            float max = float.MinValue;
            Move? bestMove = null;

            startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime += getCurrentTime() - startTime;

            foreach (Move move in moves)
            {
                Board resultingBoard = board.makeMove(move);

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
                    //Console.WriteLine("Beta pruning, move:", bestMove);
                    return new SearchResult(result.evaluation, bestMove);
                }
            }


            //Console.WriteLine("returning at end of maxi, move:" + bestMove);
            return new SearchResult(max, bestMove);
        }

        private SearchResult mini(Board board, float alpha, float beta, int depth)
        {
            long startTime;

            if (depth == 0 || board.isInMate())
            {
                evaluatedBoards++;

                startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime += getCurrentTime() - startTime;

                return new SearchResult(eval, null);
            }

            float min = float.MaxValue;
            Move? bestMove = null;

            startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime += getCurrentTime() - startTime;

            foreach (Move move in moves)
            {
                Board resultingBoard = board.makeMove(move);

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

        private void displayOverview()
        {
            Console.WriteLine("-- Computation time: " + computationTime + "ms");
            Console.WriteLine("-- Time spent generating moves: " + generationTime + "ms");
            Console.WriteLine("-- Time spent evaluating: " + evaluationTime + "ms");
            Console.WriteLine("-- Boards evaluated: " + evaluatedBoards + " ( " + prunedBranches + " branches pruned )");
        }

        private void clearCounters()
        {
            computationTime = 0;
            evaluationTime = 0;
            generationTime = 0;
            evaluatedBoards = 0;
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