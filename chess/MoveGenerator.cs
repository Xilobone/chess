using counters;

namespace chess
{
    public class MoveGenerator
    {
        private static Position[] KNIGHT_OFFSETS = new Position[]
        {
        new Position(-1,-2),
        new Position(1,-2),
        new Position(-1,2),
        new Position(1,2),
        new Position(-2, -1),
        new Position(-2,1),
        new Position(2,-1),
        new Position(2,1)
        };

        private static int[] KNIGHT_OFFSETS_INDEX = new int[] {-17, -15, -10, -6, 6, 10, 15, 17 };

        private static Position[] KING_OFFSETS = new Position[]
        {
        new Position(-1,-1),
        new Position(-1,0),
        new Position(-1,1),
        new Position(0,-1),
        new Position(0, 1),
        new Position(1,-1),
        new Position(1,0),
        new Position(1,1)
        };

        private static Dictionary<int, Position> KING_CASTLE_POSITIONS = new Dictionary<int, Position>
    {
        { Move.CASTLE_WHITE_KINGSIDE, new Position(6,0) },
        { Move.CASTLE_WHITE_QUEENSIDE, new Position(2,0) },
        { Move.CASTLE_BLACK_KINGSIDE, new Position(6,7) },
        { Move.CASTLE_BLACK_QUEENSIDE, new Position(2,7) }
    };

        //positions that must be clear of any piece, and any attack from the opponent to allow check
        private static ulong[] bitboardSafeCastleWhite = [
        0b0000000000000000000000000000000000000000000000000000000001110000,
        0b0000000000000000000000000000000000000000000000000000000000011100
        ];

        private static ulong[] bitboardSafeCastleBlack = [
        0b0111000000000000000000000000000000000000000000000000000000000000,
        0b0001110000000000000000000000000000000000000000000000000000000000,
    ];



        public static List<Move> generateAllMoves(Board board)
        {
            List<Move> moves = new List<Move>();

            for (int i = 0; i < 64; i++)
            {
                moves.AddRange(generateMoves(board, i));
            }

            return moves;
        }
        public static List<Move> generateMoves(Board board, int index)
        {
            return generateMoves(board, index, false);
        }
        public static List<Move> generateMoves(Board board, int index, bool allowCheck)
        {
            Position pos = Position.toPosition(index);

            List<Move> moves = new List<Move>();

            int piece = board.getPiece(index);

            if (piece == Piece.EMPTY)
            {
                return moves;
            }

            if (Piece.isWhite(piece) && !board.whiteToMove)
            {
                return moves;
            }

            if (Piece.isBlack(piece) && board.whiteToMove)
            {
                return moves;
            }

            if (piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN)
            {
                moves.AddRange(getPawnMoves(board, index, allowCheck));
            }

            if (piece == Piece.WHITE_ROOK || piece == Piece.BLACK_ROOK)
            {
                moves.AddRange(getRookMoves(board, pos, allowCheck));
            }

            if (piece == Piece.WHITE_BISHOP || piece == Piece.BLACK_BISHOP)
            {
                moves.AddRange(getBishopMoves(board, pos, allowCheck));
            }

            if (piece == Piece.WHITE_QUEEN || piece == Piece.BLACK_QUEEN)
            {
                moves.AddRange(getQueenMoves(board, pos, allowCheck));
            }

            if (piece == Piece.WHITE_KNIGHT || piece == Piece.BLACK_KNIGHT)
            {
                moves.AddRange(getKnightMoves(board, pos, allowCheck));
            }

            if (piece == Piece.WHITE_KING || piece == Piece.BLACK_KING)
            {
                moves.AddRange(getKingMoves(board, pos, allowCheck));
            }

            return moves;
        }

        private static List<Move> getPawnMoves(Board board, int index, bool allowCheck)
        {
            List<Move> moves = new List<Move>();
            int piece = board.getPiece(index);

            int newIndex = piece == Piece.WHITE_PAWN ? index + 8 : index - 8;

            if (newIndex >= 0 && newIndex <= 63)
            {
                if (board.getPiece(newIndex) == Piece.EMPTY)
                {
                    appendMove(moves, board, index, newIndex, allowCheck);
                }
            }

            //captures
            int captureIndexLeft = piece == Piece.WHITE_PAWN ? index + 7 : index - 9;
            int captureIndexRight = piece == Piece.WHITE_PAWN ? index + 9 : index - 7;

            //first condition checks if pawn is not on a file
            if ((index % 8 != 0) && captureIndexLeft >= 0 && captureIndexLeft < 64)
            {
                if (Piece.isDifferentColor(board.getPiece(captureIndexLeft), piece) || captureIndexLeft == board.enpassantIndex)
                {
                    appendMove(moves, board, index, captureIndexLeft, allowCheck);
                }
            }

            //first condition checks if pawn is not on h file
            if (((index + 1) % 8 != 0) && captureIndexRight >= 0 && captureIndexRight < 64)
            {
                if (Piece.isDifferentColor(board.getPiece(captureIndexRight), piece) || captureIndexRight == board.enpassantIndex)
                {
                    appendMove(moves, board, index, captureIndexRight, allowCheck);
                }
            }

            //double pawn pushes (white)
            int rank = Index.GetRank(index);
            if (Piece.isWhite(piece) && rank == 1 && board.getPiece(index + 8) == Piece.EMPTY)
            {
                int doublePush = index + 16;
                if (board.getPiece(doublePush) == Piece.EMPTY)
                {
                    appendMove(moves, board, index, doublePush, allowCheck);
                }
            }

            //double pawn pushes (black)
            if (Piece.isBlack(piece) && rank == 6 && board.getPiece(index - 8) == Piece.EMPTY)
            {
                int doublePush = index - 16;
                if (board.getPiece(doublePush) == Piece.EMPTY)
                {
                    appendMove(moves, board, index, doublePush, allowCheck);
                }
            }

            return moves;
        }

