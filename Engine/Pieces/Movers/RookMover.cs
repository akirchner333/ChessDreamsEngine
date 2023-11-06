using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public class RookMover : IMover
    {
        private const ulong _notAFile = 0b1111111011111110111111101111111011111110111111101111111011111110;
        private const ulong _notHFile = 0b0111111101111111011111110111111101111111011111110111111101111111;
        public bool Side;

        public ulong NorthOccluded(ulong p, ulong m)
        {
            p |= m & (p << 8);
            m &= (m << 8);
            p |= m & (p << 16);
            m &= (m << 16);
            p |= m & (p << 32);
            return p << 8;
        }

        public ulong SouthOccluded(ulong p, ulong m)
        {
            p |= m & (p >> 8);
            m &= (m >> 8);
            p |= m & (p >> 16);
            m &= (m >> 16);
            p |= m & (p >> 32);
            return p >> 8;
        }

        public ulong EastOccluded(ulong p, ulong m)
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

        public ulong WestOccluded(ulong p, ulong m)
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

        public ulong MoveMask(int i, Board board)
        {
            var empty = ~board.AllPieces;
            //I don't like having to do this, cause I have the position I'm just not passing it
            //But I suppose I'll be access by index eventually anyhow
            var position = BitUtil.IndexToBit(i);
            return BitUtil.Remove(
                NorthOccluded(position, empty) |
                EastOccluded(position, empty) |
                SouthOccluded(position, empty) |
                WestOccluded(position, empty), 
                board.SidePieces(Side) | position
             );
        }


        public ulong Mask(int index)
        {
            var y = BitUtil.IndexToY(index);
            var position = BitUtil.IndexToBit(index);
            return BitUtil.Fill(BitUtil.CoordToIndex(0, y), BitUtil.CoordToIndex(7, y)) | BitUtil.NorthFill(position) | BitUtil.SouthFill(position);
        }
    }
}
