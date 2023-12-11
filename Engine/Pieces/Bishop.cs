using Engine.Pieces.Movers;

namespace Engine
{
    public class Bishop : Piece, IRider
    {
        public override string Name { get; } = "Bishop";
        public override PieceTypes Type { get; } = PieceTypes.BISHOP;
        public override char Short { get; } = 'b';
        public override int Rank { get { return 3; } }

        private BishopMover _mover = new();
        public Bishop(int x, int y, bool side) : this(BitUtil.CoordToBit(x, y), side) { }

        public Bishop(ulong bit, bool side) : base(bit, side)
        {
            _mover.Side = side;
        }

        public override ulong MoveMask(Board board)
        {
            return _mover.MoveMask(Index, board);
        }

        public override ulong Mask(ulong occ)
        {
            return _mover.RawMask(Index, occ);
        }

        public override ulong XRayAttacks(Board board)
        {
            return _mover.XRayAttacks(Index, board);
        }

        public override ulong PathBetween(Board b, int i)
        {
            return _mover.PathBetween(Index, i, b);
        }
    }
}