using System;
using System.Numerics;

namespace Engine
{
	public static class BitUtilities
	{
		private const string squareLetters = "ABCDEFGH";
		public static string BitToAlgebraic(ulong n)
		{
			var index = BitOperations.TrailingZeroCount(n);
			return String.Format("{0}{1}", squareLetters[index % 8], index / 8 + 1);
		}

		public static ulong AlgebraicToBit(string a)
		{
			var column = squareLetters.IndexOf(a[0]);
			// Accessing the string by [] gets me a char, which converts to ints based on their unicode values (I'm guessing)
			// So subtracting 49 to make '1' => 0
			var row = Convert.ToInt32(a[1]) - 49;
			var index = row * 8 + column;
			return (ulong)1 << index;
		}
	}
}
