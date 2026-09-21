using System;

namespace Game2048.Core
{
    public readonly struct BoardPosition : IEquatable<BoardPosition>
    {
        public BoardPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }

        public bool Equals(BoardPosition other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            return obj is BoardPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }
    }
}
