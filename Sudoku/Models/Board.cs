using Sudoku.Enums;
using Sudoku.Extensions;

namespace Sudoku.Models
{
    public class Board
    {
        public const int RngSeed = 453465484;
        public static readonly Random Rng = new(RngSeed);

        public Tile[,] Grid { get; set; }
        public readonly Tile[,] OriginalGrid;

        public Board(int[,]? values = null)
        {
            #region OLD
            //Grid[0, 0].Value = 5;
            //Grid[0, 1].Value = 6;
            //Grid[0, 3].Value = 8;
            //Grid[0, 4].Value = 4;
            //Grid[0, 5].Value = 7;
            //Grid[1, 0].Value = 3;
            //Grid[1, 2].Value = 9;
            //Grid[1, 6].Value = 6;
            //Grid[2, 2].Value = 8;
            //Grid[3, 1].Value = 1;
            //Grid[3, 4].Value = 8;
            //Grid[3, 7].Value = 4;
            //Grid[4, 0].Value = 7;
            //Grid[4, 1].Value = 9;
            //Grid[4, 3].Value = 6;
            //Grid[4, 5].Value = 2;
            //Grid[4, 7].Value = 1;
            //Grid[4, 8].Value = 8;
            //Grid[5, 1].Value = 5;
            //Grid[5, 4].Value = 3;
            //Grid[5, 7].Value = 9;
            //Grid[6, 6].Value = 2;
            //Grid[7, 2].Value = 6;
            //Grid[7, 6].Value = 8;
            //Grid[7, 8].Value = 7;
            //Grid[8, 3].Value = 3;
            //Grid[8, 4].Value = 1;
            //Grid[8, 5].Value = 6;
            //Grid[8, 7].Value = 5;
            //Grid[8, 8].Value = 9;

            //Grid[0, 0].Value = 4;
            //Grid[0, 1].Value = 9;
            //Grid[0, 2].Value = 1;
            //Grid[0, 3].Value = 2;
            //Grid[0, 4].Value = 5;
            //Grid[0, 6].Value = 6;
            //Grid[1, 0].Value = 2;
            //Grid[1, 1].Value = 8;
            //Grid[1, 4].Value = 9;
            //Grid[1, 5].Value = 7;
            //Grid[1, 6].Value = 1;
            //Grid[1, 7].Value = 5;
            //Grid[2, 0].Value = 5;
            //Grid[2, 3].Value = 4;
            //Grid[2, 4].Value = 6;
            //Grid[2, 7].Value = 8;
            //Grid[2, 8].Value = 9;
            //Grid[3, 3].Value = 6;
            //Grid[3, 7].Value = 1;
            //Grid[3, 8].Value = 7;
            //Grid[4, 0].Value = 6;
            //Grid[4, 1].Value = 1;
            //Grid[4, 2].Value = 3;
            //Grid[4, 5].Value = 4;
            //Grid[4, 6].Value = 5;
            //Grid[4, 8].Value = 8;
            //Grid[5, 0].Value = 7;
            //Grid[5, 1].Value = 4;
            //Grid[5, 2].Value = 5;
            //Grid[5, 6].Value = 3;
            //Grid[5, 7].Value = 6;
            //Grid[5, 8].Value = 2;
            //Grid[6, 0].Value = 8;
            //Grid[6, 1].Value = 7;
            //Grid[6, 3].Value = 5;
            //Grid[6, 4].Value = 1;
            //Grid[7, 0].Value = 1;
            //Grid[7, 4].Value = 7;
            //Grid[7, 6].Value = 8;
            //Grid[7, 7].Value = 3;
            //Grid[7, 8].Value = 5;
            //Grid[8, 0].Value = 3;
            //Grid[8, 3].Value = 8;
            //Grid[8, 4].Value = 4;
            //Grid[8, 7].Value = 2;
            #endregion

            values = new int[9, 9] {
                { 0, 2, 0,  0, 0, 5,  0, 9, 8, },
                { 0, 8, 0,  0, 0, 0,  0, 0, 7, },
                { 6, 7, 0,  0, 1, 8,  0, 0, 3, },

                { 8, 0, 0,  3, 9, 0,  0, 5, 0, },
                { 0, 6, 0,  0, 0, 1,  7, 0, 0, },
                { 3, 1, 0,  0, 4, 6,  0, 8, 0, },

                { 0, 5, 0,  4, 7, 0,  3, 6, 1, },
                { 0, 0, 1,  0, 6, 0,  0, 7, 5, },
                { 0, 0, 6,  1, 0, 0,  0, 0, 2, },
            };

            //values = new int[9, 9] {
            //    { 0, 0, 6,  3, 0, 7,  0, 0, 0, },
            //    { 0, 0, 4,  0, 0, 0,  0, 0, 5, },
            //    { 1, 0, 0,  0, 0, 6,  0, 8, 2, },

            //    { 2, 0, 5,  0, 3, 0,  1, 0, 6, },
            //    { 0, 0, 0,  2, 0, 0,  3, 0, 0, },
            //    { 9, 0, 0,  0, 7, 0,  0, 0, 4, },

            //    { 0, 5, 0,  0, 0, 0,  0, 0, 0, },
            //    { 0, 1, 0,  0, 0, 0,  0, 0, 0, },
            //    { 0, 0, 8,  1, 0, 9,  0, 4, 0, },
            //};

            Grid = new Tile[Tile.Values.Count, Tile.Values.Count];
            Grid.Each((c, r) => Grid[c, r] = new());

            if (values != null)
                values.Each((c, r, value) => Grid[r, c].Value = value);

            OriginalGrid = (Tile[,])Grid.Clone();
        }

