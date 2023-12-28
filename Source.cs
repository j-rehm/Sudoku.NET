using System.Text.RegularExpressions;

const byte Magnitude = 9;
int[] cells = new int[Magnitude * Magnitude];

#region Initialize
//int[,] initialValues = new int[Magnitude, Magnitude] {
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
int[,] initialValues = new int[Magnitude, Magnitude] {
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
Iterate(cells, (r, c) => cells[IndexOf(r, c)] = initialValues[r, c]);
int[] originalCells = Clone(cells);
#endregion

#region Logic
void Iterate(int[] cells, Action<int, int> action)
{
    for (int r = 0; r < Magnitude; r++)
    {
        for (int c = 0; c < Magnitude; c++)
        {
            action(r, c);
        }
    }
}
int IndexOf(int r, int c) => r * Magnitude + c;
int BlockIndexOf(int r, int c) => (r / 3) * 3 + (c / 3);

int Clamp(int value, int min = 1, int max = Magnitude) => value < min ? min : value > max ? max : value;
int[] Clone(int[] cells) => (int[])cells.Clone();
void Set(int r, int c, int value) => cells[IndexOf(r, c)] = Clamp(value);
void Reset(int[]? snapshot = null) => cells = Clone(snapshot ?? originalCells);

bool IsPeerOf(int r, int c, int r1, int c1) => r == r1 || c == c1 || BlockIndexOf(r, c) == BlockIndexOf(r1, c1);
int[] PeerValuesOf(int r, int c)
{
    List<int> peerValues = [];
    Iterate(cells, (r1, c1) =>
    {
        int value = cells[IndexOf(r1, c1)];
        if (value > 0 && !peerValues.Contains(value))
            if (IsPeerOf(r, c, r1, c1))
                peerValues.Add(value);
    });
    return [.. peerValues];
}
int[] EntropyOf(int r, int c)
{
    List<int> entropy = [];
    int[] peerValues = PeerValuesOf(r, c);
    for (int i = 1; i <= Magnitude; i++)
        if (!peerValues.Contains(i))
            entropy.Add(i);
    return [.. entropy];
}
int[] GetEntropy()
{
    int[] entropy = new int[Magnitude * Magnitude];
    Iterate(cells, (r, c) => entropy[IndexOf(r, c)] = cells[IndexOf(r, c)] == 0 ? EntropyOf(r, c).Length : 0);
    return entropy;
}

int SolveStep()
{
    bool isValid = true;
    int solved = 0;
    int[] entropy = GetEntropy();
    Iterate(cells, (r, c) =>
    {
        int i = IndexOf(r, c);
        if (cells[i] == 0)
        {
            if (entropy[i] == 0)
                isValid = false;
            else if (entropy[i] == 1)
            {
                int[] possibilities = EntropyOf(r, c);
                if (possibilities.Length != 1)
                    isValid = false;
                else
                {
                    cells[i] = possibilities[0];
                    solved++;
                }
            }
        }
    });

    return isValid ? solved : -1;
}
//(int, int) SolveGuessSequence()
//{
//    int[] entropy = GetEntropy();
//    int[] snapshotCells = Clone(cells);

//    (int row, int col, int val) = GenerateGuess(entropy);
//    Set(row, col, val);

//    (int row, int col, int val) GenerateGuess(int[] entropy, (int row, int col, int val)? previous = null)
//    {
//        (int row, int col, int val) selected = previous ?? (0, 0, 0);

//        Iterate(entropy, (r, c) =>
//        {
//            if (entropy[IndexOf(r, c)] < entropy[IndexOf(row, col)])
//                (row, col) = (r, c);
//        });

//        int[] possibilities = EntropyOf(row, col);
//        if (previous is null || selected.row != previous.Value.row || selected.col != previous.Value.col)
//            val = possibilities[0];
//        else val = possibilities[Array.IndexOf(possibilities, val) + 1]
//        foreach (int v in )
//            if (v < val)

//        return selected;
//    }
//}
(int TotalSolved, int TotalIterations) Solve(Action<int, int> iterated)
{
    int[] snapshotCells = Clone(cells);
    int totalSolved = 0;
    int iterations = 0;

    int solved;
    do
    {
        solved = SolveStep();

        totalSolved += solved;
        iterations++;
        iterated(solved, iterations);

        if (solved == -1)
        {
            Reset(snapshotCells);
            return (-1, iterations);
        }


    } while (solved > 0);

    return (totalSolved, iterations);
}
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
    Iterate(cells, (r, c) =>
    {
        int boardR = (RQuery.IndexOf(r.ToString()) - 1) / BoardWidth;
        int boardC = CQuery.IndexOf(c.ToString());
        Console.SetCursorPosition(boardC, boardR);

        object? value = lookup is not null ? lookup(r, c, IndexOf(r, c)) : cells[IndexOf(r, c)];
        if (value is not int cell || cell > 0 && cell <= Magnitude)
            Console.Write(value);
    });

    if (displayEntropy)
    {
        int[] entropy = GetEntropy();
        Iterate(entropy, (r, c) =>
        {
            int boardR = (RQuery.IndexOf(r.ToString()) - 1) / BoardWidth;
            int boardC = CQuery.IndexOf(c.ToString()) + BoardWidth + EntropyPadding;
            Console.SetCursorPosition(boardC, boardR);

            int i = IndexOf(r, c);
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
Show(cells);

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
                Show(cells);
                break;
            }

            string subCommand = options[0];
            if (options.Length == 1)
            {
                if (subCommand == "rows" || subCommand == "r")
                    Show(cells, (r, c, _) => r);
                else if (subCommand == "columns" || subCommand == "c")
                    Show(cells, (r, c, _) => c);
                else if (subCommand == "blocks" || subCommand == "b")
                    Show(cells, (r, c, _) => BlockIndexOf(r, c));
                else if (subCommand == "original" || subCommand == "o")
                    Show(originalCells);
                else if (subCommand == "entropy" || subCommand == "e")
                    Show(GetEntropy(), isEntropy: true);
                break;
            }

            if (options.Length == 2)
            {
                if (int.TryParse(options[1], out int includeIndex))
                {
                    if (subCommand == "row" || subCommand == "r")
                        Show(cells, (r, c, i) => r == includeIndex ? cells[i] : IgnoredCell);
                    else if (subCommand == "column" || subCommand == "c")
                        Show(cells, (r, c, i) => c == includeIndex ? cells[i] : IgnoredCell);
                    else if (subCommand == "block" || subCommand == "b")
                        Show(cells, (r, c, i) => BlockIndexOf(r, c) == includeIndex ? cells[i] : IgnoredCell);
                }
                break;
            }

            if (options.Length == 3)
            {
                if (int.TryParse(options[1], out int rowIndex) && int.TryParse(options[2], out int colIndex))
                {
                    int index = IndexOf(rowIndex, colIndex);
                    if (subCommand == "cell" || subCommand == "c")
                        Console.WriteLine($"Cell ({rowIndex}, {colIndex}): {cells[index]}");
                    if (subCommand == "peers" || subCommand == "p")
                        Show(cells, (r, c, i) => IsPeerOf(r, c, rowIndex, colIndex) ? cells[i] : IgnoredCell);
                    if (subCommand == "entropy" || subCommand == "e")
                        Console.WriteLine($"Entropy ({rowIndex}, {colIndex}): [ {string.Join(", ", EntropyOf(rowIndex, colIndex))} ]");
                }
                break;
            }

            break;
        case "solve":
            (int TotalSolved, int TotalIterations) = Solve((solved, iteration) =>
            {
                Show(cells);
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
                    Set(rowIndex, colIndex, value);
                    Show(cells);
                }
            }
            break;
        case "reset":
            Reset();
            Show(cells);
            break;
        case "exit":
            break;
        default:
            Console.WriteLine($"Unknown command: '{command}'");
            break;
    }

} while (input != "exit");
#endregion