using Engine.Rules;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Pieces.Movers
{
    public class CastleMover
    {
        public bool Side {  get; set; }
        public CastleMover() { }

        public const ulong c1 = 4ul;
        public const ulong g1 = 64ul;
        public const ulong c8 = 288230376151711744;
        public const ulong g8 = 4611686018427387904;

        public const ulong whiteKingside = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_01100000;
        public const ulong whiteQueenside = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001110;
        public const ulong blackKingside = 0b01100000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        public const ulong blackQueenside = 0b00001110_00000000_00000000_00000000_00000000_00000000_00000000_00000000;

        public const ulong whiteQueenMove = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001100;
        public const ulong blackQueenMove = 0b00001100_00000000_00000000_00000000_00000000_00000000_00000000_00000000;

        // I am hardcoding a lot of castling stuff for now since castling is a weird move
        // We are assuming the king starts at E1/E8, and the Rooks are in the corners
        // Kingside castles move the King from E to G, Rook from H to F
        // Queenside castles move the King from E to C, Rook from A to D
        public List<Move> Moves(Board board, ulong Position)
        {
            List<Move> list = new List<Move>();

            if (board.Attacked(Side, Position))
                return list;

            if (Side)
            {
                if (CastleLegal(Castles.WhiteKingside, board))
                    list.Add(new CastleMove(Position, g1, Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("h1"),
                        RookEnd = BitUtil.AlgebraicToBit("f1")
                    });
                if (CastleLegal(Castles.WhiteQueenside, board))
                    list.Add(new CastleMove(Position, c1, Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("a1"),
                        RookEnd = BitUtil.AlgebraicToBit("d1")
                    });
            }
            else
            {
                if (CastleLegal(Castles.BlackKingside, board))
                    list.Add(new CastleMove(Position, g8, Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("h8"),
                        RookEnd = BitUtil.AlgebraicToBit("f8")
                    });
                if (CastleLegal(Castles.BlackQueenside, board))
                    list.Add(new CastleMove(Position, c8, Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("a8"),
                        RookEnd = BitUtil.AlgebraicToBit("d8")
                    });
            }
            return list;
        }

        public bool CastleLegal(Castles castle, Board b)
        {
            // The squares that the kill will move through - none of them can be attacked.
            // The same as empty squares for kingside but different for queenside
            var moveThrough = castle switch
            {
                Castles.BlackKingside => blackKingside,
                Castles.BlackQueenside => blackQueenMove,
                Castles.WhiteQueenside => whiteQueenMove,
                Castles.WhiteKingside => whiteKingside,
                _ => 0ul
            };
            var attacked = BitUtil.SplitBits(moveThrough).Any(x => b.Attacked(Side, x));

            //The squares which must be empty
            var emptyMask = castle switch
            {
                Castles.BlackKingside => blackKingside,
                Castles.BlackQueenside => blackQueenside,
                Castles.WhiteQueenside => whiteQueenside,
                Castles.WhiteKingside => whiteKingside,
                _ => 0ul
            };

            return b.Castles.HasCastleRights(castle) && !BitUtil.Overlap(b.AllPieces, emptyMask) && !attacked;
        }
    }
}
