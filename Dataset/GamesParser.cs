using System.IO;
using chess;
using converter;

namespace parser
{
    public class GamesParser
    {
        private string filePath;
        private IEvaluator[] evaluators;
        private Random random;
        public GamesParser(string filePath, IEvaluator[] evaluators, int seed)
        {
            this.filePath = filePath;
            this.evaluators = evaluators;

            this.random = new Random(seed);
        }

        /// <summary>
        /// Gets a list of unique boards from the file which are roughly equal
        /// </summary>
        /// <param name="listSize">The amount of boards to include in the search</param>
        /// <param name="amount">The amount of boards to save</param>
        /// <param name="range">The evaluation difference from 0 that is allowed</param>
        /// <returns></returns>
        public List<Board> parse(int listSize, int amount, float range)
        {
            HashSet<Board> boards = new HashSet<Board>();

            string[] lines = File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                //skip over empty lines of lines containing metadata
                if (string.IsNullOrEmpty(line)) continue;
                if (line[0] == '[') continue;


                List<Board> b = getAllPositions(line, range);
                foreach (Board board in b)
                {
                    boards.Add(board);
                }

                double percentage = Math.Min((double)(boards.Count * 100) / listSize, 100);
                Console.WriteLine($"Parsing pgn file ({percentage}%)");

                if (boards.Count >= listSize)
                {
                    Console.WriteLine($"done parsing, used {(double)(i * 100) / lines.Length}% of available data");
                    Console.WriteLine("--------------------");
                    break;
                }
            }

            //select random boards
            List<Board> allBoards = boards.ToList();
            List<Board> selectedBoards = new List<Board>();

            while (selectedBoards.Count < amount)
            {
                int index = random.Next(allBoards.Count);
                Board board = allBoards[index];

                selectedBoards.Add(board);
                allBoards.Remove(board);
            }

            return selectedBoards;
        }

        public List<Board> getAllPositions(string line, float range)
        {
            List<Board> boards = new List<Board>();
            Board board = Board.startPosition();
            //moves will be <whiteMove> <blackMove> <moveNr + 1>
            string[] moves = line.Split(". ");

            //skip over first string, as it only contains "1"
            for (int i = 1; i < moves.Length; i++)
            {
                string[] move = moves[i].Split(" ");

                float eval = getAverageEval(board);
                if (Math.Abs(eval) <= range && !boards.Contains(board)) boards.Add(board);


                Move whiteMove = NotationConverter.toMove(move[0], board);
                board = board.makeMove(whiteMove);
                if (move.Length == 3)
                {
                    eval = getAverageEval(board);
                    if (Math.Abs(eval) <= range && !boards.Contains(board)) boards.Add(board);

                    Move blackMove = NotationConverter.toMove(move[1], board);
                    board = board.makeMove(blackMove);
                }
            }

            return boards;
        }

        private float getAverageEval(Board board)
        {
            float eval = 0;

            foreach (IEvaluator evaluator in evaluators)
            {
                eval += evaluator.evaluate(board);
            }

            return eval / evaluators.Length;
        }
    }
}
