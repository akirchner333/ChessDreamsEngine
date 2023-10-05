using System;

namespace Engine
{
    public class Queen : Piece
    {
        public Queen(int x, int y, bool side) : base(x, y, side)
        {
            _name = "Queen";
            Type = PieceTypes.QUEEN;
        }

        public Queen(String algebraic, bool side) : base(algebraic, side)
        {
            _name = "Pawn";
            Type = PieceTypes.PAWN;
        }

        public override ulong MoveMask(Board board)
        {
            return RiderMoves(7, Right | Bottom, board) |
                RiderMoves(-7, Left | Top, board) |
                RiderMoves(9, Left | Bottom, board) |
                RiderMoves(-9, Right | Top, board) |
                RiderMoves(1, Left, board) |
                RiderMoves(-1, Right, board) |
                RiderMoves(8, Bottom, board) |
                RiderMoves(-8, Top, board);
        }
    }
}