
using System.Text.RegularExpressions;
using converter;

namespace chess
{   
    /// <summary>
    /// Represents a move in a game of chess
    /// </summary>
    public class Move
    {   
        /// <summary>
        /// Index for castling kingside for white
        /// </summary>
        public const int CASTLE_WHITE_KINGSIDE = 0;

        /// <summary>
        /// Index for castling queenside for white
        /// </summary>
        public const int CASTLE_WHITE_QUEENSIDE = 1;

        /// <summary>
        /// Index for castling kingside black
        /// </summary>
        public const int CASTLE_BLACK_KINGSIDE = 2;

        /// <summary>
        /// Index for castling queenside black
        /// </summary>
        public const int CASTLE_BLACK_QUEENSIDE = 3;

        /// <summary>
        /// Default flag
        /// </summary>
        public const int FLAG_NONE = 0;

        /// <summary>
        /// Flag for castling
        /// </summary>
        public const int FLAG_CASTLING = 1;

        /// <summary>
        /// Flag for promoting to a queen
        /// </summary>
        public const int FLAG_PROMOTE_QUEEN = 2;

        /// <summary>
        /// Flag for promoting to a rook
        /// </summary>
        public const int FLAG_PROMOTE_ROOK = 3;

        /// <summary>
        /// Flag for promoting to a bishop
        /// </summary>
        public const int FLAG_PROMOTE_BISHOP = 4;

        /// <summary>
        /// Flag for promoting to a knight
        /// </summary>
        public const int FLAG_PROMOTE_KNIGHT = 5;

        /// <summary>
        /// All flags for promotion
        /// </summary>
        public static int[] FLAG_PROMOTIONS = [FLAG_PROMOTE_QUEEN, FLAG_PROMOTE_ROOK, FLAG_PROMOTE_KNIGHT, FLAG_PROMOTE_BISHOP];

        /// <summary>
        /// Maps character to promotion flags
        /// </summary>
        public static Dictionary<string, int> PROMOTION_VALUES = new Dictionary<string, int>
    {
        {"q", FLAG_PROMOTE_QUEEN},
        {"r", FLAG_PROMOTE_ROOK},
        {"b", FLAG_PROMOTE_BISHOP},
        {"n", FLAG_PROMOTE_KNIGHT }
    };

        /// <summary>
        /// The flag of this move
        /// </summary>
        public int flag { get; set; }

        /// <summary>
        /// The index of the origin of the move
        /// </summary>
        public int fr { get; private set; }

        /// <summary>
        /// The index of the destination of the move
        /// </summary>
        public int to { get; private set; }

        /// <summary>
        /// Creates a new move
        /// </summary>
        /// <param name="fr">The origin of the move</param>
        /// <param name="to">The destination of the move</param>
        public Move(int fr, int to) : this(fr, to, FLAG_NONE) { }

        /// <summary>
        /// Creates a new move, with a flag
        /// </summary>
        /// <param name="fr">The origin of the move</param>
        /// <param name="to">The destination of the move</param>
        /// <param name="flag">The flag of the move</param>
        public Move(int fr, int to, int flag)
        {
            this.fr = fr;
            this.to = to;

            this.flag = flag;
        }

        /// <summary>
        /// Gets the move from a string
        /// </summary>
        /// <param name="moveStr">A string containing the move, (eg. a2 a4)</param>
        /// <returns></returns>
        public static Move getMove(string moveStr)
        {
            string[] move = moveStr.Split(' ');

            int fr = NotationConverter.toIndex(move[0]);
            int to = NotationConverter.toIndex(move[1]);

            return new Move(fr, to);
        }

        /// <summary>
        /// Gets a string representation of the move
        /// </summary>
        /// <returns>A string representation of the move</returns>
        public override string ToString()
        {
            return $"(From:{NotationConverter.toCoordinates(fr)}, To:{NotationConverter.toCoordinates(to)} {((flag != 0) ? $" Flag:{flag}" : "")})";
        }
    }
}