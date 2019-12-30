using System;

namespace BlazorDemo.Sudoku
{
    public struct Board
    {
        public const int Size = 81;

        private ref struct Sets
        {
            public Span<ushort> row;
            public Span<ushort> col;
            public Span<ushort> square;
        }

        internal byte[] values;

        internal Board(byte[] values)
        {
            this.values = values;
        }

        public static Board New()
        {
            return new Board(new byte[Size]);
        }

        public int this[int index]
        {
            get => values[index];
            set => values[index] = (byte)value;
        }

        public void Clear()
        {
            Array.Clear(values, 0, values.Length);
        }

        public bool Validate()
        {
            Sets sets = new Sets
            {
                row = stackalloc ushort[9],
                col = stackalloc ushort[9],
                square = stackalloc ushort[9]
            };

            return TrySetupSets(sets);
        }

        public bool TrySolve()
        {
            static bool Rec(int index, byte[] values, Sets sets)
            {
                if (index == Size)
                {
                    return true;
                }

                ref byte value = ref values[index];
                if (value != 0)
                {
                    return Rec(index + 1, values, sets);
                }

                int x = index % 9;
                int y = index / 9;

                int sx = x / 3;
                int sy = y / 3;
                int s = 3 * sy + sx;

                ref ushort rowSet = ref sets.row[y];
                ref ushort colSet = ref sets.col[x];
                ref ushort squareSet = ref sets.square[s];

                for (byte number = 1; number < 10; number++)
                {
                    ushort n = (ushort)(1 << number - 1);
                    ushort occupied = (ushort)((rowSet & n) | (colSet & n) | (squareSet & n));
                    if (occupied != 0)
                    {
                        continue;
                    }

                    rowSet |= n;
                    colSet |= n;
                    squareSet |= n;

                    if (Rec(index + 1, values, sets))
                    {
                        value = number;
                        return true;
                    }

                    rowSet &= (ushort)~n;
                    colSet &= (ushort)~n;
                    squareSet &= (ushort)~n;
                }

                return false;
            }

            Sets sets = new Sets
            {
                row = stackalloc ushort[9],
                col = stackalloc ushort[9],
                square = stackalloc ushort[9]
            };

            return TrySetupSets(sets) && Rec(0, values, sets);
        }

        private bool TrySetupSets(Sets sets)
        {
            for (int index = 0; index < values.Length; index++)
            {
                int x = index % 9;
                int y = index / 9;

                int sx = x / 3;
                int sy = y / 3;
                int s = 3 * sy + sx;

                ref ushort rowSet = ref sets.row[y];
                ref ushort colSet = ref sets.col[x];
                ref ushort squareSet = ref sets.square[s];

                ushort n = (ushort)(1 << values[index] - 1);

                ushort occupied = (ushort)((rowSet & n) | (colSet & n) | (squareSet & n));
                if (occupied != 0)
                {
                    return false;
                }

                rowSet |= n;
                colSet |= n;
                squareSet |= n;
            }

            return true;
        }
    }
}
