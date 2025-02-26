namespace chess.engine
{
    public class InvalidEngineException : Exception
    {
        public InvalidEngineException() : base("This engine is invalid") { }
        public InvalidEngineException(string message) : base(message) { }
        
    }
}