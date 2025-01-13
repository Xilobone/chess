using converter;

namespace chess
{
    public class Board
    {
        public const string START_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        public int[,] pieces { get; private set; } = new int[8, 8];
        public bool whiteToMove { get; set; }
        public Position? enpassantPos { get; private set; }
        public bool[] castlingOptions { get; private set; } = new bool[4];
        public int halfMoves { get; private set; }
        public int fullMoves { get; private set; }

        private List<Board> previousBoards = new List<Board>();

        private bool checkKnown;
        private bool inCheck;

        private bool mateKnown;
        private bool inMate;
        public Board makeMove(Move move)
        {
            Board result = getCopy();
            result.previousBoards.Add(this);

            int piece = getPiece(move.fr);

            if (piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN || getPiece(move.to) != Piece.EMPTY)
            {
                result.halfMoves = 0;
            }
            else
            {
                result.halfMoves++;
            }

            //move was en passant
            if ((piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN) && move.to == enpassantPos)
            {
                if (move.to.y == 2)
                {
                    result.pieces[move.to.x, move.to.y + 1] = Piece.EMPTY;
                }
                if (move.to.y == 5)
                {
                    result.pieces[move.to.x, move.to.y - 1] = Piece.EMPTY;
                }
            }

            //check if move creates an en passant option for black
            if (piece == Piece.WHITE_PAWN && move.fr.y == 1 && move.to.y == 3)
            {
                result.enpassantPos = move.fr + new Position(0, 1);
                //check if move creates an en passant option for white
            }
            else if (piece == Piece.BLACK_PAWN && move.fr.y == 6 && move.to.y == 4)
            {
                result.enpassantPos = move.fr + new Position(0, -1);
            }
            else
            {
                result.enpassantPos = null;
            }

            result.checkCastlingOptions(move);

            if (move.flag == Move.FLAG_CASTLING)
            {
                result.castle(move);
            }

            //move pieces around
            result.pieces[move.to.x, move.to.y] = pieces[move.fr.x, move.fr.y];
            result.pieces[move.fr.x, move.fr.y] = Piece.EMPTY;

            if (Move.FLAG_PROMOTIONS.Contains(move.flag))
            {
                result.promote(move);
            }

            result.whiteToMove = !whiteToMove;

            if (whiteToMove)
            {
                result.fullMoves++;
            }

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
                if (move.fr == new Position(0, 0)) castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] = false;
                if (move.fr == new Position(7, 0)) castlingOptions[Move.CASTLE_WHITE_KINGSIDE] = false;
            }

