using Sudoku.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.Models
{
    public sealed class Board
    {
        #region Member Properties
        public const int Magnitude = 9;
        public const int TotalSize = Magnitude * Magnitude;
        public const int BlockSize = Magnitude / 3;

        public const int InvalidPosition = -1;

        public Coordinate[][] Peers { get; }
        public int[] OriginalCells { get; }
        public int[] Cells { get; private set; }
        public int[][] Entropy { get; private set; }

        private readonly Random Rng;
        #endregion

        public Board(int[]? cells = null)
        {
            cells = (int[]?)cells?.Clone() ?? new int[TotalSize];
            if (cells.Length != TotalSize)
                throw new ArgumentException($"Provided cell array must have exactly {TotalSize} indices.", nameof(cells));
            Rng = new();

            Peers = new Coordinate[TotalSize][];
            IterateCells(position => Peers[position] = GetPeers(position));

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
        public int this[Coordinate position]
        {
            get => this[position.Index];
            set => this[position.Index] = value;
        }
        #endregion

        #region Property Initializers
        [MemberNotNull(nameof(Cells))]
        private void ResetCells(int[]? cellsSnapshot = null)
        {
            Cells = (int[])(cellsSnapshot ?? OriginalCells).Clone();
            CalculateEntropy();
        }
        public void Reset() => ResetCells();

        [MemberNotNull(nameof(Entropy))]
        private void CalculateEntropy()
        {
            Entropy = new int[TotalSize][];
            IterateCells(position => Entropy[position] = this[position] != 0 ? [] : 1.UpTo(Magnitude + 1).Where(value => !Peers[position].Select(peerPosition => this[peerPosition]).Contains(value)).ToArray());
        }
        #endregion

        public static Coordinate[] GetRowPeers(Coordinate position) => 0.UpTo(Magnitude).Where(column => position.Column != column).Select(column => new Coordinate(position.Row, column)).ToArray();
        public static Coordinate[] GetColumnPeers(Coordinate position) => 0.UpTo(Magnitude).Where(row => position.Row != row).Select(row => new Coordinate(row, position.Column)).ToArray();
        public static Coordinate[] GetBlockPeers(Coordinate position, bool excludeSameRowColumn = false)
        {
            List<Coordinate> coordinates = [];
            int blockStartRow = position.Block / BlockSize * BlockSize;
            int blockStartCol = position.Block % BlockSize * BlockSize;
            foreach (int row in blockStartRow.UpTo(blockStartRow + BlockSize))
            {
                foreach (int column in blockStartCol.UpTo(blockStartCol + BlockSize))
                {
                    if (!excludeSameRowColumn || (row != position.Row && column != position.Column))
                    {
                        coordinates.Add((row, column));
                    }
                }
            }
            return [.. coordinates];
        }
        public static Coordinate[] GetPeers(Coordinate position) => [.. GetRowPeers(position), .. GetColumnPeers(position), .. GetBlockPeers(position, true)];

        public static IEnumerable<Coordinate> IterateCells()
        {
            foreach (Coordinate position in 0.UpTo(TotalSize))
                yield return position;
        }
        public static void IterateCells(Action<Coordinate> handler)
        {
            foreach (Coordinate position in IterateCells())
                handler(position);
        }
        public static int ClampCellValue(int value) => int.Clamp(value, 0, Magnitude);

        private bool SetCell(Coordinate position, int newValue)
        {
            newValue = ClampCellValue(newValue);
            int oldValue = this[position];
            if (oldValue != newValue)
            {
                Cells[position] = newValue;
                CalculateEntropy();
                return true;
            }
            return false;
        }

        #region Solver
        public int SolveStep(Action<int, int>? notifyUpdate)
        {
            int solved = 0;

            List<Coordinate> collapsedCells = [];
            foreach (Coordinate position in IterateCells())
            {
                if (this[position] == 0)
                {
                    if (Entropy[position].Length == 0)
                        return InvalidPosition;
                        
                    if (Entropy[position].Length == 1)
                        collapsedCells.Add(position);
                }
            }

            if (collapsedCells.Count > 0)
            {
                foreach (Coordinate collapsedCell in collapsedCells)
                {
                    if (Entropy[collapsedCell].Length != 1)
                        return InvalidPosition;

                    SetCell(collapsedCell, Entropy[collapsedCell][0]);
                    solved++;
                    notifyUpdate?.Invoke(solved, 1);
                }
            }

            return solved;
        }
        private (int TotalSolved, int TotalIterations) SolvePositionRecursive(Action<int, int>? notifyUpdate)
        {
            int lowestEntropySize = Entropy.Where(entropy => entropy.Length > 0).Min(entropy => entropy.Length);
            if (lowestEntropySize == 0)
                return (0, 0);
            int[] cellsSnapshot = (int[])Cells.Clone();

            Coordinate[] lowEntropyPositions = IterateCells().Where(position => Entropy[position].Length == lowestEntropySize).ToArray();
            Coordinate selectedPosition = lowEntropyPositions[Rng.Next(lowEntropyPositions.Length)];
            int[] shuffledValues = Extensions.Extensions.Shuffle(Rng, Entropy[selectedPosition]).ToArray();

            int totalSolved = 0;
            int totalIterations = 0;
            int solved = 0;
            foreach (int i in 0.UpTo(Entropy[selectedPosition].Length))
            {
                SetCell(selectedPosition, shuffledValues[i]);
                totalSolved++;
                totalIterations++;
                notifyUpdate?.Invoke(totalSolved, totalIterations);
                (solved, int iterations) = Solve(notifyUpdate);
                totalIterations += iterations;
                
                if (solved == InvalidPosition) // Bad Guess, try again
                {
                    ResetCells(cellsSnapshot);
                    totalSolved = 0;
                    continue;
                }
                else // Good guess, exit
                {
                    totalSolved += solved;
                    break;
                }
            }
            if (solved == InvalidPosition)
                return (InvalidPosition, totalIterations);

            return (totalSolved++, totalIterations); // Increment because this method solved one position, other solutions came as a result
        }
        public (int TotalSolved, int TotalIterations) Solve(Action<int, int>? notifyUpdate = null)
        {
            int totalSolved = 0;
            int totalIterations = 0;

            int solved;
            int iterations;
            do
            {
                solved = SolveStep(notifyUpdate);
                if (solved == InvalidPosition)
                    return (InvalidPosition, totalIterations);
                totalSolved += solved;
                totalIterations += 1;
            } while (solved > 0);

            if (Cells.Any(cell => cell == 0))
            {
                (solved, iterations) = SolvePositionRecursive(notifyUpdate);
                totalIterations += iterations;
                if (solved == InvalidPosition)
                    return (InvalidPosition, totalIterations);
                totalSolved += solved;
            }

            return (totalSolved, totalIterations);
        }
        #endregion
    }
}
