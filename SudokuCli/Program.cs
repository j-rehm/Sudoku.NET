using Sudoku.Models;
using SudokuCli;
using System.Text.RegularExpressions;

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
int[,] uniqueSolution = new int[Board.Magnitude, Board.Magnitude] {
    { 2,3,7, 8,4,1, 5,6,9, },
    { 1,8,6, 7,9,5, 2,4,3, },
    { 5,9,4, 3,2,6, 7,1,8, },

    { 3,1,5, 6,7,4, 8,9,2, },
    { 4,6,9, 5,8,2, 1,3,7, },
    { 7,2,8, 1,3,9, 4,5,6, },

    { 6,4,2, 9,1,8, 3,7,5, },
    { 8,5,3, 4,6,7, 9,2,1, },
    { 9,7,1, 2,5,3, 6,8,4, },
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

string[] NoUpdatesAliases = ["noupdates", "n"];

BoardWriter.Register(stepSolvable);
BoardWriter.Register(guessSolvable);
BoardWriter.Register(uniqueSolvable);
BoardWriter.Register(uniqueSolvable, uniqueSolution);
BoardWriter.Register(invalidPosition);

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
        case "load":
            if (options.Length == 1 && int.TryParse(options[0], out int index))
                if (BoardWriter.Load(index))
                    BoardWriter.Write();
                else Console.WriteLine($"Could not find preset at index '{index}'");
            break;
        case "unload":
            BoardWriter.Unload();
            BoardWriter.Write();
            break;
        case "generate":
            BoardWriter.Unload();
            BoardWriter.Unload();

            bool noUpdates = options.Length == 1 && NoUpdatesAliases.Contains(options[0]);
            BoardWriter.Board.Solve(noUpdates ? null : (solved, iteration) =>
            {
                BoardWriter.Write();
                BoardWriter.Delay();
            });

            if (noUpdates)
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
                if (RowsAliases.Contains(subCommand))
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
                    if (RowAliases.Contains(subCommand))
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
            noUpdates = options.Length == 1 && NoUpdatesAliases.Contains(options[0]);
            (int totalSolved, int totalIterations) = BoardWriter.Board.Solve(noUpdates ? null : (solved, iteration) =>
            {
                BoardWriter.Write();
                BoardWriter.Delay();
            });

            if (!BoardWriter.Board.Cells.Any(cell => cell == 0))
                BoardWriter.Register(BoardWriter.Board, (int[])BoardWriter.Board.Cells.Clone());

            if (noUpdates)
                BoardWriter.Write();
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