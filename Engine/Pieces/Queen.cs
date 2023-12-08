using Engine.Pieces.Movers;

namespace Engine
{
    public class Queen : Piece, IRider
    {
        public override string Name { get; } = "Queen";
        public override PieceTypes Type { get; } = PieceTypes.QUEEN;
        public override char Short { get; } = 'q';
        public override int Rank { get { return 2; } }

        private readonly RookMover _rookMover = new();
        private readonly BishopMover _bishopMover = new();
        public Queen(int x, int y, bool side) : this(BitUtil.CoordToBit(x, y), side) { }

        public Queen(String algebraic, bool side) : this(BitUtil.AlgebraicToBit(algebraic), side) { }

        public Queen(ulong bit, bool side) : base(bit, side)
        {
            _rookMover.Side = side;
            _bishopMover.Side = side;
        }

        public override ulong MoveMask(Board board)
        {
            return _rookMover.MoveMask(Index, board) | _bishopMover.MoveMask(Index, board);
        }

        public override ulong AttackMask(Board b)
        {
            return _rookMover.AttackMask(Index, b) | _bishopMover.AttackMask(Index, b);
        }

        public override ulong XRayAttacks(Board board)
        {
            return _rookMover.XRayAttacks(Index, board) | _bishopMover.XRayAttacks(Index, board);
        }

        public override ulong PathBetween(Board b, int i)
        {
            return _rookMover.PathBetween(Index, i, b) | _bishopMover.PathBetween(Index, i, b);
        }
        public ulong EmptyMask()
        {
            return _rookMover.EmptyMask(Index) | _bishopMover.EmptyMask(Index);
        }
    }
}