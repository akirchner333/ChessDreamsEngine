namespace Engine.Rules
{
    public class Capture
    {
        private Board _board;
        private int[] _captures;
        private int _pointer = 0;

        public Capture(Board board)
        {
            _board = board;
            _captures = new int[_board.Pieces.Length];
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            if (!move.Capture)
                return move;

            var targetIndex = _board.FindPieceIndex(move.TargetSquare());
            if (targetIndex == -1)
            {
                throw new ArgumentException($"Incorrect capture while trying to play {move} on {_board.Fen()} {_board.BlackPieces} {_board.WhitePieces}");
            }

            _captures[_pointer++] = targetIndex;
            var piece = _board.Pieces[targetIndex];

            _board.RemoveSquare(move.TargetSquare(), piece.Side);
            _board.Move.TogglePiece(piece);
            piece.Captured = true;

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            if (!move.Capture)
                return;

            var piece = _board.Pieces[_captures[--_pointer]];
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
