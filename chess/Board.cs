using converter;

namespace chess
{   
    /// <summary>
    /// Class that represents a state of the chess board, boards are immutable
    /// </summary>
    public class Board
    {   
        /// <summary>
        /// The fen string of the regular start position of a game of chess
        /// </summary>
        public const string START_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /// <summary>
        /// The array of pieces on the board
        /// </summary>
        public int[] pieces { get; private set; } = new int[64];

        /// <summary>
        /// True if the next player to move is white, false if the next player to move is black
        /// </summary>
        public bool whiteToMove { get; set; }

        /// <summary>
        /// The index on which en passant can be played, or -1 if en passant cannot be played
        /// </summary>
        public int enpassantIndex { get; private set; }

        /// <summary>
        /// Array indicating which castling options are still possible in order KQkq
        /// </summary>
        public bool[] castlingOptions { get; private set; } = new bool[4];

        /// <summary>
        /// Number of half moves since last capture or pawn advance
        /// </summary>
        public int halfMoves { get; private set; }

        /// <summary>
        /// Number of full moves
        /// </summary>
        public int fullMoves { get; private set; }

        private List<ulong> previousBoards = new List<ulong>();

        private bool checkKnown;
        private bool inCheck;
        private bool mateKnown;
        private bool inMate;

        //bitmaps should be read from the least significant bit to the most,
        //they represent board positions from the top left to the bottom right

        public ulong[] bitboardsWhite = new ulong[6];
        public ulong[] bitboardsWhiteAttack = new ulong[6];
        public ulong[] bitboardsBlack = new ulong[6];
        public ulong[] bitboardsBlackAttack = new ulong[6];

