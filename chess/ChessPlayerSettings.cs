namespace chess
{
    public class ChessPlayerSettings
    {
        public bool limitedTurns;
        public int maxTurns;

        public bool limitedTime;
        public float maxTime;

        public bool limitedTurnTime;
        public float maxTurnTime;

        public ChessPlayerSettings()
        {
            this.limitedTurns = false;
            this.maxTurns = 0;
            this.limitedTime = false;
            this.maxTime = 0;
            this.limitedTurnTime = false;
            this.maxTurnTime = 0;
        }

        /// <summary>
        /// Creates a new chess player settings objects with the specified values,
        /// if a value is non-positive it is assumed the relevant setting is false
        /// </summary>
        /// <param name="maxTurns">The maximum number of full turns</param>
        /// <param name="maxTime">The maximum time of the game (in ms)</param>
        /// <param name="maxTurnTime">The maximum time of a turn (in ms)</param>
        public ChessPlayerSettings(int maxTurns, float maxTime, float maxTurnTime)
        {
            this.limitedTurns = maxTurns > 0;
            this.maxTurns = maxTurns;

            this.limitedTime = maxTime > 0;
            this.maxTime = maxTime;

            this.limitedTurnTime = maxTurnTime > 0;
            this.maxTurnTime = maxTime; 
        }

        public ChessPlayerSettings(bool limitedTurns, int maxTurns, bool limitedTime, float maxTime, bool limitedTurnTime, float maxTurnTime)
        {
            this.limitedTurns = limitedTurns;
            this.maxTurns = maxTurns;
            this.limitedTime = limitedTime;
            this.maxTime = maxTime;
            this.limitedTurnTime = limitedTurnTime;
            this.maxTurnTime = maxTurnTime;

        }
    }
}