        public GroupIndices GetGroupIndices(int c, int r) => new(c, r, (r / 3) * 3 + (c / 3));
        public int GetGroupIndex(GroupType groupType, int c, int r)
        {
            GroupIndices groupIndices = GetGroupIndices(c, r);
            if (groupType == GroupType.Row)
                return groupIndices.Row;
            else if (groupType == GroupType.Column)
                return groupIndices.Column;
            else if (groupType == GroupType.Block)
                return groupIndices.Block;
            else return -1;
        }

        public List<int> GetPeerValues(GroupIndices include)
        {
            List<int> peerValues = new(9);

            Grid.Each((c, r) =>
            {
                GroupIndices current = GetGroupIndices(c, r);
                if (current.Row == include.Row ||
                    current.Column == include.Column ||
                    current.Block == include.Block)
                    peerValues.Add(Grid[c, r].Value);
            });

            return peerValues;
        }
        public List<int> GetEntropy(int c, int r)
        {
            List<int> entropy = [];

            if (Grid[c, r].HasValue)
                return entropy;

            GroupIndices current = GetGroupIndices(c, r);
            List<int> peerValues = GetPeerValues(current);
            foreach (int value in Tile.Values)
                if (!peerValues.Contains(value))
                    entropy.Add(value);
            return entropy;
        }
        public List<int>[,] GetEntropy()
        {
            List<int>[,] entropy = new List<int>[Tile.Values.Count, Tile.Values.Count];

            Grid.Each((c, r) => entropy[c, r] = GetEntropy(c, r));

            return entropy;
        }

        public int Solve()
        {
            List<int>[,] entropy = GetEntropy();

            bool isValid = true;
            int solved = 0;
            entropy.Each((c, r, possibilities) =>
            {
                Tile tile = Grid[c, r];
                if (!tile.HasValue)
                {
                    if (possibilities.Count == 0)
                        isValid = false;
                    else if (possibilities.Count == 1)
                    {
                        Grid[c, r].Value = possibilities[0];
                        solved++;
                    }
                }
            });

            if (!isValid)
                return -1;

            return solved;
        }
        public (int Solved, int Iterations) Solve(int depth, Action<int>? action = null)
        {
            int totalSolved = 0;

            int solved;
            int iterations = 0;
            do
            {
                solved = Solve();
                action?.Invoke(solved);
                if (solved == -1)
                    return (-1, iterations);
                totalSolved += solved;
                iterations++;
            } while (solved > 0 && (depth <= 0 || iterations < depth));
            return (totalSolved, iterations);
        }

        public void Reset() => Grid = (Tile[,])OriginalGrid.Clone();

        public string ToString(GroupType groupType) => Grid.ToString((c, r) => GetGroupIndex(groupType, c, r));
        public string ToString(GroupIndices filter) => Grid.ToString((c, r) =>
        {
            GroupIndices current = GetGroupIndices(c, r);
            if (current.Row == filter.Row ||
                current.Column == filter.Column ||
                current.Block == filter.Block)
                return Grid[c, r];
            return "-";
        });
        public override string ToString() => Grid.ToString(null);
        public string ToEntropyString() => Grid.ToString((c, r) => !Grid[c, r].HasValue ? GetEntropy(c, r).Count : "-");
    }
}
