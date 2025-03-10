using System.Text.Json;
using System.Text.Json.Serialization;

namespace chess.engine
{
    /// <summary>
    /// Represents the result of a engine search
    /// </summary>
    public class SearchResult
    {

        /// <summary>
        /// The evaluation of the result
        /// </summary>
        public float evaluation;

        /// <summary>
        /// The depth at which the result was searched at
        /// </summary>
        public int searchedDepth;

        /// <summary>
        /// The hash of the board that was searched
        /// </summary>
        public ulong hash;

        /// <summary>
        /// The best move to make on the board
        /// </summary>
        public Move? move;

        /// <summary>
        /// Creates a new search result
        /// </summary>
        /// <param name="evaluation">The evaluation of the result</param>
        /// <param name="searchedDepth">The depth at which the board was searched at</param>
        /// <param name="move">The best move to make on the board</param>
        public SearchResult(float evaluation, int searchedDepth, Move move)
        {
            this.evaluation = evaluation;
            this.searchedDepth = searchedDepth;
            this.move = move;
        }

        /// <summary>
        /// Creates a new search result
        /// </summary>
        /// <param name="evaluation">The evaluation of the result</param>
        /// <param name="searchedDepth">The depth at which the board was searched at</param>
        public SearchResult(float evaluation, int searchedDepth)
        {
            this.evaluation = evaluation;
            this.searchedDepth = searchedDepth;
        }

        /// <summary>
        /// Creates a string representation of the result
        /// </summary>
        /// <returns>The string representation of the result</returns>
        public override string ToString()
        {
            return JsonSerializer.Serialize(new
            {
                evaluation,
                searchedDepth,
                hash,
                move = move?.ToString() ?? "No move"
            });
        }
    }
}