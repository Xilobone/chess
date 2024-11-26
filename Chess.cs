//TODO: disallow castling when or trough check
//TODO: implement stalemate
using engine;

namespace chess
{
    public class Chess
    {
        public Chess()
        {
            // assigns the correct players
            int nPlayers = getNumberOfPlayers();
            IPlayer whitePlayer;
            IPlayer blackPlayer;
            switch (nPlayers)
            {
                case 0:
                    whitePlayer = new Engine(true);
                    blackPlayer = new Engine(false);
                    break;
                case 1:
                    Console.Write("Enter 'w' if you want to play as white, or 'b' if you want to play as black:");
                    string? color = Console.ReadLine();

                    if (color == "b")
                    {
                        whitePlayer = new Engine(true);
                        blackPlayer = new Player();
                    }
                    else
                    {
                        whitePlayer = new Player();
                        blackPlayer = new Engine(false);
                    }
                    break;
                case 2:
                    whitePlayer = new Player();
                    blackPlayer = new Player();
                    break;
                default:
                    whitePlayer = new Player();
                    blackPlayer = new Player();
                    break;
            }

            //gets the starting position from a fen string
            Board board;
            Console.Write("Enter the fen position to start (or leave blank for the regular starting position):");
            string? fen = Console.ReadLine();

            if (string.IsNullOrEmpty(fen))
            {
                board = Board.startPosition();
            }
            else
            {
                board = Board.fromFen(fen);
            }

            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            //runs the game
            while (!board.isInMate())
            {
                board.display();
                //Console.ReadLine();

                Move move;
                if (board.whiteToMove)
                {
                    move = whitePlayer.makeMove(board);
                }
                else
                {
                    move = blackPlayer.makeMove(board);
                }

                //Console.WriteLine(board);
                //Console.WriteLine(move);
                board.makeMove(move);
            }

            board.display();
            long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - startTime;
            Console.WriteLine("total elapsed time:" + time + "ms");
        }

        

        //gets the number of players to play with
        private int getNumberOfPlayers()
        {
            Console.Write("How many (human) players do you want to play with:");
            string? input = Console.ReadLine();

            if (input == null)
            {
                Console.WriteLine("Please enter a value");
                return getNumberOfPlayers();
            }

            int nPlayers;
            try
            {
                nPlayers = int.Parse(input);
            }
            catch (FormatException)
            {
                Console.WriteLine("Please enter a number");
                return getNumberOfPlayers();
            }

            if (nPlayers < 0)
            {
                Console.WriteLine("Playing chess with a negative amount of players, interesting idea, but for now please enter 0, 1 or 2");
                return getNumberOfPlayers();
            }

            if (nPlayers > 2)
            {
                Console.WriteLine("Whoa Mister Popular, thats a lot of friends, please choose at most 2 people");
                return getNumberOfPlayers();
            }

            return nPlayers;
        }

        static void Main(string[] args)
        {
            new Chess();
        }
    }
}