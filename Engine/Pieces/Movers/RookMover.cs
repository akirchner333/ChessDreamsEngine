using Engine.Pieces.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public class RookMover : IMover, IMagicRider
    {
        private const ulong _notAFile = 0b1111111011111110111111101111111011111110111111101111111011111110;
        private const ulong _notHFile = 0b0111111101111111011111110111111101111111011111110111111101111111;
        public bool Side;

        public static ulong[] EmptyMasks = new ulong[64];
        public static ulong[][] MoveDB = new ulong[64][];
        public static ulong[] Magics { get; set; } = new ulong[64];
        public static int[] Offset { get; set; } = new int[64];

        static RookMover()
        {
            Magics = MagicDatabase.LoadMagics("Rook");
            Offset = MagicDatabase.LoadOffsets("Rook");

            for (var i = 0; i < 64; i++)
            {
                EmptyMasks[i] = EmptyMask(i);

                var magicNum = Magics[i];
                var offset = Offset[i];
                MoveDB[i] = new ulong[(ulong)Math.Pow(2, 64 - offset)];

                foreach (var m in TestMagic.AllVariants(EmptyMasks[i]))
                {
                    var key = (m * magicNum) >> offset;
                    MoveDB[i][key] = CalculateMask(i, m);
                }
            }
        }

        public RookMover() { }

        public ulong MoveMask(int i, Board board)
        {
            var mask = EmptyMasks[i];
            var key = ((mask & board.AllPieces) * Magics[i]) >> Offset[i];
            return BitUtil.Remove(MoveDB[i][key], board.SidePieces(Side));
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~ Calcuations ~~~~~~~~~~~~~~~~~~~~~
        public static ulong CalculateMask(int i, ulong pieces)
        {
            var empty = ~pieces;
            var position = BitUtil.IndexToBit(i);
            return BitUtil.Remove(
                NorthOccluded(position, empty) |
                EastOccluded(position, empty) |
                SouthOccluded(position, empty) |
                WestOccluded(position, empty),
                position
             );
        }

        public static ulong EmptyMask(int i)
        {
            var position = BitUtil.IndexToBit(i);
            return (NorthOccluded(position, ulong.MaxValue) | SouthOccluded(position, ulong.MaxValue)) & ~(Board.Rows[0] | Board.Rows[7])
                | (EastOccluded(position, ulong.MaxValue) | WestOccluded(position, ulong.MaxValue)) & (_notAFile & _notHFile);
        }

        ulong IMagicRider.CalculateMask(int i, ulong pieces)
        {
            return RookMover.CalculateMask(i, pieces);
        }

        ulong IMagicRider.EmptyMask(int i)
        {
            return RookMover.EmptyMask(i);
        }

        public static ulong NorthOccluded(ulong p, ulong m)
        {
            p |= m & (p << 8);
            m &= (m << 8);
            p |= m & (p << 16);
            m &= (m << 16);
            p |= m & (p << 32);
            return p << 8;
        }

        public static ulong SouthOccluded(ulong p, ulong m)
        {
            p |= m & (p >> 8);
            m &= (m >> 8);
            p |= m & (p >> 16);
            m &= (m >> 16);
            p |= m & (p >> 32);
            return p >> 8;
        }

        public static ulong EastOccluded(ulong p, ulong m)
        {
            m &= _notAFile;
            p |= m & (p << 1);
            m &= (m << 1);
            p |= m & (p << 2);
            m &= (m << 2);
            p |= m & (p << 4);
            //Moves it one more to handle captures
            return (p << 1) & _notAFile ;
        }

        public static ulong WestOccluded(ulong p, ulong m)
        {
            m &= _notHFile;
            p |= m & (p >> 1);
            m &= (m >> 1);
            p |= m & (p >> 2);
            m &= (m >> 2);
            p |= m & (p >> 4);
            //Moves it one more to handle captures
            return (p >> 1) & _notHFile;
        }
    }
}
