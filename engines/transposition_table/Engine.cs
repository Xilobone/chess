using System.DirectoryServices.ActiveDirectory;
using System.Reflection.Emit;
using chess;
using counters;

namespace transposition_table
{
    public class Engine : chess.Engine
    {
        public Counter<int> evaluatedBoards { get; private set; }
        public Counter<long> computationTime { get; private set; }
        public Counter<long> evaluationTime { get; private set; }
        public Counter<long> generationTime { get; private set; }


        private float remainingTime;

        public SearchResult[] transpositionTable { get; private set; }
        public int overwrittenSearchResults { get; private set; }

        public Engine() : this(true) { }


        public Engine(bool isWhite) : base(isWhite, new Evaluator())
        {
            evaluatedBoards = new Counter<int>("Evaluated boards");
            computationTime = new Counter<long>("Computation time", "ms");
            evaluationTime = new Counter<long>("Evaluation time", "ms");
            generationTime = new Counter<long>("Generation time", "ms");
            counters.AddRange(evaluatedBoards, computationTime, evaluationTime, generationTime);

            transpositionTable = new SearchResult[config.transpositionTableSize];
            overwrittenSearchResults = 0;
        }

        public override Move makeMove(Board board)
        {
            return makeMove(board, float.MaxValue);
        }

        public override Move makeMove(Board board, float maxTime)
        {
            long startTime = getCurrentTime();
            remainingTime = maxTime;

            SearchResult bestResult = new SearchResult(board.whiteToMove ? float.MinValue : float.MaxValue, -1);

            List<Move> moves = MoveGenerator.generateAllMoves(board);

            foreach (Move move in moves)
            {
                SearchResult result = Minimax(board.makeMove(move), config.maxDepth - 1, float.MinValue, float.MaxValue, !board.whiteToMove);
                if (board.whiteToMove && result.evaluation > bestResult.evaluation)
                {
                    bestResult = result;
                }
                else if (!board.whiteToMove && result.evaluation < bestResult.evaluation)
                {
                    bestResult = result;
                }

                //add board to transposition table
                int index = Zobrist.hash(board) % config.transpositionTableSize;
                if (transpositionTable[index] != null) overwrittenSearchResults++;
                transpositionTable[index] = bestResult;
            }

            

            computationTime.Set(getCurrentTime() - startTime);
            clearCounters();
            return bestResult.move!;
        }

        public SearchResult maxi(Board board, int depth, float alpha, float beta)
        {
            float maxEval = float.MinValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);
            remainingTime -= getCurrentTime() - startTime;

            Move? bestMove = null;

            foreach (Move move in moves)
            {
                startTime = getCurrentTime();
                Board resultingBoard = board.makeMove(move);
                remainingTime -= getCurrentTime() - startTime;

                SearchResult result = Minimax(resultingBoard, depth - 1, alpha, beta, false);

                //add result to transposition table
                int index = Zobrist.hash(resultingBoard) % config.transpositionTableSize;
                if (transpositionTable[index] != null) overwrittenSearchResults++;
                transpositionTable[index] = result;

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

            return new SearchResult(maxEval, depth, bestMove!);
        }

        public SearchResult mini(Board board, int depth, float alpha, float beta)
        {
            float minEval = float.MaxValue;
            long startTime = getCurrentTime();
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            generationTime.Increment(getCurrentTime() - startTime);
            remainingTime -= getCurrentTime() - startTime;

            Move? bestMove = null;

            foreach (Move move in moves)
            {
                startTime = getCurrentTime();
                Board resultingBoard = board.makeMove(move);
                remainingTime -= getCurrentTime() - startTime;

                SearchResult result = Minimax(resultingBoard, depth - 1, alpha, beta, true);

                //add result to transposition table
                int index = Zobrist.hash(resultingBoard) % config.transpositionTableSize;
                if (transpositionTable[index] != null) overwrittenSearchResults++;
                transpositionTable[index] = result;

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

            return new SearchResult(minEval, depth, bestMove!);
        }

        public SearchResult Minimax(Board board, int depth, float alpha, float beta, bool isMaximizingPlayer)
        {
            long startTime;

            if (depth == 0 || board.isInMate() || remainingTime <= 0)
            {
                evaluatedBoards.Increment();

                startTime = getCurrentTime();
                float eval = evaluator.evaluate(board);
                evaluationTime.Increment(getCurrentTime() - startTime);

                SearchResult result = new SearchResult(eval, 0);

                remainingTime -= getCurrentTime() - startTime;

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