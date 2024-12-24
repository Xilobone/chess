
using System.Text.RegularExpressions;
using converter;

namespace chess
{

    public class Move
    {
        public const int CASTLE_WHITE_KINGSIDE = 0;
        public const int CASTLE_WHITE_QUEENSIDE = 1;
        public const int CASTLE_BLACK_KINGSIDE = 2;
        public const int CASTLE_BLACK_QUEENSIDE = 3;

        public const int FLAG_NONE = 0;
        public const int FLAG_CASTLING = 1;

        public const int FLAG_PROMOTE_QUEEN = 2;
        public const int FLAG_PROMOTE_ROOK = 3;
        public const int FLAG_PROMOTE_BISHOP = 4;
        public const int FLAG_PROMOTE_KNIGHT = 5;

        public static int[] FLAG_PROMOTIONS = [FLAG_PROMOTE_QUEEN, FLAG_PROMOTE_ROOK, FLAG_PROMOTE_KNIGHT, FLAG_PROMOTE_KNIGHT];

        public static Dictionary<string, int> PROMOTION_VALUES = new Dictionary<string, int>
    {
        {"q", FLAG_PROMOTE_QUEEN},
        {"r", FLAG_PROMOTE_ROOK},
        {"b", FLAG_PROMOTE_BISHOP},
        {"n", FLAG_PROMOTE_KNIGHT }
    };

        public int flag { get; set; }
        public Position fr { get; private set; }
        public Position to { get; private set; }

        public Move(Position fr, Position to) : this(fr, to, FLAG_NONE) { }

        public Move(Position fr, Position to, int flag)
        {
            this.fr = fr;
            this.to = to;
            this.flag = flag;
        }

        public static bool isValid(string? move_str)
        {
            if (move_str == null)
            {
                return false;
            }

            string[] move = move_str.Split(' ');

            if (move.Length != 2)
            {
                return false;
            }

            return Regex.IsMatch(move[0], "^[A-Za-z][1-8]$") && Regex.IsMatch(move[1], "^[A-Za-z][1-8]$");
        }

        public static Move getMove(string move_str)
        {
            string[] move = move_str.Split(' ');

            Position fr = NotationConverter.toPosition(move[0]);
            Position to = NotationConverter.toPosition(move[1]);

            return new Move(fr, to);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Move other = (Move)obj;

            return fr == other.fr && to == other.to;
        }

        public override int GetHashCode()
        {
            return fr.GetHashCode() + to.GetHashCode();
        }

        public override string ToString()
        {
            return $"(From:{fr}, To:{to})";
        } 
    }
}