using Sudoku.Models;
using SudokuCli;
using System.Text.RegularExpressions;

int[,] empty = new int[Board.Magnitude, Board.Magnitude] {
    { 0,1,0, 0,0,0, 4,7,0, },
    { 0,0,0, 0,0,5, 3,0,0, },
    { 7,0,0, 2,0,8, 9,0,0, },

    { 0,0,0, 0,2,7, 0,9,4, },
    { 0,2,0, 5,9,0, 0,0,3, },
    { 0,4,0, 8,1,3, 0,0,0, },

    { 9,0,0, 0,0,0, 0,0,8, },
    { 1,0,0, 7,3,0, 0,0,0, },
    { 3,0,0, 0,0,2, 6,5,0, },
};

int[,] stepSolvable = new int[Board.Magnitude, Board.Magnitude] {
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
int[,] guessSolvable = new int[Board.Magnitude, Board.Magnitude] {
    { 0,0,6, 3,0,7, 0,0,0, },
    { 0,0,4, 0,0,0, 0,0,5, },
    { 1,0,0, 0,0,6, 0,8,2, },

    { 2,0,5, 0,3,0, 1,0,6, },
    { 0,0,0, 2,0,0, 3,0,0, },
    { 9,0,0, 0,7,0, 0,0,4, },

    { 0,5,0, 0,0,0, 0,0,0, },
    { 0,1,0, 0,0,0, 0,0,0, },
    { 0,0,8, 1,0,9, 0,4,0, },
};
int[,] uniqueSolvable = new int[Board.Magnitude, Board.Magnitude] {
    { 0,0,0, 8,0,1, 0,0,0, },  //   2 3 7   8 4 1   5 6 9
    { 0,0,0, 0,0,0, 0,4,3, },  //   1 8 6   7 9 5   2 4 3
    { 5,0,0, 0,0,0, 0,0,0, },  //   5 9 4   3 2 6   7 1 8

    { 0,0,0, 0,7,0, 8,0,0, },  //   3 1 5   6 7 4   8 9 2
    { 0,0,0, 0,0,0, 1,0,0, },  //   4 6 9   5 8 2   1 3 7
    { 0,2,0, 0,3,0, 0,0,0, },  //   7 2 8   1 3 9   4 5 6

    { 6,0,0, 0,0,0, 0,7,5, },  //   6 4 2   9 1 8   3 7 5
    { 0,0,3, 4,0,0, 0,0,0, },  //   8 5 3   4 6 7   9 2 1
    { 0,0,0, 2,0,0, 6,0,0, },  //   9 7 1   2 5 3   6 8 4
};

int[,] invalidPosition = new int[Board.Magnitude, Board.Magnitude] {
    { 0,0,6, 3,0,7, 0,0,0, },
    { 0,0,4, 0,0,0, 0,0,5, },
    { 1,0,0, 0,0,6, 0,4,2, },

    { 2,0,5, 0,3,0, 1,0,6, },
    { 0,0,0, 2,0,0, 3,0,0, },
    { 9,0,0, 0,7,0, 0,0,4, },

    { 0,5,0, 0,0,0, 0,0,0, },
    { 0,1,0, 0,0,0, 0,0,0, },
    { 0,0,8, 1,0,9, 0,4,0, },
};
int[,] testPosition = new int[Board.Magnitude, Board.Magnitude] {
    { 0,0,9, 0,5,0, 0,0,0, },
    { 5,0,0, 0,0,0, 4,0,0, },
    { 0,0,0, 0,0,1, 0,8,0, },

    { 0,0,7, 0,0,0, 0,0,9, },
    { 0,0,0, 0,0,4, 7,0,0, },
    { 0,5,0, 8,0,0, 0,0,0, },

    { 0,0,0, 0,1,0, 0,0,3, },
    { 6,0,0, 0,0,0, 0,2,0, },
    { 0,0,0, 0,2,0, 0,0,0, },
};

int[,] stephanieMed1 = new int[Board.Magnitude, Board.Magnitude] {
    { 0,5,0, 9,3,0, 0,4,0, },
    { 7,4,0, 0,1,6, 0,0,0, },
    { 6,0,0, 0,0,0, 8,0,0, },

    { 3,0,1, 0,0,0, 7,0,0, },
    { 4,0,5, 3,2,7, 0,1,6, },
    { 2,0,9, 0,6,5, 3,8,4, },

    { 0,3,0, 0,0,9, 0,0,0, },
    { 0,0,0, 6,7,0, 5,0,8, },
    { 5,0,0, 0,0,0, 0,6,9, },
};
int[,] stephanieMed2 = new int[Board.Magnitude, Board.Magnitude] {
    { 0,0,0, 5,2,0, 0,0,7, },
    { 0,0,0, 0,0,3, 0,6,0, },
    { 0,1,0, 0,6,7, 0,4,0, },

    { 0,7,0, 4,3,5, 0,0,6, },
    { 0,0,9, 0,7,0, 8,5,0, },
    { 0,0,6, 0,8,0, 4,7,3, },

    { 5,9,0, 6,1,0, 0,3,4, },
    { 0,6,4, 0,0,0, 0,1,9, },
    { 1,0,2, 7,0,0, 6,0,5, },
};
int[,] stephanieHard1 = new int[Board.Magnitude, Board.Magnitude] {
    { 0,1,0, 0,0,0, 4,7,0, },
    { 0,0,0, 0,0,5, 3,0,0, },
    { 7,0,0, 2,0,8, 9,0,0, },

    { 0,0,0, 0,2,7, 0,9,4, },
    { 0,2,0, 5,9,0, 0,0,3, },
    { 0,4,0, 8,1,3, 0,0,0, },

    { 9,0,0, 0,0,0, 0,0,8, },
    { 1,0,0, 7,3,0, 0,0,0, },
    { 3,0,0, 0,0,2, 6,5,0, },
};
int[,] stephanieHard2 = new int[Board.Magnitude, Board.Magnitude] {
    { 0,1,0, 0,0,0, 4,7,0, },
    { 0,0,0, 0,0,5, 3,0,0, },
    { 7,0,0, 2,0,8, 9,0,0, },

    { 0,0,0, 0,2,7, 0,9,4, },
    { 0,2,0, 5,9,0, 0,0,3, },
    { 0,4,0, 8,1,3, 0,0,0, },

    { 9,0,0, 0,0,0, 0,0,8, },
    { 1,0,0, 7,3,0, 0,0,0, },
    { 3,0,0, 0,0,2, 6,5,0, },
};

string[] RowAliases = ["row", "r"];
string[] RowsAliases = ["rows", "r"];

string[] ColumnAliases = ["column", "col", "c"];
string[] ColumnsAliases = ["columns", "cols", "c"];

string[] BlockAliases = ["block", "blk", "b"];
string[] BlocksAliases = ["blocks", "blks", "b"];
string[] BlockStarAliases = ["block*", "blk*", "b*"];

string[] CellAliases = ["cell", "c"];
string[] PeersAliases = ["peers", "p"];
string[] OriginalAliases = ["original", "o"];
string[] BoardAliases = ["board", "bd"];
string[] EntropyAliases = ["entropy", "e"];
string[] SolutionAliases = ["solution", "s"];
string[] BigAliases = ["big"];

BoardWriter.Register(empty, "This board is empty.");
BoardWriter.Register(stepSolvable, "This board can be solved without making any guesses.");
BoardWriter.Register(guessSolvable, "This board can only be solved by making guesses.");
BoardWriter.Register(uniqueSolvable, "This board has the minimum of 17 givens.");
BoardWriter.Register(invalidPosition, "This board is not valid.");
BoardWriter.Register(testPosition, "This is a board with randomly-selected givens.");
BoardWriter.Register(stephanieMed1, "This is a board from Stephanie's sudoku app. (Medium 1)");
BoardWriter.Register(stephanieMed2, "This is a board from Stephanie's sudoku app. (Medium 2)");
BoardWriter.Register(stephanieHard1, "This is a board from Stephanie's sudoku app. (Hard 1)");

BoardWriter.Write();

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
        case "list":
            BoardWriter.List();
            break;
        case "load":
            if (options.Length == 1 && int.TryParse(options[0], out int index))
                if (BoardWriter.Load(index))
                    BoardWriter.Write();
                else Console.WriteLine($"Could not find preset at index '{index}'");
            break;
        case "save":
            Console.WriteLine($"The board has been saved to position {BoardWriter.Register(BoardWriter.Board)}.");
            break;
        case "unload":
            BoardWriter.Unload();
            BoardWriter.Write();
            break;
        case "generate":
            BoardWriter.Unload();

            bool updates = false;
            bool delay = false;
            if (options.Length == 1)
            {
                if (options[0].StartsWith('-'))
                {
                    char[] charOptions = options[0].ToCharArray()[1..];
                    updates = charOptions.Contains('u');
                    delay = charOptions.Contains('d');
                }
                else break;
            }

            BoardWriter.Board.Solve(updates ? (solved, iteration) =>
            {
                BoardWriter.Write();
                if (delay)
                    BoardWriter.Delay();
            } : null);

            if (!updates)
                BoardWriter.Write();
            break;
        case "show":
            if (options.Length == 0)
            {
                BoardWriter.Write();
                break;
            }

            string subCommand = options[0];
            if (options.Length == 1)
            {
                if (BigAliases.Contains(subCommand))
                    BoardWriter.ShowBig(0);
                else if (RowsAliases.Contains(subCommand))
                    BoardWriter.Write(position => position.Row);
                else if (ColumnsAliases.Contains(subCommand))
                    BoardWriter.Write(position => position.Column);
                else if (BlocksAliases.Contains(subCommand))
                    BoardWriter.Write(position => position.Block);
                else if (OriginalAliases.Contains(subCommand))
                    BoardWriter.Write(BoardWriter.Board.OriginalCells);
                else if (EntropyAliases.Contains(subCommand))
                    BoardWriter.Write(position => BoardWriter.Board[position] == 0 ? BoardWriter.Board.Entropy[position].Length : null);
                else if (PeersAliases.Contains(subCommand))
                    BoardWriter.WritePeers(position => BoardWriter.Board.Peers[position]);
                else if (SolutionAliases.Contains(subCommand))
                    if (BoardWriter.Solution != null)
                        BoardWriter.Write(BoardWriter.Solution);
                    else Console.WriteLine("A solution for the current board has not been found.");
                break;
            }

            if (options.Length == 2)
            {
                if (PeersAliases.Contains(subCommand))
                {
                    if (BoardAliases.Contains(options[1]))
                        BoardWriter.WritePeers(Board.GetPeers);
                    else if (RowAliases.Contains(options[1]))
                        BoardWriter.WritePeers(Board.GetRowPeers);
                    else if (ColumnAliases.Contains(options[1]))
                        BoardWriter.WritePeers(Board.GetColumnPeers);
                    else if (BlockAliases.Contains(options[1]))
                        BoardWriter.WritePeers(position => Board.GetBlockPeers(position));
                    else if (BlockStarAliases.Contains(options[1]))
                        BoardWriter.WritePeers(position => Board.GetBlockPeers(position, true), 250);
                }
                else if (int.TryParse(options[1], out int includeIndex))
                {
                    if (BigAliases.Contains(subCommand) && includeIndex > 0 && includeIndex < BoardWriter.Big.Length)
                        BoardWriter.ShowBig(includeIndex);
                    else if (RowAliases.Contains(subCommand))
                        BoardWriter.Write(position => position.Row == includeIndex ? BoardWriter.Board[position] : BoardWriter.IgnoredCell);
                    else if (ColumnAliases.Contains(subCommand))
                        BoardWriter.Write(position => position.Column == includeIndex ? BoardWriter.Board[position] : BoardWriter.IgnoredCell);
                    else if (BlockAliases.Contains(subCommand))
                        BoardWriter.Write(position => position.Block == includeIndex ? BoardWriter.Board[position] : BoardWriter.IgnoredCell);
                }
                break;
            }

            if (options.Length == 3)
            {
                if (int.TryParse(options[1], out int rowIndex) && int.TryParse(options[2], out int colIndex))
                {
                    Coordinate providedPosition = (rowIndex, colIndex);
                    if (CellAliases.Contains(subCommand))
                        Console.WriteLine($"Cell ({rowIndex}, {colIndex}): {BoardWriter.Board.Cells[providedPosition]}");
                    if (PeersAliases.Contains(subCommand))
                        BoardWriter.Write(position => position.IsPeerOf(providedPosition) ? BoardWriter.Board[position] : BoardWriter.IgnoredCell);
                    if (EntropyAliases.Contains(subCommand))
                        Console.WriteLine($"Entropy ({rowIndex}, {colIndex}): [ {string.Join(", ", BoardWriter.Board.Entropy[providedPosition])} ]");
                }
                break;
            }

            break;
        case "solve":
            updates = false;
            delay = false;
            if (options.Length == 1)
            {
                if (options[0].StartsWith('-'))
                {
                    char[] charOptions = options[0].ToCharArray()[1..];
                    updates = charOptions.Contains('u');
                    delay = charOptions.Contains('d');
                }
                else break;
            }

            (int totalSolved, int totalIterations) = BoardWriter.Board.Solve(updates ? (solved, iteration) =>
            {
                BoardWriter.Write();
                if (delay)
                    BoardWriter.Delay();
            } : null);

            if (!updates)
                BoardWriter.Write();

            if (!BoardWriter.Board.Cells.Any(cell => cell == 0))
                BoardWriter.Register(BoardWriter.Board, solution: (int[])BoardWriter.Board.Cells.Clone());
            Console.WriteLine($"Solved {totalSolved} positions in {totalIterations} iterations.");
            break;
        case "set":
            if (options.Length == 3)
            {
                if (int.TryParse(options[0], out int rowIndex) && int.TryParse(options[1], out int colIndex) && int.TryParse(options[2], out int value))
                {
                    BoardWriter.Board[rowIndex, colIndex] = value;
                    BoardWriter.Write();
                }
            }
            break;
        case "reset":
            BoardWriter.Board.Reset();
            BoardWriter.Write();
            break;
        case "exit":
            break;
        default:
            Console.WriteLine($"Unknown command: '{command}'");
            break;
    }

} while (input != "exit");