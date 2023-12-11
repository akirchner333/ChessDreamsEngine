namespace Engine.Pieces.Movers
{
    public class RookMover : IMover
    {
        public static RiderData data;
        public bool Side;

        static RookMover()
        {
            data = new RiderData("Rook", new RookCalc());
        }

        public RookMover() { }

        public ulong MoveMask(int i, Board board)
        {
            return data.MoveMask(i, board, Side);
        }

        public ulong RawMask(int i, ulong occ)
        {
            return data.RawMask(i, occ);
        }

        public ulong XRayAttacks(int i, Board board)
        {
            return data.XRayAttacks(i, board, Side);
        }

        public ulong PathBetween(int i, int target, Board board)
        {
            return data.PathBetween(i, target, board, Side);
        }
    }

    public class RookCalc : IRiderCalc
    {
        private const ulong _notAFile = 0b1111111011111110111111101111111011111110111111101111111011111110;
        private const ulong _notHFile = 0b0111111101111111011111110111111101111111011111110111111101111111;
        public RookCalc() { }

        public ulong CalculateMask(int i, ulong pieces)
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

        public ulong EmptyMask(int i)
        {
            var position = BitUtil.IndexToBit(i);
            return (NorthOccluded(position, ulong.MaxValue) | SouthOccluded(position, ulong.MaxValue)) & ~(Board.Rows[0] | Board.Rows[7])
                | (EastOccluded(position, ulong.MaxValue) | WestOccluded(position, ulong.MaxValue)) & (_notAFile & _notHFile);
        }

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
            return (p << 1) & _notAFile;
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
    }
}
