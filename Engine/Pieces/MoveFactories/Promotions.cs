using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Text.RegularExpressions;

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
                moves[i * 4] = new PromotionMove(start, bit, Side, PieceTypes.ROOK) { Capture = capture };
                moves[i * 4 + 1] = new PromotionMove(start, bit, Side, PieceTypes.KNIGHT) { Capture = capture };
                moves[i * 4 + 2] = new PromotionMove(start, bit, Side, PieceTypes.BISHOP) { Capture = capture };
                moves[i * 4 + 3] = new PromotionMove(start, bit, Side, PieceTypes.QUEEN) { Capture = capture };
            });

            return moves;
        }
    }
}
