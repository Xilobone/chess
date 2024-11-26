
namespace chess
{
    public class Piece
    {
        public const int EMPTY = 0;
        public const int WHITE_PAWN = 1;
        public const int WHITE_ROOK = 2;
        public const int WHITE_KNIGHT = 3;
        public const int WHITE_BISHOP = 4;
        public const int WHITE_QUEEN = 5;
        public const int WHITE_KING = 6;

        public const int BLACK_PAWN = 7;
        public const int BLACK_ROOK = 8;

        public const int BLACK_KNIGHT = 9;
        public const int BLACK_BISHOP = 10;
        public const int BLACK_QUEEN = 11;
        public const int BLACK_KING = 12;

        public static Dictionary<int, string> DISPLAY = new Dictionary<int, string>
    {
        { EMPTY, " " },
        { WHITE_PAWN, "P" },
        { WHITE_ROOK, "R" },
        { WHITE_KNIGHT, "N" },
        { WHITE_BISHOP, "B" },
        { WHITE_QUEEN, "Q" },
        { WHITE_KING, "K" },
        { BLACK_PAWN, "p" },
        { BLACK_ROOK, "r" },
        { BLACK_KNIGHT, "n" },
        { BLACK_BISHOP, "b" },
        { BLACK_QUEEN, "q" },
        { BLACK_KING, "k" }
    };

        public static Dictionary<string, int> VALUES = new Dictionary<string, int>
    {
        { " ", EMPTY },
        { "P", WHITE_PAWN },
        { "R", WHITE_ROOK },
        { "N", WHITE_KNIGHT },
        { "B", WHITE_BISHOP },
        { "Q", WHITE_QUEEN },
        { "K", WHITE_KING },
        { "p", BLACK_PAWN },
        { "r", BLACK_ROOK },
        { "n", BLACK_KNIGHT },
        { "b", BLACK_BISHOP },
        { "q", BLACK_QUEEN },
        { "k", BLACK_KING }
    };
        public static bool isWhite(int piece)
        {
            return piece >= WHITE_PAWN && piece <= WHITE_KING;
        }

        public static bool isBlack(int piece)
        {
            return piece >= BLACK_PAWN && piece <= BLACK_KING;
        }

        public static bool isDifferentColor(int piece1, int piece2)
        {
            return (isWhite(piece1) && isBlack(piece2)) || (isBlack(piece1) && isWhite(piece2));
        }

        public static bool isSameColor(int piece1, int piece2)
        {
            return (isWhite(piece1) && isWhite(piece2)) || (isBlack(piece1) && isBlack(piece2));
        }

        public static bool isItsTurn(int piece, bool whiteToMove)
        {
            return (isWhite(piece) && whiteToMove) || (isBlack(piece) && !whiteToMove);
        }

    }
}