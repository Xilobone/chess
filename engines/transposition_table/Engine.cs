using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        public Counter<int> transpositionTableBoards { get; private set; }
        public Counter<int> hashCollisions { get; private set; }

        private long moveEndTime;

        public SearchResult[] transpositionTable { get; private set; }
        public int overwrittenSearchResults { get; private set; }

        public Engine() : this(true) { }


        public Engine(bool isWhite) : base(isWhite, new Evaluator())
        {
            evaluatedBoards = new Counter<int>("Evaluated boards");
            computationTime = new Counter<long>("Computation time", "ms");
            evaluationTime = new Counter<long>("Evaluation time", "ms");
            generationTime = new Counter<long>("Generation time", "ms");
            transpositionTableBoards = new Counter<int>("items added to transpositiontable");
            hashCollisions = new Counter<int>("hash collisions");
            counters.AddRange(evaluatedBoards, computationTime, evaluationTime, generationTime, transpositionTableBoards, hashCollisions);

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
            ulong hash = Zobrist.hash(board);
            ulong index = hash % config.transpositionTableSize;
            if (transpositionTable[index] != null && transpositionTable[index].hash == hash)
            {
                SearchResult storedResult = transpositionTable[index];

                //return stored result if it has been searched to at least the desired depth
                if (storedResult.searchedDepth >= depth)
                {
                    // Console.WriteLine($"Found already computed result: {storedResult}, hash:{index}, board:");

                    return storedResult;
                }
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

        private void addToTranspositionTable(Board board, SearchResult result)
        {
            ulong hash = Zobrist.hash(board);
            ulong index = hash % config.transpositionTableSize;
            result.hash = hash;
            //there is already a result stored at the given index
            if (transpositionTable[index] != null)
            {   
                //hash collision
                if (transpositionTable[index].hash != result.hash) hashCollisions.Increment();

                //the stored result has a greater depth than the new result, do not overrwide result
                if (transpositionTable[index].searchedDepth < result.searchedDepth) return;
            }

            transpositionTableBoards.Increment();
            transpositionTable[index] = result;
        }

        public void clearTranspositionTable()
        {
            transpositionTable = new SearchResult[config.transpositionTableSize];
        }
    }


}