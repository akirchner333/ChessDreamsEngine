using Engine.Pieces.Magic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public class RiderData
    {
        public ulong[] EmptyMasks = new ulong[64];
        public ulong[][] MoveDB = new ulong[64][];
        public ulong[] Magics { get; set; } = new ulong[64];
        public int[] Offset { get; set; } = new int[64];

        public RiderData(string pieceName, IRiderCalc calc)
        {
            Magics = MagicDatabase.LoadMagics(pieceName);
            Offset = MagicDatabase.LoadOffsets(pieceName);

            for (var i = 0; i < 64; i++)
            {
                EmptyMasks[i] = calc.EmptyMask(i);

                var magicNum = Magics[i];
                var offset = Offset[i];
                MoveDB[i] = new ulong[(ulong)Math.Pow(2, 64 - offset)];

                foreach (var m in TestMagic.AllVariants(EmptyMasks[i]))
                {
                    var key = (m * magicNum) >> offset;
                    MoveDB[i][key] = calc.CalculateMask(i, m);
                }
            }
        }

        public ulong MoveMask(int i, Board board, bool side)
        {
            var mask = EmptyMasks[i];
            var key = ((mask & board.AllPieces) * Magics[i]) >> Offset[i];
            return BitUtil.Remove(MoveDB[i][key], board.SidePieces(side));
        }
    }
}
