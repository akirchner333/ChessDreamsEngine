namespace Engine.Rules
{
    public class Capture : IRule
    {
        private Board _board;
        private FastStack<int> _captures;

        public Capture(Board board)
        {
            _board = board;
            _captures = new FastStack<int>(_board.Pieces.Length);
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            if (!move.Capture())
                return move;

            var targetIndex = _board.FindPieceIndex(move.TargetSquare());
            if (targetIndex == -1)
            {
                throw new ArgumentException($"Incorrect capture while trying to play {move} on {_board.Fen()} {_board.BlackPieces} {_board.WhitePieces}");
            }

            _captures.Push(targetIndex);
            var piece = _board.Pieces[targetIndex];

            _board.RemoveSquare(move.TargetSquare(), piece.Side);
            _board.Move.TogglePiece(piece);
            piece.Captured = true;

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            if (!move.Capture())
                return;

            var piece = _board.Pieces[_captures.Pop()];
            piece.Captured = false;
            _board.Move.TogglePiece(piece);
            if (!move.Side)
                _board.WhitePieces |= piece.Position;
            else
                _board.BlackPieces |= piece.Position;
            _board.AllPieces = _board.WhitePieces | _board.BlackPieces;
        }

        // Probably don't do this. It's only in here so a castling test can use it
        public void AddCapture(int pieceIndex)
        {
            _captures.Push(pieceIndex);
        }

        public Piece LastCapture()
        {
            return _board.Pieces[_captures.Peek()];
        }

        public void Save()
        {
            _captures.Clear();
            //Possibly, we could remove captured pieces from the array in board here. Might be worthwhile
        }
    }
}
