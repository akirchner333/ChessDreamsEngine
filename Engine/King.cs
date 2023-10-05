using System;

namespace Engine
{
    public class King : Piece
    {
        public King(int x, int y, bool side) : base(x, y, side)
        {
            _name = "King";
            Type = PieceTypes.KING;
        }

        public King(string square, bool side) : base(square, side)
        {
            _name = "King";
            Type = PieceTypes.KING;
        }

        public override ulong MoveMask(Board board)
        {
            return LeaperMoves(7, Right | Bottom, board) |
                   LeaperMoves(-7, Left | Top, board) |
                   LeaperMoves(9, Left | Bottom, board) |
                   LeaperMoves(-9, Right | Top, board) |
                   LeaperMoves(1, Left, board) |
                   LeaperMoves(-1, Right, board) |
                   LeaperMoves(8, Bottom, board) |
                   LeaperMoves(-8, Top, board);
        }

        public override List<Move> Moves(Board b)
        {
            var result = new List<Move>();

            result.AddRange(base.Moves(b));
            result.AddRange(CastleMoves(b));

            return result;
        }

        ulong whiteKingside =   0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_01100000;
        ulong whiteQueenside =  0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001110;
        ulong blackKingside =   0b01100000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        ulong blackQueenside =  0b00001110_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        public List<Move> CastleMoves(Board board)
        {
            List<Move> list = new List<Move>();
            
            //Need to check that 1. The king isn't in check, 2. None of the intervening squares are in check and 3. The final square isn't in check
            if (Side)
            {
                //How do I know where the knights are?
                //I'm hardcoding them for now
                if((board.CastleRights & (int)Castles.WhiteKingside) != 0 && (board.AllPieces & whiteKingside) == 0)
                    list.Add(new CastleMove(Position, Position >> 2, 1 << 7, Position >> 1, Side));
                if ((board.CastleRights & (int)Castles.WhiteQueenside) != 0 && (board.AllPieces & whiteQueenside) == 0)
                    list.Add(new CastleMove(Position, Position << 2, 1, Position << 1, Side));
            }
            else
            {
                if ((board.CastleRights & (int)Castles.BlackKingside) != 0 && (board.AllPieces & blackKingside) == 0)
                    list.Add(new CastleMove(Position, Position >> 2, (ulong)1 << 63, Position >> 1, Side));
                if ((board.CastleRights & (int)Castles.BlackQueenside) != 0 && (board.AllPieces & blackQueenside) == 0)
                    list.Add(new CastleMove(Position, Position << 2, (ulong)1 << 56, Position << 1, Side));
            }
            return list;
        }
    }
}

