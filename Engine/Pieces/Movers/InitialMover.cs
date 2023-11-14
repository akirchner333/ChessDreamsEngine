namespace Engine.Pieces.Movers
{
    // A class for moves that can only be made when the piece is on a specific set of squares
    // Generally for initial moves, like pawns, but could be others also

    // Currently it's set up to be able to able doing the initial movement twice
    // Worth coming back to genericize it probably
    // Also maybe the world's least magical magic bitboards?
    class InitialMover : IMover
    {
        public ulong Starting { get; private set; }
        public int Direction { get; private set; }
        private Func<ulong, int, ulong> _shifter;

        //public Func<int, Board, ulong> MoveMask { get; private set; }
        public InitialMover(ulong starting, int direction)
        {
            Starting = starting;
            Direction = direction;
            _shifter = MoverUtil.BareShifter(direction);
        }

        public ulong MoveMask(int index, Board board)
        {
            var position = BitUtil.IndexToBit(index);
            var empty = ~board.AllPieces;
            empty &= _shifter(empty, 1);
            return _shifter(position & Starting, 2) & empty;
        }
    }
}
