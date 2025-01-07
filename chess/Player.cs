namespace chess
{
    public class Player
    {
        public string name { get; private set; }
        public Engine engine { get; private set; }
        public IEvaluator evaluator { get; private set; }

        public Player(string name, Engine engine, IEvaluator evaluator)
        {
            this.name = name;
            this.engine = engine;
            this.evaluator = evaluator;
        }
    }
}
