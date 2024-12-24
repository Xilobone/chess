using System.Diagnostics.Contracts;
using converter;

namespace chess
{
    public class Position
    {
        public int x { get; private set; }
        public int y { get; private set; }

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.x + b.x, a.y + b.y);
        }

        public static bool operator ==(Position? a, Position? b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Position? a, Position? b)
        {
            return !(a == b);
        }

        // override object.Equals
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is not Position)
            {
                return false;
            }

            Position otherPosition = (Position)obj;

            return x == otherPosition.x && y == otherPosition.y;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return 2 * x + 3 * y;
        }

        public override string ToString()
        {
            return NotationConverter.toCoordinates(this);
        }
    }
}