namespace chess
{
    using engine;
    public class Player
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
        /// The evaluator the player uses
        /// </summary>
        public Evaluator evaluator { get; private set; }

        public Player(string name, Engine engine, Evaluator evaluator)
        {
            this.name = name;
            this.engine = engine;
            this.evaluator = evaluator;
        }
    }
}