        /// <summary>
        /// Makes the move on the board and returns a new board with the updated state, does
        /// not alter the original board
        /// </summary>
        /// <param name="move">The move to make on the board</param>
        /// <returns>A new board with the updated state</returns>
        public Board makeMove(Move move)
        {
            Board result = getCopy();
            result.previousBoards.Add(Zobrist.hash(this));

            int piece = getPiece(move.fr);
            int frRank = Index.GetRank(move.fr);
            int toRank = Index.GetRank(move.to);

            if (piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN || getPiece(move.to) != Piece.EMPTY)
            {
                result.halfMoves = 0;
            }
            else
            {
                result.halfMoves++;
            }

            bool wasEnpassant = false;
            //move was en passant
            if ((piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN) && move.to == enpassantIndex)
            {
                wasEnpassant = true;
                if (toRank == 2)
                {
                    result.pieces[move.to + 8] = Piece.EMPTY;
                }
                if (toRank == 5)
                {
                    result.pieces[move.to - 8] = Piece.EMPTY;
                }
            }

            //check if move creates an en passant option for black
            if (piece == Piece.WHITE_PAWN && frRank == 1 && toRank == 3)
            {
                result.enpassantIndex = move.fr + 8;
                //check if move creates an en passant option for white
            }
            else if (piece == Piece.BLACK_PAWN && frRank == 6 && toRank == 4)
            {
                result.enpassantIndex = move.fr - 8;
            }
            else
            {
                result.enpassantIndex = -1;
            }

            result.checkCastlingOptions(move);

            if (move.flag == Move.FLAG_CASTLING)
            {
                result.castle(move);
            }

            //move pieces around
            result.pieces[move.to] = pieces[move.fr];
            result.pieces[move.fr] = Piece.EMPTY;

            if (Move.FLAG_PROMOTIONS.Contains(move.flag))
            {
                result.promote(move);
            }

            result.whiteToMove = !whiteToMove;

            if (whiteToMove)
            {
                result.fullMoves++;
            }

            //updates affected bitboards
            switch (piece)
            {
                case Piece.WHITE_PAWN: result.bitboardsWhite[BitBoard.PAWN] = BitBoard.Compute(result, BitBoard.PAWN, true); break;
                case Piece.WHITE_KNIGHT: result.bitboardsWhite[BitBoard.KNIGHT] = BitBoard.Compute(result, BitBoard.KNIGHT, true); break;
                case Piece.WHITE_BISHOP: result.bitboardsWhite[BitBoard.BISHOP] = BitBoard.Compute(result, BitBoard.BISHOP, true); break;
                case Piece.WHITE_ROOK: result.bitboardsWhite[BitBoard.ROOK] = BitBoard.Compute(result, BitBoard.ROOK, true); break;
                case Piece.WHITE_QUEEN: result.bitboardsWhite[BitBoard.QUEEN] = BitBoard.Compute(result, BitBoard.QUEEN, true); break;
                case Piece.WHITE_KING: result.bitboardsWhite[BitBoard.KING] = BitBoard.Compute(result, BitBoard.KING, true); break;
                case Piece.BLACK_PAWN: result.bitboardsBlack[BitBoard.PAWN] = BitBoard.Compute(result, BitBoard.PAWN, false); break;
                case Piece.BLACK_KNIGHT: result.bitboardsBlack[BitBoard.KNIGHT] = BitBoard.Compute(result, BitBoard.KNIGHT, false); break;
                case Piece.BLACK_BISHOP: result.bitboardsBlack[BitBoard.BISHOP] = BitBoard.Compute(result, BitBoard.BISHOP, false); break;
                case Piece.BLACK_ROOK: result.bitboardsBlack[BitBoard.ROOK] = BitBoard.Compute(result, BitBoard.ROOK, false); break;
                case Piece.BLACK_QUEEN: result.bitboardsBlack[BitBoard.QUEEN] = BitBoard.Compute(result, BitBoard.QUEEN, false); break;
                case Piece.BLACK_KING: result.bitboardsBlack[BitBoard.KING] = BitBoard.Compute(result, BitBoard.KING, false); break;
            }

            int capturedPiece = getPiece(move.to);
            switch (capturedPiece)
            {
                case Piece.WHITE_PAWN: result.bitboardsWhite[BitBoard.PAWN] = BitBoard.Compute(result, BitBoard.PAWN, true); break;
                case Piece.WHITE_KNIGHT: result.bitboardsWhite[BitBoard.KNIGHT] = BitBoard.Compute(result, BitBoard.KNIGHT, true); break;
                case Piece.WHITE_BISHOP: result.bitboardsWhite[BitBoard.BISHOP] = BitBoard.Compute(result, BitBoard.BISHOP, true); break;
                case Piece.WHITE_ROOK: result.bitboardsWhite[BitBoard.ROOK] = BitBoard.Compute(result, BitBoard.ROOK, true); break;
                case Piece.WHITE_QUEEN: result.bitboardsWhite[BitBoard.QUEEN] = BitBoard.Compute(result, BitBoard.QUEEN, true); break;
                case Piece.WHITE_KING: result.bitboardsWhite[BitBoard.KING] = BitBoard.Compute(result, BitBoard.KING, true); break;
                case Piece.BLACK_PAWN: result.bitboardsBlack[BitBoard.PAWN] = BitBoard.Compute(result, BitBoard.PAWN, false); break;
                case Piece.BLACK_KNIGHT: result.bitboardsBlack[BitBoard.KNIGHT] = BitBoard.Compute(result, BitBoard.KNIGHT, false); break;
                case Piece.BLACK_BISHOP: result.bitboardsBlack[BitBoard.BISHOP] = BitBoard.Compute(result, BitBoard.BISHOP, false); break;
                case Piece.BLACK_ROOK: result.bitboardsBlack[BitBoard.ROOK] = BitBoard.Compute(result, BitBoard.ROOK, false); break;
                case Piece.BLACK_QUEEN: result.bitboardsBlack[BitBoard.QUEEN] = BitBoard.Compute(result, BitBoard.QUEEN, false); break;
                case Piece.BLACK_KING: result.bitboardsBlack[BitBoard.KING] = BitBoard.Compute(result, BitBoard.KING, false); break;
            }

            //update rooks bitboard if castled
            if (move.flag == Move.FLAG_CASTLING)
            {
                if (Piece.isWhite(piece)) result.bitboardsWhite[BitBoard.ROOK] = BitBoard.Compute(result, BitBoard.ROOK, true);
                else result.bitboardsBlack[BitBoard.ROOK] = BitBoard.Compute(result, BitBoard.ROOK, false);
            }

            if (Piece.isWhite(piece))
            {
                switch (move.flag)
                {
                    case Move.FLAG_PROMOTE_KNIGHT: result.bitboardsWhite[BitBoard.KNIGHT] = BitBoard.Compute(result, BitBoard.KNIGHT, true); break;
                    case Move.FLAG_PROMOTE_BISHOP: result.bitboardsWhite[BitBoard.BISHOP] = BitBoard.Compute(result, BitBoard.BISHOP, true); break;
                    case Move.FLAG_PROMOTE_ROOK: result.bitboardsWhite[BitBoard.ROOK] = BitBoard.Compute(result, BitBoard.ROOK, true); break;
                    case Move.FLAG_PROMOTE_QUEEN: result.bitboardsWhite[BitBoard.QUEEN] = BitBoard.Compute(result, BitBoard.QUEEN, true); break;
                }
            }
            else
            {
                switch (move.flag)
                {
                    case Move.FLAG_PROMOTE_KNIGHT: result.bitboardsBlack[BitBoard.KNIGHT] = BitBoard.Compute(result, BitBoard.KNIGHT, false); break;
                    case Move.FLAG_PROMOTE_BISHOP: result.bitboardsBlack[BitBoard.BISHOP] = BitBoard.Compute(result, BitBoard.BISHOP, false); break;
                    case Move.FLAG_PROMOTE_ROOK: result.bitboardsBlack[BitBoard.ROOK] = BitBoard.Compute(result, BitBoard.ROOK, false); break;
                    case Move.FLAG_PROMOTE_QUEEN: result.bitboardsBlack[BitBoard.QUEEN] = BitBoard.Compute(result, BitBoard.QUEEN, false); break;
                }
            }

            if (wasEnpassant)
            {
                if (Piece.isWhite(piece)) result.bitboardsBlack[BitBoard.PAWN] = BitBoard.Compute(result, BitBoard.PAWN, false);
                else result.bitboardsWhite[BitBoard.PAWN] = BitBoard.Compute(result, BitBoard.PAWN, true);
            }

            result.bitboardsWhiteAttack = BitBoard.ComputeAllAttack(result, true);
            result.bitboardsBlackAttack = BitBoard.ComputeAllAttack(result, false);


            return result;
        }

