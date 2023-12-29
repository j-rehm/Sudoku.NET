using Sudoku.Models;

namespace Sudoku.Extensions
{
    public static class Extensions
    {
        public static int[] ToCells(this int[,] multi)
        {
            int[] cells = new int[Board.TotalSize];
            Board.IterateCells(position => cells[position] = multi[position.Row, position.Column]);
            return cells;
        }

        public static IEnumerable<int> UpTo(this int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
                yield return i;
        }

        public static IEnumerable<T> Shuffle<T>(this Random rng, IEnumerable<T> source)
        {
            List<T> list = source.ToList();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }

            return list;
        }
    }
}
