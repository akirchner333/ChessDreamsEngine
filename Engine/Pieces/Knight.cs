using Engine.Pieces.Movers;
using System;
using System.Xml.Linq;

namespace Engine
{
    public class Knight : Piece
    {
        public override string Name { get; } = "Knight";
        public override PieceTypes Type { get; } = PieceTypes.KNIGHT;
        public override char Short { get; } = 'n';

        private Leaper _leaper = new Leaper(new int[8]{15, -15, 17, -17, 6, -6, 10, -10}, "Knight");

        public Knight(int x, int y, bool side) : base(x, y, side)
        {
            _leaper.Side = side;
        }

        public Knight(String algebraic, bool side) : base(algebraic, side)
        {
            _leaper.Side = side;
        }

        public Knight(ulong bit, bool side) : base(bit, side)
        {
            _leaper.Side = side;
        }

        public override ulong MoveMask(Board b)
        {
            return _leaper.MoveMask(Index, b);
        }
    }
}

