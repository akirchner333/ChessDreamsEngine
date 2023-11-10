using Engine.Pieces.Movers;
using Engine.Pieces.MoveFactories;
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
        protected MoveFactory _convert { get; set; } = new MoveFactory();

        public Piece(int x, int y, bool side)
        {
            Side = side;
            Index = BitUtil.CoordToIndex(x, y);
            Position = 1ul << Index;
            _convert.Side = side;
        }

        public Piece(string square, bool side)
        {
            Side = side;
            Index = BitUtil.AlgebraicToIndex(square);
            Position = 1ul << Index;
            _convert.Side = side;
        }

        public Piece(ulong bit, bool side)
        {
            Side = side;
            Position = bit;
            Index = BitUtil.BitToIndex(bit);
            _convert.Side = side;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ INFO ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public string PieceSprite()
        {
            return (Side ? "White" : "Black") + Name;
        }

        protected ulong allyPieces(Board b)
        {
            return Side ? b.WhitePieces : b.BlackPieces;
        }

        protected ulong enemyPieces(Board b)
        {
            return Side ? b.BlackPieces : b.WhitePieces;
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ Move Generation ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        public abstract ulong MoveMask(Board b);

        public virtual ulong AttackMask(Board b)
        {
            return MoveMask(b);
        }

        // In many cases just breaking up the mask (am I using that term correctly?) will be enough
        // But for stuff like en passant or castling, we'll need to extend this method to add in some extra stuff
        public virtual Move[] Moves(Board b)
        {
            var mask = MoveMask(b);
            return _convert.ConvertMask(b, Position, mask);
        }

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ MOVE APPLICATION ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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