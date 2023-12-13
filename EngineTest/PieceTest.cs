using Engine;

namespace EngineTest
{
    public class PieceTest
    {
        public static IEnumerable<string> Algebraic(Piece p, Board b)
        {
            return p.Moves(b).Select(m => m.LongAlgebraic());
        }
    }
}
