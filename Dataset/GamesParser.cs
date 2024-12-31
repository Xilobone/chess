using chess;
using converter;

namespace parser
{
    public class GamesParser
    {
        private string filePath;
        public GamesParser(string filePath)
        {
            this.filePath = filePath; 
        }

        /// <summary>
        /// Gets a list of unique boards from the file
        /// </summary>
        /// <param name="listSize">The amount of boards to include in the search</param>
        /// <param name="amount">The amount of boards to save</param>
        /// <returns></returns>
        public List<Board> parse(int listSize, int amount)
        {
            List<Board> boards = new List<Board>();

            string[] lines = File.ReadAllLines(filePath);
            for(int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                //skip over empty lines of lines containing metadata
                if (string.IsNullOrEmpty(line)) continue;
                if (line[0] == '[') continue;

                boards.AddRange(getAllPositions(line));
                Console.WriteLine($"done {i}/{lines.Length} lines ({(double) (i * 100) / lines.Length}%), boards size:{boards.Count}");

                if (boards.Count >= listSize) break;
            }

            return boards;
        }

        private List<Board> getAllPositions(string line)
        {
            List<Board> boards = new List<Board>();
            Board board = Board.startPosition();

            //moves will be <whiteMove> <blackMove> <moveNr + 1>
            string[] moves = line.Split(". ");

            //skip over first string, as it only contains "1"
            for(int i = 1; i < moves.Length; i++)
            {
                string[] move = moves[i].Split(" ");
                Board copy = board.getCopy();
                boards.Add(copy);

                Move whiteMove = NotationConverter.toMove(move[0], board);
                //Console.WriteLine("White: " + move[0] + ", " + whiteMove);
                board.makeMove(whiteMove);
                // board.display();
                if (move.Length == 3)
                {
                    Board copy2 = board.getCopy();
                    boards.Add(copy2);
                
                    Move blackMove = NotationConverter.toMove(move[1], board);
                    //Console.WriteLine("Black: " + move[1] + ", " + blackMove);
                    board.makeMove(blackMove);
                    // board.display();
                }
            }

            return boards;
        }
    }
}
