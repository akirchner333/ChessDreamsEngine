using Engine.Pieces.Movers;
using Engine.Rules;

namespace Engine
{
    public class Rook : Piece
    {
        public override string Name { get; } = "Rook";
        public override PieceTypes Type { get; } = PieceTypes.ROOK;
        public override char Short { get; } = 'r';
        public override int Rank { get { return 1; } }

        private readonly RookMover _mover = new();
        public Rook(String algebraic, bool side) : this(BitUtil.AlgebraicToBit(algebraic), side) { }

        public Rook(ulong bit, bool side) : base(bit, side)
        {
            _mover.Side = side;
            CastleRights = BitUtil.BitToX(bit) < 4 ? (int)Castles.WhiteQueenside : (int)Castles.WhiteKingside;
            if (!side)
            {
                CastleRights <<= 2;
            }
        }

        public override ulong MoveMask(Board board)
        {
            return _mover.MoveMask(Index, board);
        }

        public override Move ApplyMove(Move m)
        {
            if (m is CastleMove)
            {
                Position = ((CastleMove)m).RookEnd;
                Index = BitUtil.BitToIndex(((CastleMove)m).RookEnd);
                return m;
            }

            return base.ApplyMove(m);
        }

        public override Move ReverseMove(Move m)
        {
            if (m is CastleMove)
            {
                Position = ((CastleMove)m).RookStart;
                Index = BitUtil.BitToIndex(((CastleMove)m).RookStart);
                return m;
            }

            return base.ReverseMove(m);
        }
    }
}