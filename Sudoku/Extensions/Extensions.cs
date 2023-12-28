using Sudoku.Models;

namespace Sudoku.Extensions
{
    public static class Extensions
    {
        public static int[] ToCells(this int[,] multi)
        {
            int[] cells = new int[Board.TotalSize];
            Board.IterateCells(coord => cells[coord] = multi[coord.Row, coord.Column]);
            return cells;
        }

        public static void UpTo(this int startIndex, int endIndex, Action<int>? handler = null)
        {
            for (int i = startIndex; i < endIndex; i++)
                handler?.Invoke(i);
        }

        public static IEnumerable<int> UpTo(this int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
                yield return i;
        }
    }
}
