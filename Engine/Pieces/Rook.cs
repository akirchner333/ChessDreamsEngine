using System;
using System.Collections.Generic;

namespace Engine
{
    public class Rook : Piece
    {
        public override string Name { get; } = "Rook";
        public override PieceTypes Type { get; } = PieceTypes.ROOK;
        public override char Short { get; } = 'r';
        public Rook(int x, int y, bool side) : base(x, y, side){ }

        public Rook(String algebraic, bool side) : base(algebraic, side) { }

        public Rook(ulong bit, bool side) : base(bit, side) { }

        public override ulong MoveMask(Board board)
        {
            return RiderMoves(1, Board.Columns["A"], board) | 
                RiderMoves(-1, Board.Columns["H"], board) |
                RiderMoves(8, 0, board) |
                RiderMoves(-8, 0, board);
        }

        public override Move ApplyMove(Move m)
        {
            if(m is CastleMove)
            {
                Position = ((CastleMove)m).RookEnd;
                Index = BitUtil.BitToIndex(((CastleMove)m).RookEnd);
                return m;
            }
            
            return base.ApplyMove(m);
        }

        public override Move ReverseMove(Move m)
        {
            if (m is CastleMove)
            {
                Position = ((CastleMove)m).RookStart;
                Index = BitUtil.BitToIndex(((CastleMove)m).RookStart);
                return m;
            }

            return base.ReverseMove(m);
        }
    }
}