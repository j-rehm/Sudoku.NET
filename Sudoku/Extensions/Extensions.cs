using System.Text;

namespace Sudoku.Extensions
{
    public static class Extensions
    {
        //public List<Tile> MyProperty { get; set; }

        //public static bool CouldBe(this Tile tile, Tile tileOption) => tile.HasFlag(tileOption);

        //public static List<Tile> EnumerateOptions(this Tile tile) => Enum.GetValues

        //public static Tile GetRandomOption(this Tile tile)
        //{

        //}

        //public static void DisplayToConsole(this Board board)
        //{
        //    board.ForEachTileIndex((r, c) =>
        //    {
        //        Tile tile = board.Grid[r, c];

        //        if (r == 0 && c % 3 == 0)
        //        {
        //            Console.WriteLine("-------+-------+-------");
        //        }

        //        if (r % 3 == 0)
        //        {
        //            Console.Write("");
        //        }

        //        Console.Write(tile);
        //    });
        //}

        //public static T SelectRandom<T>(this IEnumerable<T> values) => values.ElementAt(Board.Rng.Next(values.Count()));

        public static void EachColumn<T>(this T[,] grid, Action<int> action)
        {
            for (int c = 0; c < grid.GetLength(0); c++)
                action(c);
        }
        public static void EachRow<T>(this T[,] grid, Action<int> action)
        {
            for (int r = 0; r < grid.GetLength(1); r++)
                action(r);
        }
        public static void Each<T>(this T[,] grid, Action<int, int> action) => grid.EachColumn(c => grid.EachRow(r => action(c, r)));
        public static void Each<T>(this T[,] grid, Action<int, int, T> action) => grid.Each((c, r) => action(c, r, grid[c, r]));

        private static void EachByRow<T>(this T[,] grid, Action<int, int> action) => grid.EachRow(r => grid.EachColumn(c => action(c, r)));
        public static string ToString<T>(this T[,] grid, Func<int, int, object>? func)
        {
            StringBuilder sb = new();

            grid.EachByRow((r, c) =>
            {
                if (r == 0 && c != 0 && c % 3 == 0)
                    sb.AppendLine("-------+-------+-------");

                if (r != 0 && r % 3 == 0)
                    sb.Append(" |");

                sb.Append(' ');
                if (func is not null)
                    sb.Append(func(r, c));
                else sb.Append(grid[r, c]);

                if (r == 8)
                    sb.Append('\n');
            });

            return sb.ToString();
        }
    }
}
