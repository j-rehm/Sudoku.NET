namespace Sudoku
{
    public class Board
    {
        public const byte Magnitude = 9;

        public int[] Cells { get; private set; } = new int[Magnitude * Magnitude];
        public int[] OriginalCells { get; private set; }

        #region Initialize
        public Board(int[,] initialValues)
        {
            if (initialValues.GetLength(0) != Magnitude ||
                initialValues.GetLength(1) != Magnitude)
                throw new ArgumentException($"The intiial values provided must be a multidimensional array of [{Magnitude}, {Magnitude}]", nameof(initialValues));
            Iterate((r, c) => Cells[IndexOf(r, c)] = initialValues[r, c]);
            OriginalCells = Clone(Cells);
        }
        #endregion

        #region Logic
        public static void Iterate(Action<int, int> action)
        {
            for (int r = 0; r < Magnitude; r++)
            {
                for (int c = 0; c < Magnitude; c++)
                {
                    action(r, c);
                }
            }
        }
        public static int IndexOf(int r, int c) => r * Magnitude + c;
        public static int BlockIndexOf(int r, int c) => (r / 3) * 3 + (c / 3);

        public static int Clamp(int value, int min = 1, int max = Magnitude) => value < min ? min : value > max ? max : value;
        public static int[] Clone(int[] cells) => (int[])cells.Clone();
        
        public void Set(int r, int c, int value) => Cells[IndexOf(r, c)] = Clamp(value);
        public void Reset(int[]? snapshotCells = null) => Cells = Clone(snapshotCells ?? OriginalCells);

        public static bool IsPeerOf(int r, int c, int r1, int c1) => r == r1 || c == c1 || BlockIndexOf(r, c) == BlockIndexOf(r1, c1);
        public int[] PeerValuesOf(int r, int c)
        {
            List<int> peerValues = [];
            Iterate((r1, c1) =>
            {
                int value = Cells[IndexOf(r1, c1)];
                if (value > 0 && !peerValues.Contains(value))
                    if (IsPeerOf(r, c, r1, c1))
                        peerValues.Add(value);
            });
            return [.. peerValues];
        }
        public int[] EntropyOf(int r, int c)
        {
            List<int> entropy = [];
            int[] peerValues = PeerValuesOf(r, c);
            for (int i = 1; i <= Magnitude; i++)
                if (!peerValues.Contains(i))
                    entropy.Add(i);
            return [.. entropy];
        }
        public int[] GetEntropy()
        {
            int[] entropy = new int[Magnitude * Magnitude];
            Iterate((r, c) => entropy[IndexOf(r, c)] = Cells[IndexOf(r, c)] == 0 ? EntropyOf(r, c).Length : 0);
            return entropy;
        }

        public int SolveStep()
        {
            bool isValid = true;
            int solved = 0;
            int[] entropy = GetEntropy();
            Iterate((r, c) =>
            {
                int i = IndexOf(r, c);
                if (Cells[i] == 0)
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
                            Cells[i] = possibilities[0];
                            solved++;
                        }
                    }
                }
            });

            return isValid ? solved : -1;
        }
        //public (int, int) SolveGuessSequence()
        //{
        //    int[] entropy = GetEntropy();
        //    int[] snapshotCells = Clone(Cells);

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

        //                return selected;
        //    }
        //}
        public (int TotalSolved, int TotalIterations) Solve(Action<int, int> iterated)
        {
            int[] snapshotCells = Clone(Cells);
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
    }
}
