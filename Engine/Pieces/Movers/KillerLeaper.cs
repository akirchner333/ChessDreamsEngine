using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public class KillerLeaper : Leaper
    {
        public KillerLeaper(int[] directions, string key) : base(directions, key) { }

        public override ulong MoveMask(int index, Board board)
        {
            return GetMoveDatabase(index) & board.SidePieces(!Side);
        }
    }
}
