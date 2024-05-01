using System.Numerics;

namespace Engine
{
    public static class BitUtil
    {
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ CONVERSION METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public static ulong AlgebraicToBit(string a)
        {
            return 1ul << AlgebraicToIndex(a);
        }

        public static int AlgebraicToIndex(string a)
        {
            return CoordToIndex(
                AlgebraicToX(a),
                AlgebraicToY(a)
            );
        }
        
        // So when you access a string with [] you get a char and chars are basically just numbers through unicode
        // So 'a' is 97 through 'h' is 104. But what if it's capital letters? That's 'A' = 65 through 'H' = 72
        // 97 is a prime, so there's nothing we can do with that, but if we subtract one from each value we get 96 and 64, which are both divisible by 32
        // So it's a[0] - 1 % 32 gets use the values, regardless of upper or lower case
        // Since we're moduloing by 32, this'd work for boards with up to 32 files
        // Disclaimer: I was sick with every disease when I wrote this method
        public static int AlgebraicToX(string a)
        {
            return (int)(a[0] - 1) % 32;
        }

        public static int AlgebraicToY(string a)
        {
            return (int)(a[1]) - 49;
        }

        private const string _files = "abcdefgh";
        public static string BitToAlgebraic(ulong n)
        {
            var index = BitOperations.TrailingZeroCount(n);
            return String.Format("{0}{1}", _files[IndexToX(index)], IndexToY(index) + 1);
        }

        public static int BitToIndex(ulong n)
        {
            return BitOperations.TrailingZeroCount(n);
        }

        public static int BitToX(ulong n)
        {
            return IndexToX(BitToIndex(n));
        }

        public static int BitToY(ulong n)
        {
            return IndexToY(BitToIndex(n));
        }

        public static int CoordToIndex(int x, int y)
        {
            return x + y * 8;
        }

        public static ulong CoordToBit(int x, int y)
        {
            return 1ul << CoordToIndex(x, y);
        }

        public static ulong IndexToBit(int i)
        {
            return 1ul << i;
        }

        public static int IndexToX(int i)
        {
            return i % 8;
        }

        public static int IndexToY(int i)
        {
            return i / 8;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ OPERATION METHODS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        //Do `a` and `b` share any bits?
        public static bool Overlap(ulong a, ulong b)
        {
            return (a & b) != 0;
        }

        public static bool Overlap(int a, int b)
        {
            return (a & b) != 0;
        }

        /// <summary>
        /// Method <c>Remove</c> removes all the bits in b from a 
        /// <summary>
        public static ulong Remove(ulong a, ulong b)
        {
            return a & ~b;
        }

        /// <summary>
        /// Method <c>Remove</c> removes all the bits in b from a 
        /// <summary>
        public static int Remove(int a, int b)
        {
            return a & ~b;
        }

        public static ulong[] SplitBits(ulong a)
        {
            var result = new ulong[BitOperations.PopCount(a)];

            var offset = 0;
            var i = 0;
            while ((a >> offset) > 0 && i < result.Count())
            {
                offset += BitOperations.TrailingZeroCount(a >> offset);
                result[i] = 1ul << offset;
                offset++;
                i++;
            }

            return result;
        }

        public static void SplitBitsNew(ulong a, Action<ulong, int> callback)
        {
            var length = BitOperations.PopCount(a);

            var offset = 0;
            var i = 0;
            while ((a >> offset) > 0 && i < length)
            {
                offset += BitOperations.TrailingZeroCount(a >> offset);
                callback(1ul << offset, i);
                offset++;
                i++;
            }
        }

        // Fills all the bits between start and end (inclusive)
        // This'll do something weird if start > end or if you try to put 63 in for end
        public static ulong Fill(int start, int end)
        {
            return ((1ul << (end + 1)) - 1) ^ ((1ul << start) - 1);
        }

        public static ulong SouthFill(ulong a)
        {
            a |= a >> 8;
            a |= a >> 16;
            a |= a >> 32;
            return a;
        }

        public static ulong NorthFill(ulong a)
        {
            a |= a << 8;
            a |= a << 16;
            a |= a << 32;
            return a;
        }

    }
}
