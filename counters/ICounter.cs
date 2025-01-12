namespace counters
{
    public interface ICounter
    {
        public void DisplayValue();
        public void DisplayOverview();
        public void DisplayOverview(bool showComparision);

        public void write();
    }
}