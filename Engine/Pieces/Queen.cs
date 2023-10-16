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
            return RiderMoves(7, Board.Columns["H"], board) |
                RiderMoves(-7, Board.Columns["A"], board) |
                RiderMoves(9, Board.Columns["A"], board) |
                RiderMoves(-9, Board.Columns["H"], board) |
                RiderMoves(1, Board.Columns["A"], board) |
                RiderMoves(-1, Board.Columns["H"], board) |
                RiderMoves(8, 0, board) |
                RiderMoves(-8, 0, board);
        }
    }
}