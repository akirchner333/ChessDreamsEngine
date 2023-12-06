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

            move.TargetListIndex = _board.FindPieceIndex(move.TargetSquare());
            if(move.TargetListIndex == -1)
            {
                throw new ArgumentException($"Incorrect capture while trying to play {move} on {_board.Fen()} {_board.BlackPieces} {_board.WhitePieces}");
            }
            var piece = _board.Pieces[move.TargetListIndex];

            _board.RemoveSquare(move.TargetSquare(), piece.Side);
            _board.Move.TogglePiece(piece);
            piece.Captured = true;

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            if (!move.Capture)
                return;

            var piece = _board.Pieces[move.TargetListIndex];
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
