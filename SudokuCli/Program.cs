using Sudoku.Enums;
using Sudoku.Extensions;
using Sudoku.Models;
using System.Text.RegularExpressions;

Board board = new();

Console.Clear();
Console.WriteLine(board);

do
{
    if (Console.CursorLeft != 0)
        Console.WriteLine();
    Console.Write("> ");

    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
        continue;

    string fullCommand = Regex.Replace(input, @" {2,}", " ").Trim();
    string[] components = fullCommand.Split(' ');

    string command = components[0];
    string[] arguments = components[1..];

    if (command == "exit")
        break;

    Console.WriteLine();
    switch (components[0])
    {
        case "show":
            if (arguments.Length == 0)
            {
                Console.WriteLine(board);
                break;
            }

            if (arguments.Length == 1)
            {
                if (arguments[0] == "rows")
                    Console.WriteLine(board.ToString(GroupType.Row));
                else if (arguments[0] == "cols")
                    Console.WriteLine(board.ToString(GroupType.Column));
                else if (arguments[0] == "blks")
                    Console.WriteLine(board.ToString(GroupType.Block));
                else if (arguments[0] == "ogn")
                    Console.WriteLine(board.OriginalGrid.ToString(null));
                else if (arguments[0] == "epy")
                    Console.WriteLine(board.ToEntropyString());
                break;
            }

            if (arguments.Length == 2)
            {
                if (int.TryParse(arguments[1], out int groupIndex))
                {
                    if (arguments[0] == "row")
                        Console.WriteLine(board.ToString(new GroupIndices(row: groupIndex)));
                    else if (arguments[0] == "col")
                        Console.WriteLine(board.ToString(new GroupIndices(column: groupIndex)));
                    else if (arguments[0] == "blk")
                        Console.WriteLine(board.ToString(new GroupIndices(block: groupIndex)));
                }
                break;
            }

            if (arguments.Length == 3)
            {
                if (int.TryParse(arguments[1], out int c) && int.TryParse(arguments[2], out int r))
                {
                    if (arguments[0] == "tile")
                        Console.WriteLine(board.Grid[c, r]);
                    if (arguments[0] == "epy")
                        Console.WriteLine(string.Join(", ", board.GetEntropy(c, r)));
                }
                break;
            }

            break;
        case "solve":
            int depth = 0;
            if (arguments.Length == 1)
                _ = int.TryParse(arguments[0], out depth);

            //(int cursorLeft, int cursorTop) = Console.GetCursorPosition();

            (int totalSolved, int iterations) = board.Solve(depth/*, solved =>
            {
                Console.Write($"Solved {solved} positions.\n\n");
                Console.Write(board);

                Console.SetCursorPosition(cursorLeft, cursorTop);

                Thread.Sleep(250);
            }*/);

            Console.WriteLine($"Solved {totalSolved} positions in {iterations} iterations.\n");
            Console.WriteLine(board);
            break;
        case "reset":
            board.Reset();
            Console.WriteLine(board);
            break;
        default:
            Console.WriteLine($"Unknown command '{components[0]}'");
            break;
    }

} while (true);