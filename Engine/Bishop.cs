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
            return RiderMoves(7, Right | Bottom, board) |
                RiderMoves(-7, Left | Top, board) |
                RiderMoves(9, Left | Bottom, board) |
                RiderMoves(-9, Right | Top, board);
        }
	}
}