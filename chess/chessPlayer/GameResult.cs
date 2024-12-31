using chess;

namespace chessPlayer
{
    public class GameResult
    {
        public float result { get; private set; }
        public float finalEval { get; private set; }

        public GameResult(float result, float finalEval)
        {
            this.result = result;
            this.finalEval = finalEval;
        }

        public static GameResult GetResult(Board board, Player white, Player black)
        {
            float result = 0;
            if(board.isInMate())
            {
                result = board.whiteToMove ? -1 : 1;
            }

            float eval = (white.evaluator.evaluate(board) + black.evaluator.evaluate(board)) / 2;
            return new GameResult(result, eval);
        }
    }
}