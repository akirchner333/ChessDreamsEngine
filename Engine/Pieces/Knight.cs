using Engine.Pieces.Movers;

namespace Engine
{
    public class Knight : Piece
    {
        public override string Name { get; } = "Knight";
        public override PieceTypes Type { get; } = PieceTypes.KNIGHT;
        public override char Short { get; } = 'n';
        public override int Rank { get { return 5; } }

        private Leaper _leaper = new Leaper(new int[8] { 15, -15, 17, -17, 6, -6, 10, -10 }, "Knight");

        public Knight(String algebraic, bool side) : this(BitUtil.AlgebraicToBit(algebraic), side) { }

        public Knight(ulong bit, bool side) : base(bit, side)
        {
            _leaper.Side = side;
        }

        public override ulong MoveMask(Board b)
        {
            return _leaper.MoveMask(Index, b);
        }

        public override ulong Mask(ulong _occ)
        {
            return _leaper.RawMask(Index);
        }
    }
}

