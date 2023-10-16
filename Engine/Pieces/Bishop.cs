using System;

namespace Engine
{
	public class Bishop : Piece
	{
		public Bishop(int x, int y, bool side) : base(x, y, side)
        {
			_name = "Bishop";
			Type = PieceTypes.BISHOP;
		}

        public Bishop(String algebraic, bool side) : base(algebraic, side)
        {
            _name = "Pawn";
            Type = PieceTypes.PAWN;
        }

        public override ulong MoveMask(Board board)
		{
            return RiderMoves(7, Board.Columns["H"], board) |
                RiderMoves(-7, Board.Columns["A"], board) |
                RiderMoves(9, Board.Columns["A"], board) |
                RiderMoves(-9, Board.Columns["H"], board);
        }
	}
}