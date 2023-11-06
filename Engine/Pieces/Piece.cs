using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace Engine
{

    public interface IPiece
    {
        string Name { get; }
        PieceTypes Type { get; }
        char Short { get; }
    }
    public abstract class Piece: IPiece
    {
        public abstract string Name { get; }
        public abstract PieceTypes Type { get; }
        public abstract char Short { get; }

        public ulong Position { get; set; }
        public int Index { get; set; }
        public bool Side;
        public bool Captured { get; set; } = false;

        public Piece(int x, int y, bool side)
        {
            Side = side;
            Index = y * 8 + x;
            Position = 1ul << Index;
        }

        public Piece(string square, bool side)
        {
            Side = side;
            Index = BitUtil.AlgebraicToIndex(square);
            Position = 1ul << Index;
        }

        public Piece(ulong bit, bool side)
        {
            Side = side;
            Position = bit;
            Index = BitUtil.BitToIndex(bit);
        }

        public string PieceSprite()
        {
            return (Side ? "White" : "Black") + Name;
        }

        public abstract ulong MoveMask(Board b);

        public virtual ulong AttackMask(Board b)
        {
            return MoveMask(b);
        }

        protected ulong allyPieces(Board b)
        {
            return Side ? b.WhitePieces : b.BlackPieces;
        }

        protected ulong enemyPieces(Board b)
        {
            return Side ? b.BlackPieces : b.WhitePieces;
        }

        //This is a bit of an over-complicated way to get around C#'s inability to shift by negative values
        protected Func<int, ulong> ShiftPosition(int steps)
        {
            return Shifter(steps, Position);
        }

        protected static Func<int, ulong> Shifter(int steps, ulong start)
        {
            return steps > 0 ?
                (x) => start << (x * steps) :
                (x) => start >> (x * Math.Abs(steps));
        }

        //Takes a ulong and breaks it up into a bunch of Move objects, one for each bit
        public virtual List<Move> ConvertMask(Board b)
        {
            var moves = new List<Move>();
            var mask = MoveMask(b);
            foreach(var bit in BitUtil.SplitBits(mask))
                moves.Add(new Move(Position, bit, Side) { Capture = BitUtil.Overlap(bit, b.AllPieces) });

            return moves;
        }

        // In many cases just breaking up the mask (am I using that term correctly?) will be enough
        // But for stuff like en passant or castling, we'll need to extend this method to add in some extra stuff
        public virtual List<Move> Moves(Board b)
        {
            return ConvertMask(b);
        }

        public virtual Move ApplyMove(Move m)
        {
            Position = m.End;
            Index = BitUtil.BitToIndex(m.End);
            return m;
        }

        public virtual Move ReverseMove(Move m)
        {
            Position = m.Start;
            Index = BitUtil.BitToIndex(m.Start);
            return m;
        }
    }
}