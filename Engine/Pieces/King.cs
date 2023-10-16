using System;

namespace Engine
{
    public class King : Leaper
    {
        private static ulong[]? _allMoves;
        public King(int x, int y, bool side) : base(x, y, side)
        {
            _name = "King";
            Type = PieceTypes.KING;
            if (_allMoves == null)
                _allMoves = FillMoveDatabase();
        }

        public King(string square, bool side) : base(square, side)
        {
            _name = "King";
            Type = PieceTypes.KING;
            if (_allMoves == null)
                _allMoves = FillMoveDatabase();
        }

        public override ulong MovesAtIndex(ulong index)
        {
            return LeaperMask(7, Board.Columns["H"], index) |
                   LeaperMask(-7, Board.Columns["A"], index) |
                   LeaperMask(9, Board.Columns["A"], index) |
                   LeaperMask(-9, Board.Columns["H"], index) |
                   LeaperMask(1, Board.Columns["A"], index) |
                   LeaperMask(-1, Board.Columns["H"], index) |
                   LeaperMask(8, 0, index) |
                   LeaperMask(-8, 0, index);
        }

        public override ulong GetMoveDatabase(int index)
        {
            return _allMoves![index];
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
                    list.Add(new CastleMove(Position, BitUtil.AlgebraicToBit("G1"), Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("H1"),
                        RookEnd = BitUtil.AlgebraicToBit("F1")
                    });
                if (CastleLegal(Castles.WhiteQueenside, board))
                    list.Add(new CastleMove(Position, BitUtil.AlgebraicToBit("C1"), Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("A1"),
                        RookEnd = BitUtil.AlgebraicToBit("D1")
                    });
            }
            else
            {
                if (CastleLegal(Castles.BlackKingside, board))
                    list.Add(new CastleMove(Position, BitUtil.AlgebraicToBit("G8"), Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("H8"),
                        RookEnd = BitUtil.AlgebraicToBit("F8")
                    });
                if (CastleLegal(Castles.BlackQueenside, board))
                    list.Add(new CastleMove(Position, BitUtil.AlgebraicToBit("C8"), Side)
                    {
                        RookStart = BitUtil.AlgebraicToBit("A8"),
                        RookEnd = BitUtil.AlgebraicToBit("D8")
                    });
            }
            return list;
        }


        ulong whiteKingside =  0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_01100000;
        ulong whiteQueenside = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001110;
        ulong blackKingside =  0b01100000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        ulong blackQueenside = 0b00001110_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        public bool CastleLegal(Castles castle, Board b)
        {
            var mask = castle switch
            {
                Castles.BlackKingside => blackKingside,
                Castles.BlackQueenside => blackQueenside,
                Castles.WhiteQueenside => whiteQueenside,
                Castles.WhiteKingside => whiteKingside,
                _ => 0ul
            };

            var attacked = BitUtil.SplitBits(mask).Any(x => b.Attacked(Side, x));

            return b.HasCastleRights(castle) && !BitUtil.Overlap(b.AllPieces, mask) && !attacked;
        }
    }
}

