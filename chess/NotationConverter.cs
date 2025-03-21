using chess;

namespace converter
{
    /// <summary>
    /// Class used to convert different types of thess notations
    /// </summary>
    public static class NotationConverter
    {
        /// <summary>
        /// Converts algebraic notation to a move object
        /// </summary>
        /// <param name="move">The string containing the move</param>
        /// <param name="board">The board on which the move was made</param>
        /// <returns></returns>
        public static Move toMove(string move, Board board)
        {
            move = move.Replace("#", "");
            move = move.Replace("+", "");
            move = move.Replace("?", "");
            move = move.Replace("!", "");
            //castling
            if (move == "O-O")
            {
                int fr = board.whiteToMove ? 4 : 60;
                int to = board.whiteToMove ? 6 : 62;
                return new Move(fr, to, Move.FLAG_CASTLING);
            }

            if (move == "O-O-O")
            {
                int fr = board.whiteToMove ? 4 : 60;
                int to = board.whiteToMove ? 2 : 58;
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
            string[] split = move.Split("=");
            string pos = split[0];
            string promotion = split[1].ToLower()[0].ToString();

            //get last two characters from pos
            string toString = pos.Substring(pos.Length - 2, 2);
            int to = toIndex(toString);

            int fr;

            //no information about from was provided
            if (move.Length == 4)
            {
                fr = board.whiteToMove ? to - 8 : to + 8;
            }
            else
            {
                string file = move[0].ToString();
                string rank = board.whiteToMove ? "7" : "2";
                fr = toIndex(file + rank);
            }

            int flag = Move.PROMOTION_VALUES[promotion];
            return new Move(fr, to, flag);
        }

        private static Move toMovePawn(string move, Board board)
        {
            int to = toIndex(move);

            int offset = board.whiteToMove ? -8 : 8;
            int doublePushOffset = board.whiteToMove ? -16 : 16;
            if (Piece.isItsTurn(board.getPiece(to + offset), board.whiteToMove)) return new Move(to + offset, to);

            int toRank = chess.Index.GetRank(to);
            if (toRank == 3 || toRank == 4)
            {
                int fr = to + doublePushOffset;
                if (Piece.isItsTurn(board.getPiece(fr), board.whiteToMove)) return new Move(fr, to);
            }

            throw new Exception($"notation invalid! ({move})");
        }

        private static Move toMovePawnCapture(string move, Board board)
        {
            int to = toIndex(move.Substring(move.Length - 2, 2));

            string file = move[0].ToString();

            int toRank = chess.Index.GetRank(to);
            int rank = board.whiteToMove ? toRank : toRank + 2;

            int fr = toIndex(file + rank.ToString());

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

            foreach (Move mv in moves)
            {
                if (mv.to != to)
                {
                    continue;
                }
                if (board.getPiece(mv.fr) != piece)
                {
                    continue;
                }

                int frFile = chess.Index.GetFile(mv.fr);
                int frRank = chess.Index.GetRank(mv.fr);

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

            throw new Exception($"notation invalid! ({move})");
        }

        /// <summary>
        /// Converts a move to the algebraic notation
        /// </summary>
        /// <param name="move">The move to convert</param>
        /// <param name="board">The board on which the move is made</param>
        /// <returns>The algebraic notation of the move</returns>
        public static string toAlgebraic(Move move, Board board)
        {
            //castling
            if (move.flag == Move.FLAG_CASTLING)
            {
                if (chess.Index.GetFile(move.to) == 2) return "O-O-O";
                if (chess.Index.GetFile(move.to) == 6) return "O-O";
            }

            string algebraic = "";
            int piece = board.getPiece(move.fr);

            //add the moved piece, or blank if it was a pawn
            if (!Piece.isPawn(piece))
            {
                algebraic += Piece.DISPLAY[piece].ToUpper();
            }

            //check if there is any ambiguity
            List<Move> possibleMoves = MoveGenerator.generateAllMoves(board);
            List<Move> disambiguous = new List<Move>();
            foreach (Move possibleMove in possibleMoves)
            {
                //skip the same move as the move made
                if (possibleMove.fr == move.fr) continue;

                //if the same destination is possible with the same type of piece it is ambiguous
                if (possibleMove.to == move.to && board.getPiece(possibleMove.fr) == piece) disambiguous.Add(possibleMove);
            }

            //add disambigous file and/or rank
            if (disambiguous.Count != 0 && !Piece.isPawn(piece))
            {
                bool sameFile = disambiguous.Any(mv => chess.Index.GetFile(mv.fr) == chess.Index.GetFile(move.fr));
                bool sameRank = disambiguous.Any(mv => chess.Index.GetRank(mv.fr) == chess.Index.GetRank(move.fr));

                string coordinate = toCoordinates(move.fr);
                if (!sameFile) algebraic += coordinate[0];
                else if (!sameRank) algebraic += coordinate[1];
                else algebraic += coordinate;
            }

            //add x to move if it was a capture
            if (board.getPiece(move.to) != Piece.EMPTY || (Piece.isPawn(piece) && move.to == board.enpassantIndex))
            {
                if (Piece.isPawn(piece)) algebraic += toCoordinates(move.fr)[0];

                algebraic += "x";
            }

            //add destination
            algebraic += toCoordinates(move.to);

            //add promotion
            if ((piece == Piece.WHITE_PAWN && chess.Index.GetRank(move.to) == 7)
                || (piece == Piece.WHITE_PAWN && chess.Index.GetRank(move.to) == 0))
            {
                algebraic += "=" + Move.PROMOTION_CHARS[move.flag];
            }

            //add check and mate
            Board result = board.makeMove(move);
            if (result.isInMate()) algebraic += "#";
            else if (result.isInCheck()) algebraic += "+";



            return algebraic;
        }

        /// <summary>
        /// Converts a coordinate (eg. b2) to an index
        /// </summary>
        /// <param name="coordinates">The coordinates to convert</param>
        /// <returns>The index of the coordinates</returns>
        public static int toIndex(string coordinates)
        {
            int rank = coordinates[0] - 'a';
            int file = int.Parse(coordinates[1].ToString()) - 1;
            return rank + 8 * file;
        }

        /// <summary>
        /// Converts an index to a coordinate (eg. b2)
        /// </summary>
        /// <param name="index">The index to convert</param>
        /// <returns>The converted index</returns>
        public static string toCoordinates(int index)
        {
            return (char)(chess.Index.GetFile(index) + 'a') + (chess.Index.GetRank(index) + 1).ToString();
        }
    }
}