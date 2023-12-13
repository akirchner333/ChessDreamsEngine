using System.Numerics;

namespace Engine.Pieces.MoveFactories
{
    public class Promotions : Base
    {
        public Promotions(ulong applicableSquares) : base()
        {
            ApplicableSquares = applicableSquares;
        }

        public override Move[] ConvertMask(Board b, ulong start, ulong mask)
        {
            var moveMask = mask & ApplicableSquares;
            var moves = new Move[BitOperations.PopCount(moveMask) * 4];
            BitUtil.SplitBitsNew(moveMask, (ulong bit, int i) =>
            {
                var capture = BitUtil.Overlap(bit, b.AllPieces);
                var baseMove = new Move(start, bit, Side) { Capture = capture };
                moves[i * 4] = baseMove with { Promotion = PieceTypes.ROOK };
                moves[i * 4 + 1] = baseMove with { Promotion = PieceTypes.KNIGHT };
                moves[i * 4 + 2] = baseMove with { Promotion = PieceTypes.BISHOP };
                moves[i * 4 + 3] = baseMove with { Promotion = PieceTypes.QUEEN };
            });

            return moves;
        }
    }
}
