namespace chess
{
    public class Engine : IPlayer
    {

        private const int MAX_DEPTH = 2;

        private bool isWhite;
        private int depth;
        private int evaluatedBoards;
        private long computationTime;
        private long evaluationTime;
        private long generationTime;
        private int prunedBranches;

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
            if (depth == 0 || board.isInMate())
            {
                evaluatedBoards++;

                long startTime = getCurrentTime();
                float eval = Evaluator.evaluate(board);
                evaluationTime += getCurrentTime() - startTime;

                return new SearchResult(eval, null);
            }

            float max = float.MinValue;
            Move? bestMove = null;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Position pos = new Position(x, y);
                    int piece = board.getPiece(pos);

                    if (!Piece.isItsTurn(piece, board.whiteToMove))
                    {
                        continue;
                    }

                    long startTime = getCurrentTime();
                    List<Move> moves = MoveGenerator.generateMoves(board, pos);
                    //Console.WriteLine("Length of moves list:" + moves.Count);
                    generationTime += getCurrentTime() - startTime;

                    foreach (Move move in moves)
                    {
                        Board resultingBoard = board.getCopy();
                        resultingBoard.makeMove(move);

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
                }
            }

            //Console.WriteLine("returning at end of maxi, move:" + bestMove);
            return new SearchResult(max, bestMove);
        }

        private SearchResult mini(Board board, float alpha, float beta, int depth)
        {
            if (depth == 0 || board.isInMate())
            {
                evaluatedBoards++;

                long startTime = getCurrentTime();
                float eval = Evaluator.evaluate(board);
                evaluationTime += getCurrentTime() - startTime;

                return new SearchResult(eval, null);
            }

            float min = float.MaxValue;
            Move? bestMove = null;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Position pos = new Position(x, y);
                    int piece = board.getPiece(pos);

                    if (!Piece.isItsTurn(piece, board.whiteToMove))
                    {
                        continue;
                    }

                    long startTime = getCurrentTime();
                    List<Move> moves = MoveGenerator.generateMoves(board, pos);
                    generationTime += getCurrentTime() - startTime;

                    foreach (Move move in moves)
                    {
                        Board resultingBoard = board.getCopy();
                        resultingBoard.makeMove(move);

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