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

        #region Big Board Tests
        public const string BigBoard = """
╔═══════╤═══════╤═══════╦═══════╤═══════╤═══════╦═══════╤═══════╤═══════╗
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╟―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╢
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╟―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╢
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╠═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╣
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╟―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╢
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╟―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╢
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╠═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╣
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╟―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╢
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╟―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╫―――――――┼―――――――┼―――――――╢
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
║       │       │       ║       │       │       ║       │       │       ║
╚═══════╧═══════╧═══════╩═══════╧═══════╧═══════╩═══════╧═══════╧═══════╝
""";
        public static string Big1 = """
/  | 
 | | 
[___]
""";
        public static string Big2 = """
[__ \
/ __/
|___]
""";
        public static string Big3 = """
[__ \
[__ <
[___/
""";
        public static string Big4 = """
| | |
|__ |
  |_|
""";
        public static string Big5 = """
| __]
|__ \
[___/
""";
        public static string Big6 = """
/ __]
| _ \
\___/
""";
        public static string Big7 = """
[__ |
  / /
 /_/ 
""";
        public static string Big8 = """
/ _ \
> _ <
\___/
""";
        public static string Big9 = """
/ _ \
\__ |
[___/
""";
        public static string[] Big = [BigBoard, Big1, Big2, Big3, Big4, Big5, Big6, Big7, Big8, Big9];
        #endregion

        public const string IgnoredCell = "#";
        public const int MsDelay = 25;
         
        private readonly static List<(Board Board, string? About, Board? Solution)> Presets = [];
        public static Board Board { get; private set; } = new();
        public static Board? Solution { get; private set; }

        public static int Register(Board board, string? about = null, Board? solution = null)
        {
            if (!board.OriginalCells.Any(cell => cell != 0))
                board = new(board.Cells);

            for (int i = 0; i < Presets.Count; i++)
            {
                if (Presets[i].Board == board)
                {
                    Presets[i] = (board, about, solution);
                    if (Board == board)
                        Solution = solution;
                    return i;
                }
            }

            Presets.Add((board, about, solution));
            return Presets.Count - 1;
        }
        public static void UnRegister(Board board)
        {
            for (int i = 0; i < Presets.Count; i++)
            {
                if (Presets[i].Board == board)
                {
                    if (Board == board)
                        Unload();
                    Presets.RemoveAt(i);
                    return;
                }
            }
        }

        public static bool Load(int index)
        {
            if (index >= 0 && index < Presets.Count)
            {
                Board = Presets[index].Board;
                Solution = Presets[index].Solution;
                return true;
            }
            return false;
        }
        public static void Unload()
        {
            Board = new();
            Solution = null;
        }

        public static void List()
        {
            Console.Clear();
            for (int i = 0; i < Presets.Count; i++)
                Console.WriteLine($"{i.ToString().PadLeft(Presets.Count / 10 + 1)}: {Presets[i].About}");
        }

        private static void Write(Func<Coordinate, (object?, bool)> lookup, int[][]? entropy, Board? solution = null)
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
            foreach (Coordinate position in Board.IterateCells())
            {
                int cellTop = RowIndexLookup.IndexOf(position.Row.ToString());
                int cellLeft = ColumnIndexLookup.IndexOf(position.Column.ToString());

                Console.SetCursorPosition(cellLeft, cellTop);
                (object? value, bool write) = lookup(position);
                if (write)
                    WriteWithColor(value, (value => solution is not null && value is int cell && solution[position] == cell, ConsoleColor.Green), (value => solution is not null && value is int cell && solution[position] != cell, ConsoleColor.Yellow));

                if (entropy is not null)
                {
                    int entropyLeft = BoardWidth + EntropyBuffer + EntropyColumnIndexLookup.IndexOf(position.Column.ToString());

                    Console.SetCursorPosition(entropyLeft, cellTop);
                    if (value is int cell && cell == 0)
                        WriteWithColor(entropy[position].Length, (entropyValue => entropyValue is int entropySize && entropySize == 1, ConsoleColor.Green), (entropyValue => entropyValue is int entropySize && entropySize == 0 && cell == 0, ConsoleColor.Red));
                }
            }

            Console.SetCursorPosition(finalLeft, finalTop);
            Console.WriteLine();

            static void WriteWithColor(object? value, params (Func<object?, bool> Condition, ConsoleColor ForegroundColor)[] conditions)
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
        public static void Write(Func<Coordinate, object?> lookup) => Write(position => (lookup(position), true));
        public static void Write(Board? boardOverride = null, bool writeEntropy = true)
        {
            Board board = boardOverride ?? Board;
            Write(position => (board[position], board[position] > 0), writeEntropy ? board.Entropy : null, boardOverride == null ? Solution : null);
            foreach (var preset in Presets)
            {
                if (preset.Board == board && preset.About != null)
                {
                    Console.WriteLine(preset.About);
                    Console.WriteLine();
                    break;
                }
            }
        }

        public static void WritePeers(Func<Coordinate, Coordinate[]> peersLookup, int msDelay = MsDelay)
        {
            foreach (Coordinate currentPosition in Board.IterateCells())
            {
                Coordinate[] peers = peersLookup(currentPosition);
                Write(position => position == currentPosition ? "O" : peers.Contains(position) ? "*" : null);
                Delay(msDelay * 4);
            }
            Write();
        }

        public static void Delay(int msDelay = MsDelay) => Thread.Sleep(msDelay);

        public static void ShowBig(int index)
        {
            Console.Clear();
            Console.WriteLine(Big[index]);
            Console.WriteLine();
        }
    }
}

