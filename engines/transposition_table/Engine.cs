using chess;
using chess.engine;
using counters;

namespace transposition_table
{
    public class Engine : TTEngine
    {
        public Counter<int> evaluatedBoards { get; private set; }
        public Counter<long> computationTime { get; private set; }
        public Counter<long> evaluationTime { get; private set; }
        public Counter<long> generationTime { get; private set; }
        private long moveEndTime;

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

        public SearchResult maxi(Board board, int depth, float alpha, float beta)
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

        public SearchResult mini(Board board, int depth, float alpha, float beta)
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

        public SearchResult Minimax(Board board, int depth, float alpha, float beta, bool isMaximizingPlayer)
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