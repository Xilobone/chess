using converter;

namespace chess
{
    public class Board
    {
        public const string START_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public int[] pieces { get; private set; } = new int[64];
        public bool whiteToMove { get; set; }
        public int enpassantIndex { get; private set; }
        public bool[] castlingOptions { get; private set; } = new bool[4];
        public int halfMoves { get; private set; }
        public int fullMoves { get; private set; }

        private List<Board> previousBoards = new List<Board>();

        private bool checkKnown;
        private bool inCheck;
        private bool mateKnown;
        private bool inMate;

        //bitmaps should be read from the least significant bit to the most,
        //they represent board positions from the top left to the bottom right
        public long attackMapWhite;

        public ulong[] bitboardsWhite = new ulong[6];
        public ulong[] bitboardsWhiteAttack = new ulong[6];
        public ulong[] bitboardsBlack = new ulong[6];
        public ulong[] bitboardsBlackAttack = new ulong[6];


        public Board makeMove(Move move)
        {
            Board result = getCopy();
            result.previousBoards.Add(this);

            int piece = getPiece(move.frIndex);
            int frRank = Index.GetRank(move.frIndex);
            int toRank = Index.GetRank(move.toIndex);
            if (piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN || getPiece(move.toIndex) != Piece.EMPTY)
            {
                result.halfMoves = 0;
            }
            else
            {
                result.halfMoves++;
            }

            //move was en passant
            if ((piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN) && move.toIndex == enpassantIndex)
            {
                if (toRank == 2)
                {
                    result.pieces[move.toIndex + 8] = Piece.EMPTY;
                }
                if (toRank == 5)
                {
                    result.pieces[move.toIndex - 8] = Piece.EMPTY;
                }
            }

            //check if move creates an en passant option for black
            if (piece == Piece.WHITE_PAWN && frRank == 1 && toRank == 3)
            {
                result.enpassantIndex = move.frIndex + 8;
                //check if move creates an en passant option for white
            }
            else if (piece == Piece.BLACK_PAWN && frRank == 6 && toRank == 4)
            {
                result.enpassantIndex = move.frIndex - 8;
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
            result.pieces[move.toIndex] = pieces[move.frIndex];
            result.pieces[move.frIndex] = Piece.EMPTY;

            if (Move.FLAG_PROMOTIONS.Contains(move.flag))
            {
                result.promote(move);
            }

            result.whiteToMove = !whiteToMove;

            if (whiteToMove)
            {
                result.fullMoves++;
            }

            /*TODO: only the affected pieces bitboards have to be updated, this includes
                - bitboard of the moved piece
                - if captute, bitboard of the captured piece
                - if castling, botboard of the rooks
                - if promoted, bitboard of the promoted to piece
            */
            //update bitboards
            result.bitboardsWhite = BitBoard.ComputeAll(result, true);
            result.bitboardsBlack = BitBoard.ComputeAll(result, false);

            result.bitboardsWhiteAttack = BitBoard.ComputeAllAttack(result, true);
            result.bitboardsBlackAttack = BitBoard.ComputeAllAttack(result, false);


            return result;
        }

        private void checkCastlingOptions(Move move)
        {
            int piece = getPiece(move.frIndex);

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
                if (move.frIndex == 0) castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] = false;
                if (move.frIndex == 7) castlingOptions[Move.CASTLE_WHITE_KINGSIDE] = false;
            }

            if (piece == Piece.BLACK_ROOK)
            {
                if (move.frIndex == 56) castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] = false;
                if (move.frIndex == 63) castlingOptions[Move.CASTLE_BLACK_KINGSIDE] = false;
            }

            //check if rook has been captured
            if (move.toIndex == 0) castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] = false;
            if (move.toIndex == 7) castlingOptions[Move.CASTLE_WHITE_KINGSIDE] = false;
            if (move.toIndex == 56) castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] = false;
            if (move.toIndex == 63) castlingOptions[Move.CASTLE_BLACK_KINGSIDE] = false;
        }

        private void castle(Move move)
        {
            if (move.toIndex == 2)
            {
                pieces[3] = pieces[0];
                pieces[0] = Piece.EMPTY;
            }
            else if (move.toIndex == 6)
            {
                pieces[5] = pieces[7];
                pieces[7] = Piece.EMPTY;
            }
            else if (move.toIndex == 58)
            {
                pieces[59] = pieces[56];
                pieces[56] = Piece.EMPTY;
            }
            else if (move.toIndex == 62)
            {
                pieces[61] = pieces[63];
                pieces[63] = Piece.EMPTY;
            }
        }

        private void promote(Move move)
        {
            int piece = getPiece(move.toIndex);
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

            pieces[move.toIndex] = promotedPiece;
        }

        public int getPiece(Position position)
        {
            return pieces[position.toIndex()];
        }

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

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Position pos = new Position(x, y);
                    int piece = getPiece(pos);

                    if (!Piece.isItsTurn(piece, whiteToMove))
                    {
                        continue;
                    }

                    List<Move> moves = MoveGenerator.generateMoves(this, pos, false);

                    if (moves.Count > 0)
                    {
                        return false;
                    }
                }
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

            foreach (Board board in previousBoards)
            {
                if (this.Equals(board)) sameBoards++;
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
                    int index = new Position(x, y).toIndex();
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
            Console.WriteLine($"previous boards:{previousBoards.Count}");
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

        public string toFen()
        {
            string fen = "";

            //add pieces
            for (int y = 7; y >= 0; y--)
            {
                int emptySquares = 0;

                for (int x = 0; x < 8; x++)
                {
                    int index = new Position(x, y).toIndex();
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
                    int index = new Position(x, y).toIndex();
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

        public Board getCopy()
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

            copy.previousBoards = new List<Board>(previousBoards);

            for (int i = 0; i < bitboardsWhite.Length; i++)
            {
                copy.bitboardsWhite[i] = bitboardsWhite[i];
                copy.bitboardsBlack[i] = bitboardsBlack[i];

                copy.bitboardsWhiteAttack[i] = bitboardsWhiteAttack[i];
                copy.bitboardsBlackAttack[i] = bitboardsBlackAttack[i];
            }
            return copy;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj is not Board) return false;

            Board other = (Board)obj;

            //check if pieces are the same
            for (int i = 0; i < 64; i++)
            {
                if (pieces[i] != other.pieces[i]) return false;
            }

            //check white to move
            if (whiteToMove != other.whiteToMove) return false;

            //check en passant
            if (enpassantIndex != other.enpassantIndex) return false;

            //check castling options
            for (int i = 0; i < castlingOptions.Length; i++)
            {
                if (castlingOptions[i] != other.castlingOptions[i]) return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            for (int i = 0; i < 8; i++)
            {
                hash += (17 * pieces[i]) + (19 * i);
            }


            if (whiteToMove) hash += 29;

            if (enpassantIndex != -1)
            {
                hash += 31 * enpassantIndex;
            }

            for (int i = 0; i < castlingOptions.Length; i++)
            {
                if (castlingOptions[i]) hash += 987;
            }

            return hash;
        }
    }
}