/*
        ╔═══════╤═══════╤═══════╦═══════╤═══════╤═══════╦═══════╤═══════╤═══════╗
/  |    ║       │       │       ║       │       │       ║       │       │       ║
 | |    ║       │       │       ║       │       │       ║       │       │       ║
[___]   ║       │       │       ║       │       │       ║       │       │       ║
        ╟⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╢
[__ \   ║       │       │       ║       │       │       ║       │       │       ║
/ __/   ║       │       │       ║       │       │       ║       │       │       ║
|___]   ║       │       │       ║       │       │       ║       │       │       ║
        ╟⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╢
[__ \   ║       │       │       ║       │       │       ║       │       │       ║
[__ ❬   ║       │       │       ║       │       │       ║       │       │       ║
[___/   ║       │       │       ║       │       │       ║       │       │       ║
        ╠═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╣
| | |   ║       │       │       ║       │       │       ║       │       │       ║
|__ |   ║       │       │       ║       │       │       ║       │       │       ║
  |_|   ║       │       │       ║       │       │       ║       │       │       ║
        ╟⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╢
| __]   ║       │       │       ║       │       │       ║       │       │       ║
|__ \   ║       │       │       ║       │       │       ║       │       │       ║
[___/   ║       │       │       ║       │       │       ║       │       │       ║
        ╟⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╢
/ __]   ║       │       │       ║       │       │       ║       │       │       ║
⎸ _ \   ║       │       │       ║       │       │       ║       │       │       ║
\___/   ║       │       │       ║       │       │       ║       │       │       ║
        ╠═══════╪═══════╪═══════╬═══════╪═══════╪═══════╬═══════╪═══════╪═══════╣
[__ ⎹   ║       │       │       ║       │       │       ║       │       │       ║
  / /   ║       │       │       ║       │       │       ║       │       │       ║
 /_/    ║       │       │       ║       │       │       ║       │       │       ║
        ╟⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╢
/ _ \   ║       │       │       ║       │       │       ║       │       │       ║
❭ _ ❬   ║       │       │       ║       │       │       ║       │       │       ║
\___/   ║       │       │       ║       │       │       ║       │       │       ║
        ╟⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╫⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯┼⎯⎯⎯⎯⎯⎯⎯╢
/ _ \   ║       │       │       ║       │       │       ║       │       │       ║
\__ ⎹   ║       │       │       ║       │       │       ║       │       │       ║
[___/   ║       │       │       ║       │       │       ║       │       │       ║
        ╚═══════╧═══════╧═══════╩═══════╧═══════╧═══════╩═══════╧═══════╧═══════╝
*/