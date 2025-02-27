using chess;

namespace chessPlayer
{
    public class GameResult
    {
        public float result { get; private set; }
        public float whiteEval { get; private set; }
        public float blackEval { get; private set; }
        public float finalEval { get; private set; }

        public GameResult(float result, float whiteEval, float blackEval)
        {
            this.result = result;
            this.whiteEval = whiteEval;
            this.blackEval = blackEval;

            finalEval = (whiteEval + blackEval) / 2;
        }

        public static GameResult GetResult(Board board, Player white, Player black)
        {
            float result = 0;
            if (board.isInMate())
            {
                result = board.whiteToMove ? -1 : 1;
            }

            return new GameResult(result, white.evaluator.evaluate(board), black.evaluator.evaluate(board));
        }

        public override string ToString()
        {
            string winner = "";
            switch (result)
            {
                case 0: winner = "draw"; break;
                case 1: winner = "white won"; break;
                case -1: winner = "black won"; break;
            }
            return $"{winner}, eval: white:{whiteEval}, black:{blackEval}";
        }
    }
}