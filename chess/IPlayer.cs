using chess.engine;

namespace chess
{   
    /// <summary>
    /// interface for a chess player
    /// </summary>
    public interface IPlayer
    {      
        /// <summary>
        /// The name of the player
        /// </summary>
        public string name { get; }
        /// <summary>
        /// The engine that the player uses to make its moves
        /// </summary>
        public Engine engine { get; }
    }
}