using Sudoku;
using System.Text.RegularExpressions;

#region Initialize
//int[,] initialValues = new int[Board.Magnitude, Board.Magnitude] {
//    { 0,0,6, 3,0,7, 0,0,0, },
//    { 0,0,4, 0,0,0, 0,0,5, },
//    { 1,0,0, 0,0,6, 0,8,2, },

//    { 2,0,5, 0,3,0, 1,0,6, },
//    { 0,0,0, 2,0,0, 3,0,0, },
//    { 9,0,0, 0,7,0, 0,0,4, },

//    { 0,5,0, 0,0,0, 0,0,0, },
//    { 0,1,0, 0,0,0, 0,0,0, },
//    { 0,0,8, 1,0,9, 0,4,0, },
//};
int[,] initialValues = new int[Board.Magnitude, Board.Magnitude] {
    { 0,2,0, 0,0,5, 0,9,8, },
    { 0,8,0, 0,0,0, 0,0,7, },
    { 6,7,0, 0,1,8, 0,0,3, },

    { 8,0,0, 3,9,0, 0,5,0, },
    { 0,6,0, 0,0,1, 7,0,0, },
    { 3,1,0, 0,4,6, 0,8,0, },

    { 0,5,0, 4,7,0, 3,6,1, },
    { 0,0,1, 0,6,0, 0,7,5, },
    { 0,0,6, 1,0,0, 0,0,2, },
};
Board board = new(initialValues);
#endregion

#region Show
#region RSLs
const string EmptyBoard = """
 - - - | - - - | - - - 
 - - - | - - - | - - - 
 - - - | - - - | - - - 
-------+-------+-------
 - - - | - - - | - - - 
 - - - | - - - | - - - 
 - - - | - - - | - - - 
-------+-------+-------
 - - - | - - - | - - - 
 - - - | - - - | - - - 
 - - - | - - - | - - - 
""";
const string RQuery = """
 0 - - | - - - | - - - 
 1 - - | - - - | - - - 
 2 - - | - - - | - - - 
-------+-------+-------
 3 - - | - - - | - - - 
 4 - - | - - - | - - - 
 5 - - | - - - | - - - 
-------+-------+-------
 6 - - | - - - | - - - 
 7 - - | - - - | - - - 
 8 - - | - - - | - - - 
""";
const string CQuery = " 0 1 2 | 3 4 5 | 6 7 8 ";

const int EntropyPadding = 1;
const int BoardWidth = 23;
const string IgnoredCell = "#";
#endregion

void Show(int[] cells, Func<int, int, int, object?>? lookup = null, bool isEntropy = false)
{
    Console.Clear();

    bool displayEntropy = lookup is null && !isEntropy;
    Console.Write(EmptyBoard);
    (int finalLeft, int finalTop) = displayEntropy ? ((BoardWidth * 2) + EntropyPadding - 1, Console.CursorTop) : Console.GetCursorPosition();
    Board.Iterate((r, c) =>
    {
        int boardR = (RQuery.IndexOf(r.ToString()) - 1) / BoardWidth;
        int boardC = CQuery.IndexOf(c.ToString());
        Console.SetCursorPosition(boardC, boardR);

        object? value = lookup is not null ? lookup(r, c, Board.IndexOf(r, c)) : cells[Board.IndexOf(r, c)];
        if (value is not int cell || cell > 0 && cell <= Board.Magnitude)
            Console.Write(value);
    });

    if (displayEntropy)
    {
        int[] entropy = board.GetEntropy();
        Board.Iterate((r, c) =>
        {
            int boardR = (RQuery.IndexOf(r.ToString()) - 1) / BoardWidth;
            int boardC = CQuery.IndexOf(c.ToString()) + BoardWidth + EntropyPadding;
            Console.SetCursorPosition(boardC, boardR);

            int i = Board.IndexOf(r, c);
            if (cells[i] == 0)
                SetIf(() => Console.Write(entropy[i]),
                    (() => entropy[i] == 1, ConsoleColor.Green),
                    (() => entropy[i] == 0 && cells[i] == 0, ConsoleColor.Red));
        });
    }

    Console.SetCursorPosition(finalLeft, finalTop);
    Console.WriteLine();
    Console.WriteLine();

    void SetIf(Action exec, params (Func<bool> Condition, ConsoleColor ForegroundColor)[] conditions)
    {
        ConsoleColor originalColor = Console.ForegroundColor;
        foreach (var condition in conditions)
        {
            if (condition.Condition())
            {
                Console.ForegroundColor = condition.ForegroundColor;
                break;
            }
        }
        exec();
        Console.ForegroundColor = originalColor;
    }
}
#endregion

#region CLI
Show(board.Cells);

