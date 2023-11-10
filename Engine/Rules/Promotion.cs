using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rules
{
    public class Promotion : IRule
    {
        private Board _board;
        public Promotion(Board board)
        {
            _board = board;
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            if (move.Promoting)
            {
                _board.Pieces[pieceIndex] = _board.NewPiece(move.End, ((PromotionMove)move).Promotion, move.Side);
            }

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            if (move.Promoting)
            {
                _board.Pieces[pieceIndex] = _board.NewPiece(move.Start, PieceTypes.PAWN, move.Side);
            }
        }
    }
}
