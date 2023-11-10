using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rules
{
    public enum Castles
    {
        WhiteKingside =  0b0001,
        WhiteQueenside = 0b0010,
        BlackKingside =  0b0100,
        BlackQueenside = 0b1000
    }

    public class Castling
    {
        public int CastleRights { get; set; } = 0b1111;
        private Board _board;

        public Castling(Board board)
        {
            _board = board;
        }

        public Castling(string[] fenParts, Board board)
        {
            _board = board;
            CastleRights = 0;
            if (fenParts[2] != "-")
            {
                if (fenParts[2].Contains('K'))
                    CastleRights |= (int)Castles.WhiteKingside;
                if (fenParts[2].Contains('Q'))
                    CastleRights |= (int)Castles.WhiteQueenside;
                if (fenParts[2].Contains('k'))
                    CastleRights |= (int)Castles.BlackKingside;
                if (fenParts[2].Contains('q'))
                    CastleRights |= (int)Castles.BlackQueenside;
            }
        }
        public string Fen()
        {
            if (CastleRights == 0)
                return "-";
            else
            {
                var fen = "";
                if ((CastleRights & (int)Castles.WhiteKingside) > 0)
                    fen += "K";
                if ((CastleRights & (int)Castles.WhiteQueenside) > 0)
                    fen += "Q";
                if ((CastleRights & (int)Castles.BlackKingside) > 0)
                    fen += "k";
                if ((CastleRights & (int)Castles.BlackQueenside) > 0)
                    fen += "q";
                return fen;
            }
        }

        public Boolean HasCastleRights(Castles c)
        {
            return (CastleRights & (int)c) != 0;
        }

        public Move ApplyMove(Move m, Piece p)
        {
            if (m is CastleMove)
            {
                var rook = _board.FindPiece(((CastleMove)m).RookStart);
                if (rook != null)
                    m = _board.MovePiece(rook, ((CastleMove)m).RookStart, ((CastleMove)m).RookEnd, m);
            }

            m.Castles = CastleRights;
            int effectedCastles = 0;
            if (p.Type == PieceTypes.ROOK)
            {
                if (BitUtil.Overlap(m.Start, Board.Columns["A"]))
                    effectedCastles |= p.Side ? (int)Castles.WhiteQueenside : (int)Castles.BlackQueenside;
                else if (BitUtil.Overlap(m.Start, Board.Columns["H"]))
                    effectedCastles |= p.Side ? (int)Castles.WhiteKingside : (int)Castles.BlackKingside;
            }
            else if (p.Type == PieceTypes.KING)
            {
                if (p.Side)
                {
                    effectedCastles |= (int)Castles.WhiteKingside;
                    effectedCastles |= (int)Castles.WhiteQueenside;
                }
                else
                {
                    effectedCastles |= (int)Castles.BlackKingside;
                    effectedCastles |= (int)Castles.BlackQueenside;
                }
            }
            CastleRights = BitUtil.Remove(CastleRights, effectedCastles);

            return m;
        }

        public void ReverseMove(Move m)
        {
            if (m is CastleMove)
            {
                var rook = _board.FindPiece(((CastleMove)m).RookEnd);
                if (rook != null)
                    _board.MovePiece(rook, ((CastleMove)m).RookEnd, ((CastleMove)m).RookStart, m, true);
            }

            CastleRights = m.Castles;
        }

    }
}
