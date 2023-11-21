using Engine.Pieces.Movers;

namespace Engine
{
    public class Bishop : Piece
    {
        public override string Name { get; } = "Bishop";
        public override PieceTypes Type { get; } = PieceTypes.BISHOP;
        public override char Short { get; } = 'b';

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
    }
}