using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rules
{
    public class Capture
    {
        private Board _board;

        public Capture(Board board)
        {
            _board = board;
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            if (!move.Capture)
                return move;

            move.TargetIndex = _board.FindPieceIndex(move.TargetSquare());
            var piece = _board.Pieces[move.TargetIndex];

            _board.RemoveSquare(move.TargetSquare(), piece.Side);
            _board.Move.TogglePiece(piece);
            piece.Captured = true;

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            if (!move.Capture)
                return;

            var piece = _board.Pieces[move.TargetIndex];
            piece.Captured = false;
            _board.Move.TogglePiece(piece);
            if (!move.Side)
                _board.WhitePieces |= piece.Position;
            else
                _board.BlackPieces |= piece.Position;
            _board.AllPieces = _board.WhitePieces | _board.BlackPieces;
        }
    }
}
