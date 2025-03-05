namespace counters
{
    /// <summary>
    /// Interface for the counter class
    /// </summary>
    public interface ICounter
    {   
        /// <summary>
        /// Prints the current value of the counter to the console
        /// </summary>
        public void DisplayValue();

        /// <summary>
        /// Displays an statistic overview of the history of stored values
        /// </summary>
        public void DisplayOverview();

        /// <summary>
        /// Displays an statistic overview of the history of stored values
        /// </summary>
        /// <param name="showComparision">Whether to show the percentage difference with the stored counter values</param>
        public void DisplayOverview(bool showComparision);

        /// <summary>
        /// Resets the current value of the counter and adds it to the history
        /// </summary>
        public void Reset();
        
        /// <summary>
        /// Write all stored values to a file
        /// </summary>
        public void write();
    }
}