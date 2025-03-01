using chess;

namespace improved_minimax_engine
{
    public class Evaluator : chess.Evaluator
    {
        public override float evaluate(Board board)
        {
            float eval = 0;

            if (board.isInDraw()) return eval;

            eval += 2 * getPieceValue(board);
            eval += 0.2f * getPawnChain(board);
            eval += 0.5f * getCenterControl(board);
            eval += 0.5f * getCheck(board);
            eval += 100000 * getMate(board);

            return eval;
        }
    }
}