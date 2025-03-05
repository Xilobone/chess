
namespace chess
{
    /// <summary>
    /// Represents a piece on a chess board
    /// </summary>
    public class Piece
    {
        /// <summary>
        /// No piece
        /// </summary>
        public const int EMPTY = 0;

        /// <summary>
        /// The white pawn
        /// </summary>
        public const int WHITE_PAWN = 1;

        /// <summary>
        /// The white rook
        /// </summary>
        public const int WHITE_ROOK = 2;

        /// <summary>
        /// The white knight
        /// </summary>
        public const int WHITE_KNIGHT = 3;

        /// <summary>
        /// The white bishop
        /// </summary>
        public const int WHITE_BISHOP = 4;

        /// <summary>
        /// The white queen
        /// </summary>
        public const int WHITE_QUEEN = 5;

        /// <summary>
        /// The white king
        /// </summary>
        public const int WHITE_KING = 6;

        /// <summary>
        /// The black pawn
        /// </summary>
        public const int BLACK_PAWN = 7;

        /// <summary>
        /// The black rook
        /// </summary>
        public const int BLACK_ROOK = 8;

        /// <summary>
        /// the black knight
        /// </summary>
        public const int BLACK_KNIGHT = 9;

        /// <summary>
        /// The black bishop
        /// </summary>
        public const int BLACK_BISHOP = 10;

        /// <summary>
        /// The black queen
        /// </summary>
        public const int BLACK_QUEEN = 11;

        /// <summary>
        /// The black king
        /// </summary>
        public const int BLACK_KING = 12;

        /// <summary>
        /// Maps the values of the piece to the console display characters
        /// </summary>
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

        /// <summary>
        /// Maps the console display characters of the pieces to the integer values
        /// </summary>
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
        /// <summary>
        /// Whether the piece is a white piece or not
        /// </summary>
        /// <param name="piece">The piece to check</param>
        /// <returns>True if the piece is white, false otherwise</returns>
        public static bool isWhite(int piece)
        {
            return piece >= WHITE_PAWN && piece <= WHITE_KING;
        }

        /// <summary>
        /// Whether the piece is a black piece or not
        /// </summary>
        /// <param name="piece">The piece to check</param>
        /// <returns>True if the piece is black, false otherwise</returns>
        public static bool isBlack(int piece)
        {
            return piece >= BLACK_PAWN && piece <= BLACK_KING;
        }

        /// <summary>
        /// Checks if two pieces are a different color
        /// </summary>
        /// <param name="piece1">The first piece</param>
        /// <param name="piece2">The second piece</param>
        /// <returns>True if the pieces are of a different color, false otherwise</returns>
        public static bool isDifferentColor(int piece1, int piece2)
        {
            return (isWhite(piece1) && isBlack(piece2)) || (isBlack(piece1) && isWhite(piece2));
        }

        /// <summary>
        /// Checks if two pieces are the same color
        /// </summary>
        /// <param name="piece1">The first piece</param>
        /// <param name="piece2">The second piece</param>
        /// <returns>True if the pieces are the same color, false otherwise</returns>
        public static bool isSameColor(int piece1, int piece2)
        {
            return (isWhite(piece1) && isWhite(piece2)) || (isBlack(piece1) && isBlack(piece2));
        }

        /// <summary>
        /// Checks if a piece is allowed to move
        /// </summary>
        /// <param name="piece">The piece to check</param>
        /// <param name="whiteToMove">Whether white is to move or not</param>
        /// <returns>True if it is the pieces turn to move, false otherwise</returns>
        public static bool isItsTurn(int piece, bool whiteToMove)
        {
            return (isWhite(piece) && whiteToMove) || (isBlack(piece) && !whiteToMove);
        }

    }
}