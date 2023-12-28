using Sudoku.Extensions;
using Sudoku.Models;
using SudokuCli;
using System.Text.RegularExpressions;

Board board = new int[Board.Magnitude, Board.Magnitude] {
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

BoardWriter.Write(board);

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
            if (options.Length == 0)
            {
                BoardWriter.Write(board);
                break;
            }

            string subCommand = options[0];
            if (options.Length == 1)
            {
                if (subCommand == "rows" || subCommand == "r")
                    BoardWriter.Write(coord => coord.Row);
                else if (subCommand == "columns" || subCommand == "c")
                    BoardWriter.Write(coord => coord.Column);
                else if (subCommand == "blocks" || subCommand == "b")
                    BoardWriter.Write(coord => coord.Block);
                else if (subCommand == "original" || subCommand == "o")
                    BoardWriter.Write(board.OriginalCells);
                else if (subCommand == "entropy" || subCommand == "e")
                    BoardWriter.Write(coord => board[coord] == 0 ? board.Entropy[coord].Length : null);
                else if (subCommand == "peers" || subCommand == "p")
                {
                    Board.IterateCells(currentCoord =>
                    {
                        BoardWriter.Write(coord => coord == currentCoord ? "O" : coord.IsPeerOf(currentCoord) ? "*" : null);
                        Thread.Sleep(100);
                    });
                    BoardWriter.Write(board);
                }
                break;
            }

            if (options.Length == 2)
            {
                if (int.TryParse(options[1], out int includeIndex))
                {
                    if (subCommand == "row" || subCommand == "r")
                        BoardWriter.Write(coord => coord.Row == includeIndex ? board[coord] : BoardWriter.IgnoredCell);
                    else if (subCommand == "column" || subCommand == "c")
                        BoardWriter.Write(coord => coord.Column == includeIndex ? board[coord] : BoardWriter.IgnoredCell);
                    else if (subCommand == "block" || subCommand == "b")
                        BoardWriter.Write(coord => coord.Block == includeIndex ? board[coord] : BoardWriter.IgnoredCell);
                }
                break;
            }

            if (options.Length == 3)
            {
                if (int.TryParse(options[1], out int rowIndex) && int.TryParse(options[2], out int colIndex))
                {
                    Coordinate providedCoord = (rowIndex, colIndex);
                    if (subCommand == "cell" || subCommand == "c")
                        Console.WriteLine($"Cell ({rowIndex}, {colIndex}): {board.Cells[providedCoord]}");
                    if (subCommand == "peers" || subCommand == "p")
                        BoardWriter.Write(coord => coord.IsPeerOf(providedCoord) ? board[coord] : BoardWriter.IgnoredCell);
                    if (subCommand == "entropy" || subCommand == "e")
                        Console.WriteLine($"Entropy ({rowIndex}, {colIndex}): [ {string.Join(", ", board.Entropy[providedCoord])} ]");
                }
                break;
            }

            break;
        case "solve":
            (int TotalSolved, int TotalIterations) = board.Solve((solved, iteration) =>
            {
                BoardWriter.Write(board);
                Thread.Sleep(100);
            });

            BoardWriter.Write(board);
            Console.WriteLine($"Solved {TotalSolved} positions in {TotalIterations} iterations.");
            break;
        case "set":
            if (options.Length == 3)
            {
                if (int.TryParse(options[0], out int rowIndex) && int.TryParse(options[1], out int colIndex) && int.TryParse(options[2], out int value))
                {
                    board[rowIndex, colIndex] = value;
                    BoardWriter.Write(board);
                }
            }
            break;
        case "reset":
            board.Reset();
            BoardWriter.Write(board);
            break;
        case "exit":
            break;
        default:
            Console.WriteLine($"Unknown command: '{command}'");
            break;
    }

} while (input != "exit");