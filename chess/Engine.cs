using counters;

namespace chess
{
    public abstract class Engine
    {
        public bool isWhite { get; set; }
        public bool displayStats { get; set; }
        public List<ICounter> counters { get; set; }

        public Engine()
        {
            counters = new List<ICounter>();
        }
        public abstract Move makeMove(Board board);
        public abstract Move makeMove(Board board, float maxTime);
    }
}