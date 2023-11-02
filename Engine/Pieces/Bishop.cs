using System;

namespace Engine
{
	public class Bishop : Piece
	{
        public override string Name { get; } = "Bishop";
        public override PieceTypes Type { get; } = PieceTypes.BISHOP;
        public override char Short { get; } = 'b';
        public Bishop(int x, int y, bool side) : base(x, y, side)
        { }

        public Bishop(String algebraic, bool side) : base(algebraic, side)
        { }

        public Bishop(ulong bit, bool side) : base(bit, side)
        { }

        public override ulong MoveMask(Board board)
		{
            return RiderMoves(7, Board.Columns["H"], board) |
                RiderMoves(-7, Board.Columns["A"], board) |
                RiderMoves(9, Board.Columns["A"], board) |
                RiderMoves(-9, Board.Columns["H"], board);
        }
	}
}