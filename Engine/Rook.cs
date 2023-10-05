using System;
using System.Collections.Generic;

namespace Engine
{
    public class Rook : Piece
    {
        public Rook(int x, int y, bool side) : base(x, y, side)
        {
            _name = "Rook";
            Type = PieceTypes.ROOK;
        }

        public Rook(String algebraic, bool side) : base(algebraic, side)
        {
            _name = "Pawn";
            Type = PieceTypes.PAWN;
        }

        public override ulong MoveMask(Board board)
        {
            return RiderMoves(1, Left, board) | RiderMoves(-1, Right, board) | RiderMoves(8, Bottom, board) | RiderMoves(-8, Top, board);
        }
    }
}