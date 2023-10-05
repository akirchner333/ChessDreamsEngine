using System;
using System.Xml.Linq;

namespace Engine
{
    public class Knight : Piece
    {
        public Knight(int x, int y, bool side) : base(x, y, side)
        {
            _name = "Knight";
            Type = PieceTypes.KNIGHT;
        }

        public Knight(String algebraic, bool side) : base(algebraic, side)
        {
            _name = "Pawn";
            Type = PieceTypes.PAWN;
        }

        public override ulong MoveMask(Board board)
        {
            return LeaperMoves(15, Board.Columns["H"] | Board.Rows[0] | Board.Rows[1], board) |
                   LeaperMoves(-15, Board.Columns["A"] | Board.Rows[6] | Board.Rows[7], board) |
                   LeaperMoves(17, Board.Columns["A"] | Board.Rows[0] | Board.Rows[1], board) |
                   LeaperMoves(-17, Board.Columns["H"] | Board.Rows[6] | Board.Rows[7], board) |
                   LeaperMoves(6, Board.Columns["H"] | Board.Columns["G"] | Board.Rows[0], board) |
                   LeaperMoves(-6, Board.Columns["A"] | Board.Columns["B"] | Board.Rows[7], board) |
                   LeaperMoves(10, Board.Columns["A"] | Board.Columns["B"] | Board.Rows[0], board) |
                   LeaperMoves(-10, Board.Columns["H"] | Board.Columns["G"] | Board.Rows[7], board);
        }
    }
}

