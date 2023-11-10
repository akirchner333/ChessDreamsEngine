using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Engine.Pieces.Magic;

namespace Engine.Pieces.Movers
{
    public class BishopMover : IMover, IMagicRider
    {
        // I want a place to store this sort of thing, plus files generally, that's really quick and easy to find.
        // Currently I'm putting them all over the place, which is less than ideal
        private const ulong _notAFile = 0b1111111011111110111111101111111011111110111111101111111011111110;
        private const ulong _notHFile = 0b0111111101111111011111110111111101111111011111110111111101111111;

        public static ulong[] EmptyMasks = new ulong[64];
        public static ulong[][] MoveDB = new ulong[64][];

        public static ulong[] Magics { get; set; } = new ulong[64];
        public static int[] Offset { get; set; } = new int[64];

        public bool Side;

        static BishopMover()
        {
            Magics = MagicDatabase.LoadMagics("Bishop");
            Offset = MagicDatabase.LoadOffsets("Bishop");

            for (var i = 0; i< 64; i++)
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

        public BishopMover() { }

        public ulong MoveMask(int i, Board board)
        {
            var mask = EmptyMasks[i];
            var key = ((mask & board.AllPieces) * Magics[i]) >> Offset[i];
            return BitUtil.Remove(MoveDB[i][key], board.SidePieces(Side));
        }

        // ~~~~~~~~~~~~~~~~ CALCULATIONS ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public static ulong EmptyMask(int i)
        {
            var mask = CalculateMask(i, 0);
            return BitUtil.Remove(mask, Board.Columns["A"] | Board.Columns["H"] | Board.Rows[0] | Board.Rows[7] | 1ul << i);
        }

        ulong IMagicRider.EmptyMask(int i)
        {
            return BishopMover.EmptyMask(i);
        }

        public static ulong CalculateMask(int i, ulong pieces)
        {
            var empty = ~pieces;
            var position = BitUtil.IndexToBit(i);
            return NorthEastOccluded(position, empty) |
                SouthEastOccluded(position, empty) |
                SouthWestOccluded(position, empty) |
                NorthWestOccluded(position, empty);
        }

        ulong IMagicRider.CalculateMask(int i, ulong pieces)
        {
            return BishopMover.CalculateMask(i, pieces);
        }

        public static ulong NorthEastOccluded(ulong p, ulong m)
        {
            m &= _notAFile;
            p |= m & (p << 9);
            m &= (m << 9);
            p |= m & (p << 18);
            m &= (m << 18);
            p |= m & (p << 36);
            //Moves it one more to handle captures
            return (p << 9) & _notAFile;
        }

        public static ulong NorthWestOccluded(ulong p, ulong m)
        {
            m &= _notHFile;
            p |= m & (p << 7);
            m &= (m << 7);
            p |= m & (p << 14);
            m &= (m << 14);
            p |= m & (p << 28);
            //Moves it one more to handle captures
            return (p << 7) & _notHFile;
        }

        public static ulong SouthEastOccluded(ulong p, ulong m)
        {
            m &= _notAFile;
            p |= m & (p >> 7);
            m &= (m >> 7);
            p |= m & (p >> 14);
            m &= (m >> 14);
            p |= m & (p >> 28);
            //Moves it one more to handle captures
            return (p >> 7) & _notAFile;
        }

        public static ulong SouthWestOccluded(ulong p, ulong m)
        {
            m &= _notHFile;
            p |= m & (p >> 9);
            m &= (m >> 9);
            p |= m & (p >> 18);
            m &= (m >> 18);
            p |= m & (p >> 36);
            //Moves it one more to handle captures
            return (p >> 9) & _notHFile;
        }
    }
}
