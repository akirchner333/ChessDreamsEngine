using Engine;

namespace EngineTest
{
    public class PieceTest
    {
        public static IEnumerable<string> EndPoints(Piece p, Board b)
        {
            return p.Moves(b).Where(m => m != null).Select(m => m.EndAlgebraic());
        }

        public static IEnumerable<string> StartEnd(Piece p, Board b)
        {
            return p.Moves(b).Where(m => m != null).Select(m => m.StartEnd());
        }
    }
}
