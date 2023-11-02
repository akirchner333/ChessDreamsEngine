using Engine.Pieces.Movers;
using System;

namespace Engine
{
    public class King : Piece
    {
        public override string Name { get; } = "King";
        public override PieceTypes Type { get; } = PieceTypes.KING;
        public override char Short { get; } = 'k';

        private Leaper _leaper = new Leaper(new int[8] { 7, -7, 1, -1, 8, -8, 9, -9 }, "King");

        public King(int x, int y, bool side) : base(x, y, side)
        {
            _leaper.Side = side;
        }

        public King(string square, bool side) : base(square, side)
        {
            _leaper.Side = side;
        }
        public King(ulong bit, bool side) : base(bit, side)
        {
            _leaper.Side = side;
        }

        public override ulong MoveMask(Board b)
        {
            return _leaper.MoveMask(Index, b);
        }

        public override List<Move> Moves(Board b)
        {
            var result = new List<Move>();

            result.AddRange(base.Moves(b));
            result.AddRange(CastleMoves(b));

            return result;
        }

        // I am hardcoding a lot of castling stuff for now since castling is a weird move
        // We are assuming the king starts at E1/E8, and the Rooks are in the corners
        // Kingside castles move the King from E to G, Rook from H to F
        // Queenside castles move the King from E to C, Rook from A to D
        public List<Move> CastleMoves(Board board)
        {
            List<Move> list = new List<Move>();

            if (board.Attacked(Side, Position))
                return list;
            
            if (Side)
            {
                if (CastleLegal(Castles.WhiteKingside, board))
                    list.Add(new CastleMove(Position, BitUtil.AlgebraicToBit("g1"), Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("h1"),
                        RookEnd = BitUtil.AlgebraicToBit("f1")
                    });
                if (CastleLegal(Castles.WhiteQueenside, board))
                    list.Add(new CastleMove(Position, BitUtil.AlgebraicToBit("c1"), Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("a1"),
                        RookEnd = BitUtil.AlgebraicToBit("d1")
                    });
            }
            else
            {
                if (CastleLegal(Castles.BlackKingside, board))
                    list.Add(new CastleMove(Position, BitUtil.AlgebraicToBit("g8"), Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("h8"),
                        RookEnd = BitUtil.AlgebraicToBit("f8")
                    });
                if (CastleLegal(Castles.BlackQueenside, board))
                    list.Add(new CastleMove(Position, BitUtil.AlgebraicToBit("c8"), Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("a8"),
                        RookEnd = BitUtil.AlgebraicToBit("d8")
                    });
            }
            return list;
        }


        ulong whiteKingside =  0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_01100000;
        ulong whiteQueenside = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001110;
        ulong blackKingside =  0b01100000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        ulong blackQueenside = 0b00001110_00000000_00000000_00000000_00000000_00000000_00000000_00000000;

        ulong whiteQueenMove = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001100;
        ulong blackQueenMove = 0b00001100_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
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

            return b.HasCastleRights(castle) && !BitUtil.Overlap(b.AllPieces, emptyMask) && !attacked;
        }
    }
}