        private void checkCastlingOptions(Move move)
        {
            int piece = getPiece(move.fr);

            //check if king has moved
            if (piece == Piece.WHITE_KING)
            {
                castlingOptions[Move.CASTLE_WHITE_KINGSIDE] = false;
                castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] = false;
            }

            if (piece == Piece.BLACK_KING)
            {
                castlingOptions[Move.CASTLE_BLACK_KINGSIDE] = false;
                castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] = false;
            }

            //check if rook has moved
            if (piece == Piece.WHITE_ROOK)
            {
                if (move.fr == 0) castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] = false;
                if (move.fr == 7) castlingOptions[Move.CASTLE_WHITE_KINGSIDE] = false;
            }

            if (piece == Piece.BLACK_ROOK)
            {
                if (move.fr == 56) castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] = false;
                if (move.fr == 63) castlingOptions[Move.CASTLE_BLACK_KINGSIDE] = false;
            }

            //check if rook has been captured
            if (move.to == 0) castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] = false;
            if (move.to == 7) castlingOptions[Move.CASTLE_WHITE_KINGSIDE] = false;
            if (move.to == 56) castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] = false;
            if (move.to == 63) castlingOptions[Move.CASTLE_BLACK_KINGSIDE] = false;
        }

        private void castle(Move move)
        {
            if (move.to == 2)
            {
                pieces[3] = pieces[0];
                pieces[0] = Piece.EMPTY;
            }
            else if (move.to == 6)
            {
                pieces[5] = pieces[7];
                pieces[7] = Piece.EMPTY;
            }
            else if (move.to == 58)
            {
                pieces[59] = pieces[56];
                pieces[56] = Piece.EMPTY;
            }
            else if (move.to == 62)
            {
                pieces[61] = pieces[63];
                pieces[63] = Piece.EMPTY;
            }
        }

        private void promote(Move move)
        {
            int piece = getPiece(move.fr);

            int promotedPiece = -1;
            if (Piece.isWhite(piece))
            {
                switch (move.flag)
                {
                    case Move.FLAG_PROMOTE_QUEEN: promotedPiece = Piece.WHITE_QUEEN; break;
                    case Move.FLAG_PROMOTE_ROOK: promotedPiece = Piece.WHITE_ROOK; break;
                    case Move.FLAG_PROMOTE_BISHOP: promotedPiece = Piece.WHITE_BISHOP; break;
                    case Move.FLAG_PROMOTE_KNIGHT: promotedPiece = Piece.WHITE_KNIGHT; break;
                }
            }
            else
            {
                switch (move.flag)
                {
                    case Move.FLAG_PROMOTE_QUEEN: promotedPiece = Piece.BLACK_QUEEN; break;
                    case Move.FLAG_PROMOTE_ROOK: promotedPiece = Piece.BLACK_ROOK; break;
                    case Move.FLAG_PROMOTE_BISHOP: promotedPiece = Piece.BLACK_BISHOP; break;
                    case Move.FLAG_PROMOTE_KNIGHT: promotedPiece = Piece.BLACK_KNIGHT; break;
                }
            }

            pieces[move.to] = promotedPiece;
        }

        // public int getPiece(Position position)
        // {
        //     return pieces[position.toIndex()];
        // }

        /// <summary>
        /// Gets the piece that is at the given index, board is indexed left to right, top to bottom
        /// </summary>
        /// <param name="index">The index of which to get the piece from</param>
        /// <returns>The piece at the index</returns>
        public int getPiece(int index)
        {
            if (((bitboardsWhite[BitBoard.PAWN] >> index) & 1) == 1) return Piece.WHITE_PAWN;
            if (((bitboardsWhite[BitBoard.KNIGHT] >> index) & 1) == 1) return Piece.WHITE_KNIGHT;
            if (((bitboardsWhite[BitBoard.BISHOP] >> index) & 1) == 1) return Piece.WHITE_BISHOP;
            if (((bitboardsWhite[BitBoard.ROOK] >> index) & 1) == 1) return Piece.WHITE_ROOK;
            if (((bitboardsWhite[BitBoard.QUEEN] >> index) & 1) == 1) return Piece.WHITE_QUEEN;
            if (((bitboardsWhite[BitBoard.KING] >> index) & 1) == 1) return Piece.WHITE_KING;

            if (((bitboardsBlack[BitBoard.PAWN] >> index) & 1) == 1) return Piece.BLACK_PAWN;
            if (((bitboardsBlack[BitBoard.KNIGHT] >> index) & 1) == 1) return Piece.BLACK_KNIGHT;
            if (((bitboardsBlack[BitBoard.BISHOP] >> index) & 1) == 1) return Piece.BLACK_BISHOP;
            if (((bitboardsBlack[BitBoard.ROOK] >> index) & 1) == 1) return Piece.BLACK_ROOK;
            if (((bitboardsBlack[BitBoard.QUEEN] >> index) & 1) == 1) return Piece.BLACK_QUEEN;
            if (((bitboardsBlack[BitBoard.KING] >> index) & 1) == 1) return Piece.BLACK_KING;

            return 0;
        }

        /// <summary>
        /// Checks if this board is in check
        /// </summary>
        /// <returns>True if this board is in check, false otherwise</returns>
        public bool isInCheck()
        {
            if (checkKnown) return inCheck;

            checkKnown = true;
            inCheck = getInCheck();

            return inCheck;
        }
        private bool getInCheck()
        {
            return whiteToMove ?
                (bitboardsWhite[BitBoard.KING] & BitBoard.GetAnyAttack(this, false)) != 0 :
                (bitboardsBlack[BitBoard.KING] & BitBoard.GetAnyAttack(this, true)) != 0;
        }

        /// <summary>
        /// Checks if this board is in mate
        /// </summary>
        /// <returns>True if the board is in mate, false otherwise</returns>
        public bool isInMate()
        {
            if (mateKnown) return inMate;

            mateKnown = true;
            inMate = getInMate();

            return inMate;
        }
        private bool getInMate()
        {
            if (!isInCheck())
            {
                return false;
            }

            ulong allFriendlyPieces = BitBoard.GetAny(this, whiteToMove);

            int index = 0;
            while (allFriendlyPieces != 0)
            {
                //if last bit is zero, shift and continue
                if ((allFriendlyPieces & 1) == 0)
                {
                    allFriendlyPieces >>= 1;
                    index++;
                    continue;
                }

                List<Move> moves = MoveGenerator.generateMoves(this, index, false);

                if (moves.Count > 0)
                {
                    return false;
                }

                allFriendlyPieces >>= 1;
                index++;

            }

            return true;
        }

        /// <summary>
        /// Checks if a board is a draw by repetition
        /// </summary>
        /// <returns>true if the board is a draw, false otherwise</returns>
        public bool isInDraw()
        {
            int sameBoards = 0;
            ulong hash = Zobrist.hash(this);
            foreach (ulong h in previousBoards)
            {

                if (hash == h) sameBoards++;
            }

            //a draw occurs if it is the third time this position occurs,
            //the current board is not included in previousBoards but counts as 1
            return sameBoards >= 2;
        }

        /// <summary>
        /// Shows a display of the board in the console
        /// </summary>
        public void display()
        {
            Console.WriteLine("  +---+---+---+---+---+---+---+---+");
            for (int y = 7; y >= 0; y--)
            {
                Console.Write((y + 1) + " |");
                for (int x = 0; x < 8; x++)
                {
                    int index = x + 8*y;
                    Console.Write(" " + Piece.DISPLAY[pieces[index]] + " |");
                }
                Console.WriteLine();
                Console.WriteLine("  +---+---+---+---+---+---+---+---+");
            }
            Console.Write("    ");
            for (int i = 0; i < 8; i++)
            {
                Console.Write((char)(i + 'a') + "   ");
            }

            Console.WriteLine();

            Console.WriteLine(toFen());
            Console.WriteLine($"Hash: {Zobrist.hash(this)}");
            if (isInCheck()) Console.WriteLine("Check!");
            if (isInMate())
            {
                Console.WriteLine("Mate!");
                return;
            }

            if (isInDraw())
            {
                Console.WriteLine("Draw!");
                return;
            }

            if (whiteToMove)
            {
                Console.WriteLine("White to move");
            }
            else
            {
                Console.WriteLine("Black to move");
            }
        }

        /// <summary>
        /// Creates a new board from the regular starting position
        /// </summary>
        /// <returns></returns>
        public static Board startPosition()
        {
            return fromFen(START_FEN);
        }

        /// <summary>
        /// Creates a fen string from this board
        /// </summary>
        /// <returns>The fen string of this board</returns>
        public string toFen()
        {
            string fen = "";

            //add pieces
            for (int y = 7; y >= 0; y--)
            {
                int emptySquares = 0;

                for (int x = 0; x < 8; x++)
                {
                    int index = x + 8 * y;
                    if (pieces[index] == Piece.EMPTY)
                    {
                        emptySquares++;
                    }
                    else
                    {
                        if (emptySquares > 0)
                        {
                            fen += emptySquares;
                            emptySquares = 0;
                        }

                        fen += Piece.DISPLAY[pieces[index]];
                    }
                }

                if (emptySquares > 0)
                {
                    fen += emptySquares;
                }

                fen += "/";
            }

            //remove last / from fen
            fen = fen.Substring(0, fen.Length - 1);

            fen += " ";
            fen += whiteToMove ? "w" : "b";

            //add castling
            string castle = "";

            if (castlingOptions[Move.CASTLE_WHITE_KINGSIDE]) castle += "K";
            if (castlingOptions[Move.CASTLE_WHITE_QUEENSIDE]) castle += "Q";
            if (castlingOptions[Move.CASTLE_BLACK_KINGSIDE]) castle += "k";
            if (castlingOptions[Move.CASTLE_BLACK_QUEENSIDE]) castle += "q";

            if (castle.Length == 0) castle += "-";

            fen += " " + castle + " ";

            if (enpassantIndex == -1)
            {
                fen += "-";
            }
            else
            {
                fen += NotationConverter.toCoordinates(enpassantIndex);
            }

            //add move counters

            fen += " " + halfMoves + " " + fullMoves;

            return fen;
        }

        /// <summary>
        /// Creates a board from a fen string
        /// </summary>
        /// <param name="fenString">The fen string to create a board from</param>
        /// <returns>The board fro mthe fen string</returns>
        public static Board fromFen(string fenString)
        {
            string[] fen = fenString.Split(' ');

            Board board = new Board();
            board.pieces = new int[64];

            //setup pieces
            int x = 0;
            int y = 7;

            foreach (char c in fen[0])
            {
                string i = c.ToString();
                int digit;

                if (Piece.VALUES.Keys.Contains(i))
                {
                    int index = x + 8 * y;
                    board.pieces[index] = Piece.VALUES[i];
                    x++;
                }
                else if (i == "/")
                {
                    x = 0;
                    y--;
                }
                else if (int.TryParse(i, out digit))
                {
                    x += digit;
                }
            }

            //setup white to move
            board.whiteToMove = fen[1] == "w";

            //castling
            board.castlingOptions = new bool[4];
            foreach (char i in fen[2])
            {
                switch (i)
                {
                    case 'K': board.castlingOptions[Move.CASTLE_WHITE_KINGSIDE] = true; break;
                    case 'Q': board.castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] = true; break;
                    case 'k': board.castlingOptions[Move.CASTLE_BLACK_KINGSIDE] = true; break;
                    case 'q': board.castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] = true; break;
                    case '-': break;
                }
            }

            //en passant
            board.enpassantIndex = fen[3] == "-" ? -1 : NotationConverter.toIndex(fen[3]);

            board.halfMoves = int.Parse(fen[4]);
            board.fullMoves = int.Parse(fen[5]);

            board.bitboardsWhite = BitBoard.ComputeAll(board, true);
            board.bitboardsBlack = BitBoard.ComputeAll(board, false);

            board.bitboardsWhiteAttack = BitBoard.ComputeAllAttack(board, true);
            board.bitboardsBlackAttack = BitBoard.ComputeAllAttack(board, false);
            return board;
        }

        private Board getCopy()
        {
            Board copy = new Board();

            copy.pieces = new int[64];

            for (int i = 0; i < 64; i++)
            {
                copy.pieces[i] = pieces[i];
            }

            copy.whiteToMove = whiteToMove;

            copy.enpassantIndex = enpassantIndex;

            copy.castlingOptions = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                copy.castlingOptions[i] = castlingOptions[i];
            }

            copy.halfMoves = halfMoves;
            copy.fullMoves = fullMoves;

            copy.previousBoards = new List<ulong>(previousBoards);

            for (int i = 0; i < bitboardsWhite.Length; i++)
            {
                copy.bitboardsWhite[i] = bitboardsWhite[i];
                copy.bitboardsBlack[i] = bitboardsBlack[i];

                copy.bitboardsWhiteAttack[i] = bitboardsWhiteAttack[i];
                copy.bitboardsBlackAttack[i] = bitboardsBlackAttack[i];
            }
            return copy;
        }
    }
}