        private static List<Move> getRookMoves(Board board, Position pos, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            moves.AddRange(searchDirection(board, pos, new Position(1, 0), allowCheck));
            moves.AddRange(searchDirection(board, pos, new Position(-1, 0), allowCheck));
            moves.AddRange(searchDirection(board, pos, new Position(0, 1), allowCheck));
            moves.AddRange(searchDirection(board, pos, new Position(0, -1), allowCheck));

            return moves;
        }

        private static List<Move> getBishopMoves(Board board, Position pos, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            moves.AddRange(searchDirection(board, pos, new Position(1, 1), allowCheck));
            moves.AddRange(searchDirection(board, pos, new Position(1, -1), allowCheck));
            moves.AddRange(searchDirection(board, pos, new Position(-1, 1), allowCheck));
            moves.AddRange(searchDirection(board, pos, new Position(-1, -1), allowCheck));

            return moves;
        }

        private static List<Move> getQueenMoves(Board board, Position pos, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            moves.AddRange(getRookMoves(board, pos, allowCheck));
            moves.AddRange(getBishopMoves(board, pos, allowCheck));

            return moves;
        }

        private static List<Move> getKnightMoves(Board board, Position pos, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            foreach (Position offset in KNIGHT_OFFSETS)
            {
                moves.AddRange(searchOffset(board, pos, offset, allowCheck));
            }

            return moves;
        }

        private static List<Move> getKingMoves(Board board, Position pos, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            foreach (Position offset in KING_OFFSETS)
            {
                moves.AddRange(searchOffset(board, pos, offset, allowCheck));
            }

            // Console.WriteLine($"safecastlewhite kingside: {bitboardSafeCastleWhite[0]}");
            // Console.WriteLine($"safecastlewhite kingside: {bitboardSafeCastleWhite[1]}");
            //add castling moves
            int piece = board.getPiece(pos);
            if (piece == Piece.WHITE_KING)
            {


                ulong allPiecesExceptWhiteKing = BitBoard.GetAny(board) & ~board.bitboardsWhite[BitBoard.KING];

                bool safeToCastle = (bitboardSafeCastleWhite[0] & (allPiecesExceptWhiteKing | BitBoard.GetAnyAttack(board, false))) == 0;
                if (board.castlingOptions[Move.CASTLE_WHITE_KINGSIDE] && safeToCastle)
                {
                    appendMove(moves, board, pos.toIndex(), KING_CASTLE_POSITIONS[Move.CASTLE_WHITE_KINGSIDE].toIndex(), allowCheck, Move.FLAG_CASTLING);
                }

                //last condition checks if b1 is empty
                safeToCastle = (bitboardSafeCastleWhite[1] & (allPiecesExceptWhiteKing | BitBoard.GetAnyAttack(board, false))) == 0 & (allPiecesExceptWhiteKing & (1UL << 1)) == 0;
                if (board.castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] && safeToCastle)
                {
                    appendMove(moves, board, pos.toIndex(), KING_CASTLE_POSITIONS[Move.CASTLE_WHITE_QUEENSIDE].toIndex(), allowCheck, Move.FLAG_CASTLING);
                }
            }

            if (piece == Piece.BLACK_KING)
            {
                // Console.WriteLine("checking castle options black");
                ulong allPiecesExceptBlackKing = BitBoard.GetAny(board) & ~board.bitboardsBlack[BitBoard.KING];

                bool safeToCastle = (bitboardSafeCastleBlack[0] & (allPiecesExceptBlackKing | BitBoard.GetAnyAttack(board, true))) == 0;
                // Console.WriteLine($"can castle kingside {safeToCastle}");
                if (board.castlingOptions[Move.CASTLE_BLACK_KINGSIDE] && safeToCastle)
                {
                    appendMove(moves, board, pos.toIndex(), KING_CASTLE_POSITIONS[Move.CASTLE_BLACK_KINGSIDE].toIndex(), allowCheck, Move.FLAG_CASTLING);
                }

                //last condition checks if b8 is empty
                safeToCastle = (bitboardSafeCastleBlack[1] & (allPiecesExceptBlackKing | BitBoard.GetAnyAttack(board, true))) == 0 & (allPiecesExceptBlackKing & (1UL << 57)) == 0;

                if (board.castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] && safeToCastle)
                {
                    appendMove(moves, board, pos.toIndex(), KING_CASTLE_POSITIONS[Move.CASTLE_BLACK_QUEENSIDE].toIndex(), allowCheck, Move.FLAG_CASTLING);
                }



