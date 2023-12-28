namespace Sudoku.Models
{
    public class GroupIndices
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Block { get; set; }

        public GroupIndices(int row = -1, int column = -1, int block = -1)
        {
            Row = row;
            Column = column;
            Block = block;
        }
    }
}
