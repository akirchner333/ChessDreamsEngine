using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public class BishopMover : IMover
    {
        // I want a place to store this sort of thing, plus files generally, that's really quick and easy to find.
        // Currently I'm putting them all over the place, which is less than ideal
        private const ulong _notAFile = 0b1111111011111110111111101111111011111110111111101111111011111110;
        private const ulong _notHFile = 0b0111111101111111011111110111111101111111011111110111111101111111;
        public bool Side;
        public ulong NorthEastOccluded(ulong p, ulong m)
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

        public ulong NorthWestOccluded(ulong p, ulong m)
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

        public ulong SouthEastOccluded(ulong p, ulong m)
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

        public ulong SouthWestOccluded(ulong p, ulong m)
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

        public ulong MoveMask(int i, Board board)
        {
            var empty = ~board.AllPieces;
            var position = BitUtil.IndexToBit(i);
            return BitUtil.Remove(
                NorthEastOccluded(position, empty) |
                SouthEastOccluded(position, empty) |
                SouthWestOccluded(position, empty) |
                NorthWestOccluded(position, empty),
                board.SidePieces(Side) | position
             );
        }
    }
}
