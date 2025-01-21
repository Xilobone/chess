using System.Collections.Frozen;
using chess;

namespace converter
{
    public static class NotationConverter
    {
        public static Move toMove(string move, Board board)
        {
            move = move.Replace("#", "");
            move = move.Replace("+", "");
            move = move.Replace("?", "");
            move = move.Replace("!", "");
            //castling
            if (move == "O-O")
            {
                Position fr = board.whiteToMove ? new Position(4, 0) : new Position(4, 7);
                Position to = board.whiteToMove ? new Position(6, 0) : new Position(6, 7);
                return new Move(fr, to, Move.FLAG_CASTLING);
            }

            if (move == "O-O-O")
            {
                Position fr = board.whiteToMove ? new Position(4, 0) : new Position(4, 7);
                Position to = board.whiteToMove ? new Position(2, 0) : new Position(2, 7);
                return new Move(fr, to, Move.FLAG_CASTLING);
            }

            //promotion
            if (move.Contains("=")) return toMovePromotion(move, board);

            //regular (or double pushed) pawn move
            if (move.Length == 2) return toMovePawn(move, board);

            return toMoveRegular(move, board);
        }

        private static Move toMovePromotion(string move, Board board)
        {
            // Console.WriteLine($"{move} is a promition");

            string[] split = move.Split("=");
            string pos = split[0];
            string promotion = split[1].ToLower()[0].ToString();

            // Console.WriteLine($"pos:{pos}");

            //get last two characters from pos
            string toString = pos.Substring(pos.Length - 2, 2);
            // Console.WriteLine($"toString:{toString}");
            Position to = toPosition(toString);

            // Console.WriteLine($"to:{to}");


            Position fr;

            //no information about from was provided
            if (move.Length == 4)
            {
                fr = board.whiteToMove ? to + new Position(0, -1) : to + new Position(0, 1);
            } else 
            {
                string file = move[0].ToString();
                string rank = board.whiteToMove ? "7" : "2";
                fr = toPosition(file + rank);
            }

            int flag = Move.PROMOTION_VALUES[promotion];
            return new Move(fr, to, flag);
        }

        private static Move toMovePawn(string move, Board board)
        {
            Position to = toPosition(move);

            Position offset = board.whiteToMove ? new Position(0, -1) : new Position(0, 1);
            Position doublePushOffset = board.whiteToMove ? new Position(0, -2) : new Position(0, 2);
            if (Piece.isItsTurn(board.getPiece(to + offset), board.whiteToMove)) return new Move(to + offset, to);

            if (to.y == 3 || to.y == 4)
            {
                Position fr = to + doublePushOffset;
                if (Piece.isItsTurn(board.getPiece(fr), board.whiteToMove)) return new Move(fr, to);
            }

            throw new Exception($"notation invalid! ({move})");
        }
        
        private static Move toMovePawnCapture(string move, Board board)
        {
            Position to = toPosition(move.Substring(move.Length - 2, 2));
            string file = move[0].ToString();

            int rank = board.whiteToMove ? to.y : to.y + 2;

            Position fr = toPosition(file + rank.ToString());

            return new Move(fr, to);
        }

        private static Move toMoveRegular(string move, Board board)
        {
            char pieceChar = move[0];

            //if fist char is lower case it indicated a file
            if (char.IsLower(pieceChar)) return toMovePawnCapture(move, board);

            //convert to lowercase if it is blacks turn to move
            string pieceStr = board.whiteToMove ? pieceChar.ToString() : pieceChar.ToString().ToLower();

            int piece = Piece.VALUES[pieceStr];

            int to = toIndex(move.Substring(move.Length - 2, 2));


            bool fileSpecified = move.Length >= 4 && move[1] != 'x' && char.IsLetter(move[1]);
            bool rankSpecified = move.Length >= 4 && move[1] != 'x' && char.IsDigit(move[1]);
            bool bothSpecified = move.Length >= 5 && move[1] != 'x' && char.IsLetter(move[1]) && char.IsDigit(move[2]);

            int file = -1;
            int rank = -1;

            file = move[1] - 'a';

            if (char.IsDigit(move[1])) rank = int.Parse(move[1].ToString());

            if (bothSpecified)
            {
                file = move[1] - 'a';
                rank = int.Parse(move[2].ToString()) - 1;
            }
            
            //get correct move
            List<Move> moves = MoveGenerator.generateAllMoves(board);
            if (move.Equals("Rxb8"))
            {
                board.display();
                Console.WriteLine($"to:{to}, {NotationConverter.toCoordinates(to)}");
            }
            
            foreach(Move mv in moves)
            {
                if (move.Equals("Rxb8")) Console.WriteLine(mv);

                if (mv.toIndex != to)
                {
                    continue;
                }
                if (board.getPiece(mv.frIndex) != piece)
                {   
                    continue;
                }

                int frFile = chess.Index.GetFile(mv.frIndex);
                int frRank = chess.Index.GetRank(mv.frIndex);

                if ((fileSpecified || bothSpecified) && frFile != file)
                {
                    continue;
                }
                if ((rankSpecified || bothSpecified) && frRank != rank)
                {
                    if (move.Equals("Rxb8")) Console.WriteLine("rank not equal, skipped");

                    continue;
                }

                return mv;
            }

            // board.display();
            throw new Exception($"notation invalid! ({move})");
        }

        /// <summary>
        /// Converts a coordinate (eg. b2) to a position object
        /// </summary>
        /// <param name="coordinates">The coordinate to convert</param>
        /// <returns>The converted position</returns>
        public static Position toPosition(string coordinates)
        {
            int x = coordinates[0] - 'a';
            int y = int.Parse(coordinates[1].ToString()) - 1;

            return new Position(x, y);
        }

        public static int toIndex(string coordinates)
        {
            return toPosition(coordinates).toIndex();
        }

        /// <summary>
        /// Converts a position to a coordinate (eg. b2)
        /// </summary>
        /// <param name="position">The position to convert</param>
        /// <returns>The converted position</returns>
        public static string toCoordinates(Position position)
        {
            return (char)(position.x + 'a') + (position.y + 1).ToString();
        }  

        /// <summary>
        /// Converts an index to a coordinate (eg. b2)
        /// </summary>
        /// <param name="index">The index to convert</param>
        /// <returns>The converted index</returns>
        public static string toCoordinates(int index)
        {
            return toCoordinates(Position.toPosition(index));
        }
    }
}