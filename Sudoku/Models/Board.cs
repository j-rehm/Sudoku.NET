using Sudoku.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Models
{
    public sealed class Board
    {
        public const int Magnitude = 9;
        public const int TotalSize = Magnitude * Magnitude;
        public const int BlockSize = Magnitude / 3;

        public int[] OriginalCells { get; }
        public int[] Cells { get; private set; }
        public Coordinate[][] Peers { get; private set; }
        public int[][] Entropy { get; private set; }

        public Board(int[]? cells = null)
        {
            cells = (int[]?)cells?.Clone() ?? new int[TotalSize];
            if (cells.Length != TotalSize)
                throw new ArgumentException($"Provided cell array must have exactly {TotalSize} indices.", nameof(cells));

            CalculatePeers();

            OriginalCells = cells;
            ResetCells();
            CalculateEntropy();
        }

        #region Implicit Operators
        public static implicit operator Board(int[] cells) => new(cells);
        public static implicit operator Board(int[,] multi) => new(multi.ToCells());
        #endregion

        #region Indexers
        public int this[int index]
        {
            get => Cells[index];
            set => SetCell(index, value);
        }
        public int this[int row, int column]
        {
            get => this[new Coordinate(row, column).Index];
            set => this[new Coordinate(row, column).Index] = value;
        }
        public int this[Coordinate coord]
        {
            get => this[coord.Index];
            set => this[coord.Index] = value;
        }
        #endregion

        public static void IterateCells(Action<Coordinate> handler) => 0.UpTo(TotalSize, index => handler(index));
        public static int ClampCellValue(int value, int min = 0, int max = Magnitude) => value < min ? min : value > max ? max : value;

        private bool SetCell(Coordinate coord, int newValue, bool recalculate = true)
        {
            newValue = ClampCellValue(newValue);
            int oldValue = this[coord];
            if (oldValue != newValue)
            {
                Cells[coord] = newValue;
                if (recalculate)
                    CalculateEntropy();
                return true;
            }
            return false;
        }

        [MemberNotNull(nameof(Cells))]
        private void ResetCells(int[]? cellsSnapshot = null)
        {
            Cells = (int[])(cellsSnapshot ?? OriginalCells).Clone();
            CalculateEntropy();
        }
        public void Reset() => ResetCells();


        private static Coordinate[] GetRowPeers(Coordinate coord) => 0.UpTo(Magnitude).Where(column => coord.Column != column).Select(column => new Coordinate(coord.Row, column)).ToArray();
        private static Coordinate[] GetColumnPeers(Coordinate coord) => 0.UpTo(Magnitude).Where(row => coord.Row != row).Select(row => new Coordinate(row, coord.Column)).ToArray();
        private static Coordinate[] GetBlockPeers(Coordinate coord, bool excludeSameRowColumn = false)
        {
            List<Coordinate> coordinates = [];
            int blockStartRow = coord.Block / BlockSize * BlockSize;
            int blockStartCol = coord.Block % BlockSize * BlockSize;
            foreach (int row in blockStartRow.UpTo(blockStartRow + BlockSize))
            {
                foreach (int column in blockStartCol.UpTo(blockStartCol + BlockSize))
                {
                    if (!excludeSameRowColumn || (row != coord.Row && column != coord.Column))
                    {
                        coordinates.Add((row, column));
                    }
                }
            }
            return [.. coordinates];
        }

        [MemberNotNull(nameof(Peers))]
        private void CalculatePeers()
        {
            Peers = new Coordinate[TotalSize][];
            IterateCells(coord => Peers[coord] = [.. GetRowPeers(coord), .. GetColumnPeers(coord), .. GetBlockPeers(coord, true)]);
        }

        [MemberNotNull(nameof(Entropy))]
        private void CalculateEntropy()
        {
            Entropy = new int[TotalSize][];
            IterateCells(coord => Entropy[coord] = 1.UpTo(Magnitude + 1).Where(value => !Peers[coord].Select(peerCoord => this[peerCoord]).Contains(value)).ToArray());
        }

        public int SolveStep()
        {
            bool isValid = true;
            int solved = 0;
            IterateCells((coord) =>
            {
                int[] entropy = Entropy[coord];
                if (this[coord] == 0)
                {
                    if (entropy.Length == 0)
                        isValid = false;
                    else if (entropy.Length == 1)
                    {
                        SetCell(coord, entropy[0], false);
                        solved++;
                    }
                }
            });
            CalculateEntropy();
            return isValid ? solved : -1;
        }

        public (int TotalSolved, int TotalIterations) Solve(Action<int, int> iterated)
        {
            int totalSolved = 0;
            int totalIterations = 0;

            int solved;
            do
            {
                solved = SolveStep();
                totalSolved += solved;
                totalIterations++;
                iterated(solved, totalIterations);

                if (solved == -1)
                    return (-1, totalIterations);
            } while (solved > 0);

            return (totalSolved, totalIterations);
        }
    }
}
