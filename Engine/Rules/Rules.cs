using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rules
{
    public interface IRule
    {
        Move ApplyMove(Move move, int pieceIndex);
        void ReverseMove(Move move, int pieceIndex);
    }
}
