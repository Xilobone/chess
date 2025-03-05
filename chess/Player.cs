namespace chess
{
    using engine;

    /// <summary>
    /// Represents an agent that can play a game of chess
    /// </summary>
    public class Player<T> : IPlayer where T : Engine, new()
    {
        /// <summary>
        /// The name of the player
        /// </summary>
        public string name { get; private set; }

        /// <summary>
        /// The engine the player uses
        /// </summary>
        public Engine engine { get; private set; }

        /// <summary>
        /// Creates a new player
        /// </summary>
        public Player()
        {
            engine = new T();
            name = engine.GetType().Namespace!;
        }
    }
}
