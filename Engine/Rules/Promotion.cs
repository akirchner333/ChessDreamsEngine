﻿using System;
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
            if (move is PromotionMove promo)
            {
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
                _board.Pieces[pieceIndex] = _board.NewPiece(promo.End, promo.Promotion, promo.Side);
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
            }

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            if (move.Promoting)
            {
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
                _board.Pieces[pieceIndex] = _board.NewPiece(move.Start, PieceTypes.PAWN, move.Side);
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
            }
        }
    }
}
