using counters;

namespace chess
{
    public abstract class Engine
    {
        public bool isWhite { get; set; }
        public bool displayStats { get; set; }
        public List<ICounter> counters { get; set; }

        protected IEvaluator evaluator;

        public Engine(bool isWhite, IEvaluator evaluator)
        {
            counters = new List<ICounter>();

            this.isWhite = isWhite;
            this.evaluator = evaluator;
        }
        public abstract Move makeMove(Board board);
        public abstract Move makeMove(Board board, float maxTime);

        protected void clearCounters()
        {
            foreach(ICounter counter in counters)
            {
                counter.Reset();
            }
        }
        protected long getCurrentTime()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        public class SearchResult
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