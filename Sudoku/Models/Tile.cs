namespace Sudoku.Models
{
    public struct Tile
    {
        public static readonly List<int> Values = [1, 2, 3, 4, 5, 6, 7, 8, 9];

        private int _value = 0;
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value && Values.Contains(value))
                {
                    _value = value;
                    HasValue = true;
                }
            }
        }

        public bool HasValue { get; private set; } = false;

        public Tile() { }
        public Tile(int value)
        {
            Value = value;
        }

        public override string ToString() => HasValue ? Value.ToString() : "-";

        public static implicit operator Tile(int value) => new(value);
    }
}
