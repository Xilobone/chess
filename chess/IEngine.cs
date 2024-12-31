namespace chess
{
    public interface IEngine
    {
        public bool isWhite { get; set; }
        public bool displayStats { get; set; }
        public Move makeMove(Board board);
        public Move makeMove(Board board, float maxTime);
    }
}