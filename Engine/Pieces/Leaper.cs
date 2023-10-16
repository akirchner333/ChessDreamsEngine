using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    abstract public class Leaper : Piece
    {
        public Leaper(int x, int y, bool side) : base(x, y, side) { }

        public Leaper(string square, bool side) : base(square, side){ }

        public override ulong MoveMask(Board board)
        {
            return BitUtil.Remove(GetMoveDatabase(Index), allyPieces(board));
        }

        public ulong[] FillMoveDatabase()
        {
            var moves = new ulong[64];
            for (var i = 0; i < 64; i++)
            {
                var index = 1ul << i;
                moves[i] = MovesAtIndex(index);
            }
            return moves;
        }

        // Moves directly to a fixed distance square, once. Knights and (technically) kings
        public static ulong LeaperMask(int steps, ulong blocker, ulong position)
        {
            ulong move = Shifter(steps, position)(1);

            if ((move & blocker) == 0)
                return move;
            return 0;
        }

        public override void MoveTo(ulong end)
        {
            base.MoveTo(end);
            Index = BitUtil.BitToIndex(end);
        }

        abstract public ulong MovesAtIndex(ulong index);
        abstract public ulong GetMoveDatabase(int index);
    }
}