string? input;
do
{
    Console.Write("> ");
    input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    string[] components = Regex.Replace(input, @" {2,}", " ").Trim().Split(' ');
    string command = components[0];
    string[] options = components[1..];

    switch (command)
    {
        case "show":
            //Dictionary<char, (Type, bool)> optionDatas = new()
            //{
            //    { 'r', (typeof(int[]), true) },
            //    { 'c', (typeof(int[]), true) },
            //    { 'b', (typeof(int[]), true) }
            //};
            //Dictionary<char, object> optionValues = [];

            //string? currentOption = null;
            //for (int i = 0; i < options.Length; i++)
            //{
            //    Match match = Regex.Match(options[i], @"-([a-zA-Z])");
            //    if (match.Captures.Count == 1)
            //    {
            //        char option = match.Captures[0].Value[0];
            //        if (optionDatas.TryGetValue(option, out (Type Type, bool RequiresParameter) optionData))
            //        {
            //            if (typeof(IEnumerable).IsAssignableFrom(optionData.Type) && optionData.Type != typeof(string))
            //            {
            //                optionValues.TryGetValue(option, out object? value);
            //                value ??= [];
            //                optionValues[option] = 
            //            }
            //        }
            //        else i++;
            //    }
            //    else if (currentOption == null)
            //    {
            //        Console.WriteLine($"Unexpected token: '{options[i]}'");
            //        continue;
            //    }
            //}

            if (options.Length == 0)
            {
                Show(board.Cells);
                break;
            }

            string subCommand = options[0];
            if (options.Length == 1)
            {
                if (subCommand == "rows" || subCommand == "r")
                    Show(board.Cells, (r, c, _) => r);
                else if (subCommand == "columns" || subCommand == "c")
                    Show(board.Cells, (r, c, _) => c);
                else if (subCommand == "blocks" || subCommand == "b")
                    Show(board.Cells, (r, c, _) => Board.BlockIndexOf(r, c));
                else if (subCommand == "original" || subCommand == "o")
                    Show(board.OriginalCells);
                else if (subCommand == "entropy" || subCommand == "e")
                    Show(board.GetEntropy(), isEntropy: true);
                break;
            }

            if (options.Length == 2)
            {
                if (int.TryParse(options[1], out int includeIndex))
                {
                    if (subCommand == "row" || subCommand == "r")
                        Show(board.Cells, (r, c, i) => r == includeIndex ? board.Cells[i] : IgnoredCell);
                    else if (subCommand == "column" || subCommand == "c")
                        Show(board.Cells, (r, c, i) => c == includeIndex ? board.Cells[i] : IgnoredCell);
                    else if (subCommand == "block" || subCommand == "b")
                        Show(board.Cells, (r, c, i) => Board.BlockIndexOf(r, c) == includeIndex ? board.Cells[i] : IgnoredCell);
                }
                break;
            }

            if (options.Length == 3)
            {
                if (int.TryParse(options[1], out int rowIndex) && int.TryParse(options[2], out int colIndex))
                {
                    int index = Board.IndexOf(rowIndex, colIndex);
                    if (subCommand == "cell" || subCommand == "c")
                        Console.WriteLine($"Cell ({rowIndex}, {colIndex}): {board.Cells[index]}");
                    if (subCommand == "peers" || subCommand == "p")
                        Show(board.Cells, (r, c, i) => Board.IsPeerOf(r, c, rowIndex, colIndex) ? board.Cells[i] : IgnoredCell);
                    if (subCommand == "entropy" || subCommand == "e")
                        Console.WriteLine($"Entropy ({rowIndex}, {colIndex}): [ {string.Join(", ", board.EntropyOf(rowIndex, colIndex))} ]");
                }
                break;
            }

            break;
        case "solve":
            (int TotalSolved, int TotalIterations) = board.Solve((solved, iteration) =>
            {
                Show(board.Cells);
                //Console.WriteLine($"Solved {solved} positions on iteration {iteration}.");
                Thread.Sleep(100);
            });

            //Show(cells);
            Console.WriteLine($"Solved {TotalSolved} positions in {TotalIterations} iterations.");

            break;
        case "set":
            if (options.Length == 3)
            {
                if (int.TryParse(options[0], out int rowIndex) && int.TryParse(options[1], out int colIndex) && int.TryParse(options[2], out int value))
                {
                    board.Set(rowIndex, colIndex, value);
                    Show(board.Cells);
                }
            }
            break;
        case "reset":
            board.Reset();
            Show(board.Cells);
            break;
        case "exit":
            break;
        default:
            Console.WriteLine($"Unknown command: '{command}'");
            break;
    }

} while (input != "exit");
#endregion