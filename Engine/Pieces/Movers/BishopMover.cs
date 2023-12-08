namespace Engine.Pieces.Movers
{
    public class BishopMover : IMover
    {
        public static RiderData data;
        public bool Side;

        static BishopMover()
        {
            data = new RiderData("Bishop", new BishopCalc());
        }

        public BishopMover() { }

        public ulong MoveMask(int i, Board board)
        {
            return data.MoveMask(i, board, Side);
        }

        public ulong AttackMask(int i, Board board)
        {
            return data.RawMask(i, board.AllPieces);
        }

        public ulong EmptyMask(int i)
        {
            return data.RawMask(i, 0ul);
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

    public class BishopCalc : IRiderCalc
    {
        private const ulong _notAFile = 0b1111111011111110111111101111111011111110111111101111111011111110;
        private const ulong _notHFile = 0b0111111101111111011111110111111101111111011111110111111101111111;
        public BishopCalc() { }

        public ulong EmptyMask(int i)
        {
            var mask = CalculateMask(i, 0);
            return BitUtil.Remove(mask, Board.Columns["A"] | Board.Columns["H"] | Board.Rows[0] | Board.Rows[7] | 1ul << i);
        }

        public ulong CalculateMask(int i, ulong pieces)
        {
            var empty = ~pieces;
            var position = BitUtil.IndexToBit(i);
            return NorthEastOccluded(position, empty) |
                SouthEastOccluded(position, empty) |
                SouthWestOccluded(position, empty) |
                NorthWestOccluded(position, empty);
        }

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
    }
}
