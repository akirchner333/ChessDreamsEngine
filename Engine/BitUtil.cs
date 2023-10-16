using System;
using System.Drawing;
using System.Numerics;
using System.Threading.Tasks;

namespace Engine
{
	public static class BitUtil
	{
		private const string squareLetters = "ABCDEFGH";
		public static string BitToAlgebraic(ulong n)
		{
			var index = BitOperations.TrailingZeroCount(n);
			return String.Format("{0}{1}", squareLetters[index % 8], index / 8 + 1);
		}

		public static ulong AlgebraicToBit(string a)
		{
			return 1ul << AlgebraicToIndex(a);
		}

		public static int AlgebraicToIndex(string a)
		{
            var column = squareLetters.IndexOf(a[0]);
            // Accessing the string by [] gets me a char, which converts to ints based on their unicode values (I'm guessing)
            // So subtracting 49 to make '1' => 0
            var row = Convert.ToInt32(a[1]) - 49;
            return row * 8 + column;
        }

		public static int BitToIndex(ulong n)
		{
			return BitOperations.TrailingZeroCount(n) + 1;
		}

        //Do `a` and `b` share any bits?
        public static bool Overlap(ulong a, ulong b)
        {
            return (a & b) != 0;
        }

		// Remove all the bits from 
        public static ulong Remove(ulong a, ulong b)
		{
			return (a & b) ^ a;
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
	}
}
