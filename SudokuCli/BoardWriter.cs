using Sudoku.Models;

namespace SudokuCli
{
    public static class BoardWriter
    {
        public const string EmptyBoardLiteral = """
       |       |       
       |       |       
       |       |       
-------+-------+-------
       |       |       
       |       |       
       |       |       
-------+-------+-------
       |       |       
       |       |       
       |       |       
""";
        public const string RowIndexLookup = "012 345 678";
        public const string ColumnIndexLookup = " 0 1 2 | 3 4 5 | 6 7 8 ";
        public static readonly int BoardWidth = ColumnIndexLookup.Length;

        public const int EntropyBuffer = 3;
        public const string EmptyEntropyLiteral = """
      ·       ·      
      ·       ·      
      ·       ·      
· · · · · · · · · · ·
      ·       ·      
      ·       ·      
      ·       ·      
· · · · · · · · · · ·
      ·       ·      
      ·       ·      
      ·       ·      
""";
        public const string EntropyColumnIndexLookup = "0 1 2   3 4 5   6 7 8";
        public static readonly int EntropyBoardWidth = EntropyColumnIndexLookup.Length;

        public const string IgnoredCell = "#";

        private static void Write(Func<Coordinate, (object?, bool)> lookup, int[][]? entropy)
        {
            Console.Clear();

            int finalLeft = entropy is null ? BoardWidth : BoardWidth + EntropyBuffer + EntropyBoardWidth;
            int finalTop = RowIndexLookup.Length;

            //if (entropy is not null)
            //{
            //    Console.SetCursorPosition(BoardWidth + EntropyBuffer, 0);
            //    Console.Write(EmptyEntropyLiteral);
            //    Console.SetCursorPosition(0, 0);
            //}

            Console.Write(EmptyBoardLiteral);
            Board.IterateCells(coord =>
            {
                int cellTop = RowIndexLookup.IndexOf(coord.Row.ToString());
                int cellLeft = ColumnIndexLookup.IndexOf(coord.Column.ToString());

                Console.SetCursorPosition(cellLeft, cellTop);
                (object? value, bool write) = lookup(coord);
                if (write)
                    Console.Write(value);

                if (entropy is not null)
                {
                    int entropyLeft = BoardWidth + EntropyBuffer + EntropyColumnIndexLookup.IndexOf(coord.Column.ToString());

                    Console.SetCursorPosition(entropyLeft, cellTop);
                    if (value is int cell && cell == 0)
                        WriteWithColor(entropy[coord].Length, (value => value is int entropyVolume && entropyVolume == 1, ConsoleColor.Green), (value => value is int entropyVolume && entropyVolume == 0 && cell == 0, ConsoleColor.Red));
                }
            });

            Console.SetCursorPosition(finalLeft, finalTop);
            Console.WriteLine();
            Console.WriteLine();

            static void WriteWithColor(object value, params (Func<object, bool> Condition, ConsoleColor ForegroundColor)[] conditions)
            {
                ConsoleColor originalColor = Console.ForegroundColor;
                foreach (var (condition, foregroundColor) in conditions)
                {
                    if (condition(value))
                    {
                        Console.ForegroundColor = foregroundColor;
                        break;
                    }
                }
                Console.Write(value);
                Console.ForegroundColor = originalColor;
            }
        }

        public static void Write(Func<Coordinate, (object?, bool)> lookup) => Write(lookup, null);
        public static void Write(Func<Coordinate, object?> lookup) => Write(coord => (lookup(coord), true));
        public static void Write(Board board, bool writeEntropy = true) => Write(coord => (board[coord], board[coord] > 0), writeEntropy ? board.Entropy : null);
    }
}