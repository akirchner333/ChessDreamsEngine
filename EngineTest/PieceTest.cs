using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineTest
{
    public class PieceTest
    {
        public static IEnumerable<string> EndPoints(Piece p, Board b)
        {
            return p.Moves(b).Select(m => m.EndAlgebraic());
        }

        public static IEnumerable<string> StartEnd(Piece p, Board b)
        {
            return p.Moves(b).Select(m => m.StartEnd());
        }
    }
}
