namespace chess
{
    public class Index
    {
        public static int GetFile(int index)
        {
            return index - 8 * GetRank(index);
        }

        public static int GetRank(int index)
        {
            return index / 8;
        }
    }
}