using Engine.Pieces.Movers;

namespace Engine
{
    public class King : Piece
    {
        public override string Name { get; } = "King";
        public override PieceTypes Type { get; } = PieceTypes.KING;
        public override char Short { get; } = 'k';

        private Leaper _leaper = new Leaper(new int[8] { 7, -7, 1, -1, 8, -8, 9, -9 }, "King");
        private CastleMover _castler = new CastleMover();

        public King(int x, int y, bool side) : base(x, y, side)
        {
            _leaper.Side = side;
            _castler.Side = side;
        }

        public King(string square, bool side) : base(square, side)
        {
            _leaper.Side = side;
            _castler.Side = side;
        }
        public King(ulong bit, bool side) : base(bit, side)
        {
            _leaper.Side = side;
            _castler.Side = side;
        }

        public override ulong MoveMask(Board b)
        {
            return _leaper.MoveMask(Index, b);
        }

        public override Move[] Moves(Board b)
        {
            //This is probably not the fastest way to do this
            return base.Moves(b).Concat(_castler.Moves(b, Position)).ToArray();
        }


    }
}