                // if (board.castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] && board.getPiece(57) == Piece.EMPTY && board.getPiece(58) == Piece.EMPTY && board.getPiece(59) == Piece.EMPTY)
                // {
                //     appendMove(moves, board, pos, KING_CASTLE_POSITIONS[Move.CASTLE_BLACK_QUEENSIDE], allowCheck, Move.FLAG_CASTLING);
                // }
                // if (board.castlingOptions[Move.CASTLE_BLACK_KINGSIDE] && board.getPiece(61) == Piece.EMPTY && board.getPiece(62) == Piece.EMPTY)
                // {
                //     appendMove(moves, board, pos, KING_CASTLE_POSITIONS[Move.CASTLE_BLACK_KINGSIDE], allowCheck, Move.FLAG_CASTLING);
                // }
            }

            return moves;
        }

        private static List<Move> searchDirection(Board board, Position pos, Position dir, bool allowCheck)
        {
            List<Move> moves = new List<Move>();
            int piece = board.getPiece(pos);
            Position newPos = pos + dir;

            while (newPos.x >= 0 && newPos.y >= 0 && newPos.x < 8 && newPos.y < 8)
            {
                int occupiedPiece = board.getPiece(newPos);

                if (Piece.isSameColor(piece, occupiedPiece))
                {
                    break;
                }

                if (Piece.isDifferentColor(piece, occupiedPiece))
                {
                    appendMove(moves, board, pos.toIndex(), newPos.toIndex(), allowCheck);
                    break;
                }

                appendMove(moves, board, pos.toIndex(), newPos.toIndex(), allowCheck);
                newPos += dir;
            }

            return moves;
        }

        private static List<Move> searchOffset(Board board, Position pos, Position offset, bool allowCheck)
        {
            List<Move> moves = new List<Move>();
            int piece = board.getPiece(pos);

            Position newPos = pos + offset;

            //do not do anything if newpos is outside of the board
            if (newPos.x < 0 || newPos.x > 7 || newPos.y < 0 || newPos.y > 7)
            {
                return moves;
            }

            int occupiedPiece = board.getPiece(newPos);

            if (Piece.isSameColor(piece, occupiedPiece))
            {
                return moves;
            }

            appendMove(moves, board, pos.toIndex(), newPos.toIndex(), allowCheck);
            return moves;
        }

        private static void appendMove(List<Move> moves, Board board, int fr, int to, bool allowCheck)
        {
            appendMove(moves, board, fr, to, allowCheck, Move.FLAG_NONE);
        }

        private static void appendMove(List<Move> moves, Board board, int fr, int to, bool allowCheck, int flag)
        {
            int piece = board.getPiece(fr);

            //do not add move if it results in being in check
            Board resultingBoard = board.makeMove(new Move(fr, to));
            resultingBoard.whiteToMove = !resultingBoard.whiteToMove;

            if (!allowCheck)
            {
                if (resultingBoard.isInCheck())
                {
                    return;
                }
            }

            //add promotions
            int rank = Index.GetRank(to);
            if ((piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN) && ( rank == 0 || rank == 7))
            {
                foreach (int prom_flag in Move.FLAG_PROMOTIONS)
                {
                    moves.Add(new Move(fr, to, prom_flag));
                }
                return;
            }

            moves.Add(new Move(fr, to, flag));
        }

        /// <summary>
        /// Performs an performance test (perft) for the given board, calculates the number of positions generated up to a given depth
        /// used to test whether the move generation is accurate
        /// </summary>
        /// <param name="board">The board to start the generation from</param>
        /// <param name="depth">The number of ply's to generate</param>
        /// <param name="showDetailedOutput">true if output should be showed, false otherwise</param>
        /// <returns></returns>
        public static int perft(Board board, int depth, bool showDetailedOutput)
        {
            if (depth == 0) throw new Exception("Depth must be positive");

            if (!showDetailedOutput) return perft(board, depth);

            int nBoards = 0;
            foreach (Move move in generateAllMoves(board))
            {
                Board result = board.makeMove(move);
                int resultBoards = perft(board.makeMove(move), depth - 1);

                Console.WriteLine($"move:{move} positions:{resultBoards}");

                nBoards += resultBoards;
            }

            Console.WriteLine($"\nTotal positions:{nBoards}");
            return nBoards;
        }

        private static int perft(Board board, int depth)
        {
            if (depth == 0) return 1;

            int nBoards = 0;

            List<Move> moves = generateAllMoves(board);
            foreach (Move move in moves)
            {
                Board result = board.makeMove(move);

                nBoards += perft(result, depth - 1);
            }

            return nBoards;
        }
    }
}