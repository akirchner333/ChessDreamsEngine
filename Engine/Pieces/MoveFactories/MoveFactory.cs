using System.Numerics;

namespace Engine.Pieces.MoveFactories
{
    public class MoveFactory : Base
    {
        public MoveFactory() { }

        public override Move[] ConvertMask(Board b, ulong start, ulong mask)
        {
            var moveMask = mask & ApplicableSquares;
            var moves = new Move[BitOperations.PopCount(moveMask)];
            BitUtil.SplitBitsNew(moveMask, (ulong bit, int i) =>
            {
                moves[i] = new Move(start, bit, Side) { Capture = BitUtil.Overlap(bit, b.AllPieces) };
            });

            return moves;
        }
    }
}
