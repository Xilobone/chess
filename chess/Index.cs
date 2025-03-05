namespace chess
{
    /// <summary>
    /// Class containing helper functions for indexes
    /// </summary>
    public static class Index
    {
        /// <summary>
        /// Gets the file (vertical line) of the index
        /// </summary>
        /// <param name="index">The index to get the file of</param>
        /// <returns>TThe file of the index</returns>
        public static int GetFile(int index)
        {
            return index - 8 * GetRank(index);
        }

        /// <summary>
        /// Gets the file (horizontal line) of the index
        /// </summary>
        /// <param name="index">The index to get the rank of</param>
        /// <returns>TThe rank of the index</returns>
        public static int GetRank(int index)
        {
            return index / 8;
        }
    }
}