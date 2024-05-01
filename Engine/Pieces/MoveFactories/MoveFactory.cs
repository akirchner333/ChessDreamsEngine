using System.Numerics;

namespace Engine.Pieces.MoveFactories
{
    // Converts boards into arrays of moves
    public class MoveFactory : Base
    {
        public MoveFactory() { }

        public override Move[] ConvertMask(Board b, ulong start, ulong mask)
        {
            var moveMask = mask & ApplicableSquares;
            var moves = new Move[BitOperations.PopCount(moveMask)];
            BitUtil.SplitBitsNew(moveMask, (ulong bit, int i) =>
            {
                moves[i] = new Move(start, bit, Side, BitUtil.Overlap(bit, b.AllPieces));
            });

            return moves;
        }
    }
}
