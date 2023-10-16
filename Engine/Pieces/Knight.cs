using System;
using System.Xml.Linq;

namespace Engine
{
    public class Knight : Leaper
    {
        private static ulong[]? _allMoves;

        public Knight(int x, int y, bool side) : base(x, y, side)
        {
            _name = "Knight";
            Type = PieceTypes.KNIGHT;
            if (_allMoves == null)
                _allMoves = FillMoveDatabase();
        }

        public Knight(String algebraic, bool side) : base(algebraic, side)
        {
            _name = "Pawn";
            Type = PieceTypes.PAWN;
            if (_allMoves == null)
                _allMoves = FillMoveDatabase();
        }

        public override ulong MovesAtIndex(ulong index)
        {
            return LeaperMask(15, Board.Columns["H"], index) |
                   LeaperMask(-15, Board.Columns["A"], index) |
                   LeaperMask(17, Board.Columns["A"], index) |
                   LeaperMask(-17, Board.Columns["H"], index) |
                   LeaperMask(6, Board.Columns["H"] | Board.Columns["G"], index) |
                   LeaperMask(-6, Board.Columns["A"] | Board.Columns["B"], index) |
                   LeaperMask(10, Board.Columns["A"] | Board.Columns["B"], index) |
                   LeaperMask(-10, Board.Columns["H"] | Board.Columns["G"], index);
        }

        public override ulong GetMoveDatabase(int index)
        {
            return _allMoves![index];
        }
    }
}