            if (piece == Piece.BLACK_ROOK)
            {
                if (move.fr == new Position(0, 0)) castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] = false;
                if (move.fr == new Position(7, 0)) castlingOptions[Move.CASTLE_BLACK_KINGSIDE] = false;
            }

            //check if rook has been captured
            if (move.to == new Position(0, 0)) castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] = false;
            if (move.to == new Position(7, 0)) castlingOptions[Move.CASTLE_WHITE_KINGSIDE] = false;
            if (move.to == new Position(0, 7)) castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] = false;
            if (move.to == new Position(7, 7)) castlingOptions[Move.CASTLE_BLACK_KINGSIDE] = false;
        }

        private void castle(Move move)
        {
            if (move.to == new Position(2, 0))
            {
                pieces[3, 0] = pieces[0, 0];
                pieces[0, 0] = Piece.EMPTY;
            }
            else if (move.to == new Position(6, 0))
            {
                pieces[5, 0] = pieces[7, 0];
                pieces[7, 0] = Piece.EMPTY;
            }
            else if (move.to == new Position(2, 7))
            {
                pieces[3, 7] = pieces[0, 7];
                pieces[0, 7] = Piece.EMPTY;
            }
            else if (move.to == new Position(6, 7))
            {
                pieces[5, 7] = pieces[7, 7];
                pieces[7, 7] = Piece.EMPTY;
            }
        }

        private void promote(Move move)
        {
            int piece = getPiece(move.to);
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

            pieces[move.to.x, move.to.y] = promotedPiece;
        }

        public int getPiece(Position position)
        {
            return pieces[position.x, position.y];
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
            // get the kings position
            Position? kingPos = null;
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Position pos = new Position(x, y);
                    int piece = getPiece(pos);

                    if ((piece == Piece.WHITE_KING || piece == Piece.BLACK_KING) && Piece.isItsTurn(piece, whiteToMove))
                    {
                        kingPos = pos;
                    }
                }
            }

            //create a copy of the board and let it be the other players turn
            Board boardCopy = getCopy();
            boardCopy.whiteToMove = !boardCopy.whiteToMove;

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Position pos = new Position(x, y);
                    int piece = boardCopy.getPiece(pos);

                    if (!Piece.isItsTurn(piece, boardCopy.whiteToMove))
                    {
                        continue;
                    }

                    List<Move> moves = MoveGenerator.generateMoves(boardCopy, pos, true);

                    Move kingCaptureMove = new Move(pos, kingPos!);
                    if (moves.Contains(kingCaptureMove))
                    {
                        return true;
                    }
                }
            }

            return false;

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
                    Console.Write(" " + Piece.DISPLAY[pieces[x, y]] + " |");
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
                    if (pieces[x, y] == Piece.EMPTY)
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

                        fen += Piece.DISPLAY[pieces[x, y]];
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

            if (enpassantPos == null)
            {
                fen += "-";
            }
            else
            {
                fen += NotationConverter.toCoordinates(enpassantPos);
            }

            //add move counters

            fen += " " + halfMoves + " " + fullMoves;

            return fen;
        }

        public static Board fromFen(string fenString)
        {
            string[] fen = fenString.Split(' ');

            Board board = new Board();
            board.pieces = new int[8, 8];

            //setup pieces
            int x = 0;
            int y = 7;

            foreach (char c in fen[0])
            {
                string i = c.ToString();
                int digit;

                if (Piece.VALUES.Keys.Contains(i))
                {
                    board.pieces[x, y] = Piece.VALUES[i];
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
            board.enpassantPos = fen[3] == "-" ? null : NotationConverter.toPosition(fen[3]);

            board.halfMoves = int.Parse(fen[4]);
            board.fullMoves = int.Parse(fen[5]);

            return board;
        }

        public Board getCopy()
        {
            Board copy = new Board();

            copy.pieces = new int[8, 8];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    copy.pieces[x, y] = pieces[x, y];
                }
            }

            copy.whiteToMove = whiteToMove;

            copy.enpassantPos = enpassantPos != null ? new Position(enpassantPos.x, enpassantPos.y) : null;

            copy.castlingOptions = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                copy.castlingOptions[i] = castlingOptions[i];
            }

            copy.halfMoves = halfMoves;
            copy.fullMoves = fullMoves;

            copy.previousBoards = new List<Board>(previousBoards);

            return copy;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            if (ReferenceEquals(this, obj)) return true;

            if (obj is not Board) return false;

            Board other = (Board)obj;

            //check if pieces are the same
            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    if (pieces[x, y] != other.pieces[x, y]) return false;
                }
            }

            //check white to move
            if (whiteToMove != other.whiteToMove) return false;

            //check en passant
            if (enpassantPos is null && other.enpassantPos is not null) return false;

            if (enpassantPos is not null && other.enpassantPos is not null)
            {
                if (enpassantPos.Equals(other.enpassantPos)) return false;
            }

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

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    hash += (17 * pieces[x, y]) + (19 * x) + (23 * y);
                }
            }

            if (whiteToMove) hash += 29;

            if (enpassantPos is not null)
            {
                hash += 31 * enpassantPos.x;
                hash += 37 * enpassantPos.y;
            }

            for (int i = 0; i < castlingOptions.Length; i++)
            {
                if (castlingOptions[i]) hash += 987;
            }

            return hash;
        }
    }
}