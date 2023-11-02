using System;

namespace Engine
{
    public class Queen : Piece
    {
        public override string Name { get; } = "Queen";
        public override PieceTypes Type { get; } = PieceTypes.QUEEN;
        public override char Short { get; } = 'q';
        public Queen(int x, int y, bool side) : base(x, y, side)
        {}

        public Queen(String algebraic, bool side) : base(algebraic, side)
        {}

        public Queen(ulong bit, bool side) : base(bit, side)
        {}

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