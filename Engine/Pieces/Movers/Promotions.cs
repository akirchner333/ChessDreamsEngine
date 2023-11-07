using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Engine.Pieces.Movers
{
    // Should this be a /Mover/? Or something else, like a MoveBuilder?
    // A converter
    public class Promotions
    {
        public ulong PromoteSquares;
        public bool Side { get; set; }
        public Promotions(ulong promoteSquares)
        {
            PromoteSquares = promoteSquares;
        }

        public Move[] ConvertMask(Board b, ulong start, ulong mask)
        {
            var promoteMask = PromoteSquares & mask;
            var moves = new Move[BitOperations.PopCount(promoteMask) * 4];
            var bits = BitUtil.SplitBits(promoteMask);
            for(var i = 0; i < bits.Length; i++)
            {
                var capture = BitUtil.Overlap(bits[i], b.AllPieces);
                moves[i * 4] = new PromotionMove(start, bits[i], Side, PieceTypes.ROOK) { Capture = capture };
                moves[i * 4 + 1] = new PromotionMove(start, bits[i], Side, PieceTypes.KNIGHT) { Capture = capture };
                moves[i * 4 + 2] = new PromotionMove(start, bits[i], Side, PieceTypes.BISHOP) { Capture = capture };
                moves[i * 4 + 3] = new PromotionMove(start, bits[i], Side, PieceTypes.QUEEN) { Capture = capture };
            }

            return moves;
        }
    }
}
