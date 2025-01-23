using counters;

namespace chess
{
    public static class MoveGenerator
    {
        private static int[] KNIGHT_OFFSETS = new int[] { -17, -15, -10, -6, 6, 10, 15, 17 };
        private static ulong[] KNIGHT_OFFSETS_EXCLUDE = new ulong[] { BitBoard.aFile, BitBoard.hFile, BitBoard.abFile, BitBoard.ghFile, BitBoard.abFile, BitBoard.ghFile, BitBoard.aFile, BitBoard.hFile };

        private static int[] KING_OFFSETS = new int[] { -9, -8, -7, -1, 1, 7, 8, 9 };
        private static ulong[] KING_OFFSETS_EXCLUDE = new ulong[] { BitBoard.aFile, 0, BitBoard.hFile, BitBoard.aFile, BitBoard.hFile, BitBoard.aFile, 0, BitBoard.hFile };


        private static Dictionary<int, int> KING_CASTLE_POSITIONS = new Dictionary<int, int>
    {
        { Move.CASTLE_WHITE_KINGSIDE, 6},
        { Move.CASTLE_WHITE_QUEENSIDE, 2 },
        { Move.CASTLE_BLACK_KINGSIDE, 62 },
        { Move.CASTLE_BLACK_QUEENSIDE, 58 }
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
                moves.AddRange(getRookMoves(board, index, allowCheck));
            }

            if (piece == Piece.WHITE_BISHOP || piece == Piece.BLACK_BISHOP)
            {
                moves.AddRange(getBishopMoves(board, index, allowCheck));
            }

            if (piece == Piece.WHITE_QUEEN || piece == Piece.BLACK_QUEEN)
            {
                moves.AddRange(getQueenMoves(board, index, allowCheck));
            }

            if (piece == Piece.WHITE_KNIGHT || piece == Piece.BLACK_KNIGHT)
            {
                moves.AddRange(getKnightMoves(board, index, allowCheck));
            }

            if (piece == Piece.WHITE_KING || piece == Piece.BLACK_KING)
            {
                moves.AddRange(getKingMoves(board, index, allowCheck));
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

        private static List<Move> getRookMoves(Board board, int index, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            moves.AddRange(searchDirection(board, index, 1, allowCheck));
            moves.AddRange(searchDirection(board, index, -1, allowCheck));
            moves.AddRange(searchDirection(board, index, 8, allowCheck));
            moves.AddRange(searchDirection(board, index, -8, allowCheck));

            return moves;
        }

        private static List<Move> getBishopMoves(Board board, int index, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            moves.AddRange(searchDirection(board, index, 9, allowCheck));
            moves.AddRange(searchDirection(board, index, 7, allowCheck));
            moves.AddRange(searchDirection(board, index, -7, allowCheck));
            moves.AddRange(searchDirection(board, index, -9, allowCheck));

            return moves;
        }

        private static List<Move> getQueenMoves(Board board, int index, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            moves.AddRange(getRookMoves(board, index, allowCheck));
            moves.AddRange(getBishopMoves(board, index, allowCheck));

            return moves;
        }

        private static List<Move> searchOffset(Board board, int index, int[] offsets, ulong[] exclude, bool allowCheck)
        {
            List<Move> moves = new List<Move>();

            ulong bitboardPiece = 1ul << index;
            bool isWhite = Piece.isWhite(board.getPiece(index));

            for (int i = 0; i < offsets.Length; i++)
            {
                //do not add move if the knight is on the files to exclude
                if ((bitboardPiece & exclude[i]) != 0) continue;

                int to = index + offsets[i];

                if (to < 0 || to >= 64) continue;
                //do not add move if the to position is occupied by a friendly piece
                ulong friendlyPieces = BitBoard.GetAny(board, isWhite);
                if (((1uL << to) & friendlyPieces) != 0) continue;

                appendMove(moves, board, index, to, allowCheck);

            }

            return moves;
        }
        private static List<Move> getKnightMoves(Board board, int index, bool allowCheck)
        {
            return searchOffset(board, index, KNIGHT_OFFSETS, KNIGHT_OFFSETS_EXCLUDE, allowCheck);
        }

        private static List<Move> getKingMoves(Board board, int index, bool allowCheck)
        {
            List<Move> moves = searchOffset(board, index, KING_OFFSETS, KING_OFFSETS_EXCLUDE, allowCheck);

            //add castling moves
            ulong bitboardKing = board.whiteToMove ? board.bitboardsWhite[BitBoard.KING] : board.bitboardsBlack[BitBoard.KING];
            ulong[] bitboardSafeCastle = board.whiteToMove ? bitboardSafeCastleWhite : bitboardSafeCastleBlack;
            int kingside = board.whiteToMove ? Move.CASTLE_WHITE_KINGSIDE : Move.CASTLE_BLACK_KINGSIDE;
            int queenside = board.whiteToMove ? Move.CASTLE_WHITE_QUEENSIDE : Move.CASTLE_BLACK_QUEENSIDE;
            int queensideCheck = board.whiteToMove ? 1 : 57;

            ulong allPiecesExceptKing = BitBoard.GetAny(board) & ~bitboardKing;

            bool safeToCastle = (bitboardSafeCastle[0] & (allPiecesExceptKing | BitBoard.GetAnyAttack(board, !board.whiteToMove))) == 0;
            if (board.castlingOptions[kingside] && safeToCastle)
            {
                appendMove(moves, board, index, KING_CASTLE_POSITIONS[kingside], allowCheck, Move.FLAG_CASTLING);
            }

            //last condition checks if b1 is empty
            safeToCastle = (bitboardSafeCastle[1] & (allPiecesExceptKing | BitBoard.GetAnyAttack(board, !board.whiteToMove))) == 0 & (allPiecesExceptKing & (1UL << queensideCheck)) == 0;
            if (board.castlingOptions[queenside] && safeToCastle)
            {
                appendMove(moves, board, index, KING_CASTLE_POSITIONS[queenside], allowCheck, Move.FLAG_CASTLING);
            }

            return moves;
        }

        private static List<Move> searchDirection(Board board, int index, int shift, bool allowCheck)
        {
            bool isWhite = Piece.isWhite(board.getPiece(index));
            List<Move> moves = new List<Move>();
            int to = index;
            ulong pos = 1ul << index;

            while (true)
            {
                ulong before = pos;
                pos = shift > 0 ? (pos << shift) : (pos >> -shift);
                to += shift;

                //break if wrapped around the board
                if (((before & BitBoard.aFile) != 0) && ((pos & BitBoard.hFile) != 0)) break;
                if (((before & BitBoard.hFile) != 0) && ((pos & BitBoard.aFile) != 0)) break;

                //break if encountered a friendly piece
                if ((pos & BitBoard.GetAny(board, isWhite)) != 0) break;

                if (to < 0 || to >= 64) break;

                appendMove(moves, board, index, to, allowCheck);

                //break if just captured an enemy piece
                if ((pos & BitBoard.GetAny(board, !isWhite)) != 0) break;

                // reached top or bottom of board
                if (pos == 0) break;

                //reached left or right edge of board
                if (shift != 8 && shift != -8 && ((pos & BitBoard.aFile) != 0 || (pos & BitBoard.hFile) != 0)) break;
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
            if ((piece == Piece.WHITE_PAWN || piece == Piece.BLACK_PAWN) && (rank == 0 || rank == 7))
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