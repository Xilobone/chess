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

        public static List<Move> generateAllMoves(Board board) {
            List<Move> moves = new List<Move>();

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    moves.AddRange(generateMoves(board, new Position(x, y)));
                }
            }

            return moves;
        }
        public static List<Move> generateMoves(Board board, Position position)
        {
            return generateMoves(board, position, false);
        }
        public static List<Move> generateMoves(Board board, Position position, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            int piece = board.getPiece(position);

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
                moves.AddRange(getPawnMoves(board, position, allowCheck));
            }

            if (piece == Piece.WHITE_ROOK || piece == Piece.BLACK_ROOK)
            {
                moves.AddRange(getRookMoves(board, position, allowCheck));
            }

            if (piece == Piece.WHITE_BISHOP || piece == Piece.BLACK_BISHOP)
            {
                moves.AddRange(getBishopMoves(board, position, allowCheck));
            }

            if (piece == Piece.WHITE_QUEEN || piece == Piece.BLACK_QUEEN)
            {
                moves.AddRange(getQueenMoves(board, position, allowCheck));
            }

            if (piece == Piece.WHITE_KNIGHT || piece == Piece.BLACK_KNIGHT)
            {
                moves.AddRange(getKnightMoves(board, position, allowCheck));
            }

            if (piece == Piece.WHITE_KING || piece == Piece.BLACK_KING)
            {
                moves.AddRange(getKingMoves(board, position, allowCheck));
            }

            return moves;
        }

        private static List<Move> getPawnMoves(Board board, Position pos, bool allowCheck)
        {
            List<Move> moves = new List<Move>();
            int piece = board.getPiece(pos);

            int offset = piece == Piece.WHITE_PAWN ? 1 : -1;
            Position newPos = pos + new Position(0, offset);

            if (newPos.y >= 0 && newPos.y <= 7)
            {
                if (board.getPiece(newPos) == Piece.EMPTY)
                {
                    appendMove(moves, board, pos, newPos, allowCheck);
                }
            }

            //captures
            Position capturePos1 = pos + new Position(-1, offset);
            Position capturePos2 = pos + new Position(1, offset);

            if (capturePos1.x >= 0 && capturePos1.y >= 0 && capturePos1.y <= 7)
            {
                if (Piece.isDifferentColor(board.getPiece(capturePos1), piece) || capturePos1 == board.enpassantPos)
                {
                    appendMove(moves, board, pos, capturePos1, allowCheck);
                }
            }

            if (capturePos2.x <= 7 && capturePos2.y >= 0 && capturePos2.y <= 7)
            {
                if (Piece.isDifferentColor(board.getPiece(capturePos2), piece) || capturePos2 == board.enpassantPos)
                {
                    appendMove(moves, board, pos, capturePos2, allowCheck);
                }
            }

            //double pawn pushes (white)
            if (Piece.isWhite(piece) && pos.y == 1 && board.getPiece(pos + new Position(0, 1)) == Piece.EMPTY)
            {
                Position nPos = pos + new Position(0, 2);
                if (board.getPiece(nPos) == Piece.EMPTY)
                {
                    appendMove(moves, board, pos, nPos, allowCheck);
                }
            }

            //double pawn pushes (black)
            if (Piece.isBlack(piece) && pos.y == 6 && board.getPiece(pos + new Position(0, -1)) == Piece.EMPTY)
            {
                Position nPos = pos + new Position(0, -2);
                if (board.getPiece(nPos) == Piece.EMPTY)
                {
                    appendMove(moves, board, pos, nPos, allowCheck);
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

            //add castling moves
            int piece = board.getPiece(pos);
            if (piece == Piece.WHITE_KING)
            {
                if (board.castlingOptions[Move.CASTLE_WHITE_QUEENSIDE] && board.getPiece(new Position(1, 0)) == Piece.EMPTY && board.getPiece(new Position(2, 0)) == Piece.EMPTY && board.getPiece(new Position(3, 0)) == Piece.EMPTY)
                {
                    appendMove(moves, board, pos, KING_CASTLE_POSITIONS[Move.CASTLE_WHITE_QUEENSIDE], allowCheck, Move.FLAG_CASTLING);
                }
                if (board.castlingOptions[Move.CASTLE_WHITE_KINGSIDE] && board.getPiece(new Position(5, 0)) == Piece.EMPTY && board.getPiece(new Position(6, 0)) == Piece.EMPTY)
                {
                    appendMove(moves, board, pos, KING_CASTLE_POSITIONS[Move.CASTLE_WHITE_KINGSIDE], allowCheck, Move.FLAG_CASTLING);
                }
            }

            if (piece == Piece.BLACK_KING)
            {
                if (board.castlingOptions[Move.CASTLE_BLACK_QUEENSIDE] && board.getPiece(new Position(1, 7)) == Piece.EMPTY && board.getPiece(new Position(2, 7)) == Piece.EMPTY && board.getPiece(new Position(3, 7)) == Piece.EMPTY)
                {
                    appendMove(moves, board, pos, KING_CASTLE_POSITIONS[Move.CASTLE_BLACK_QUEENSIDE], allowCheck, Move.FLAG_CASTLING);
                }
                if (board.castlingOptions[Move.CASTLE_BLACK_KINGSIDE] && board.getPiece(new Position(5, 7)) == Piece.EMPTY && board.getPiece(new Position(6, 7)) == Piece.EMPTY)
                {
                    appendMove(moves, board, pos, KING_CASTLE_POSITIONS[Move.CASTLE_BLACK_KINGSIDE], allowCheck, Move.FLAG_CASTLING);
                }
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
                    appendMove(moves, board, pos, newPos, allowCheck);
                    break;
                }

                appendMove(moves, board, pos, newPos, allowCheck);
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

            appendMove(moves, board, pos, newPos, allowCheck);
            return moves;
        }

        private static void appendMove(List<Move> moves, Board board, Position pos, Position newPos, bool allowCheck)
        {
            appendMove(moves, board, pos, newPos, allowCheck, Move.FLAG_NONE);
        }

        private static void appendMove(List<Move> moves, Board board, Position pos, Position newPos, bool allowCheck, int flag)
        {
            int piece = board.getPiece(pos);

            //do not add move if it results in being in check
            Board resultingBoard = board.makeMove(new Move(pos, newPos));
            //resultingBoard.makeMove(new Move(pos, newPos));
            resultingBoard.whiteToMove = !resultingBoard.whiteToMove;

            if (!allowCheck)
            {
                if (resultingBoard.isInCheck())
                {
                    return;
                }
            }

            //add promotions
            if ((piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN) && (newPos.y == 0 || newPos.y == 7))
            {
                foreach (int prom_flag in Move.FLAG_PROMOTIONS)
                {
                    moves.Add(new Move(pos, newPos, prom_flag));
                }
                return;
            }

            moves.Add(new Move(pos, newPos, flag));
        }
    }
}