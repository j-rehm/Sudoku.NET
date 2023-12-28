namespace Sudoku.Models
{
    public readonly struct Coordinate
    {
        public readonly int Row { get; }
        public readonly int Column { get; }
        public readonly int Block { get; }

        public readonly int Index { get; }

        public Coordinate(int rowOrIndex, int? column = null)
        {
            Row = column.HasValue ? rowOrIndex : rowOrIndex / Board.Magnitude;
            Column = column ?? rowOrIndex % Board.Magnitude;
            Block = (Row / Board.BlockSize) * Board.BlockSize + (Column / Board.BlockSize);
            Index = column.HasValue ? rowOrIndex * Board.Magnitude + column.Value : rowOrIndex;
        }

        public static implicit operator Coordinate((int row, int column) coordinates) => new(coordinates.row, coordinates.column);
        public static implicit operator (int, int)(Coordinate coordinate) => (coordinate.Row, coordinate.Column);

        public static implicit operator Coordinate(int index) => new(index);
        public static implicit operator int(Coordinate coordinate) => coordinate.Index;

        public bool IsPeerOf(Coordinate coordinate) => Row == coordinate.Row ||
                                                       Column == coordinate.Column ||
                                                       Block == coordinate.Block;

        public override string ToString() => $"({Row}, {Column})";
    }
}
