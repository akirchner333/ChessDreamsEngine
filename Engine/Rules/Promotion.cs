namespace Engine.Rules
{
    public class Promotion : IRule
    {
        private readonly Board _board;
        public Promotion(Board board)
        {
            _board = board;
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            if (move.Promoting())
            {
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
                _board.Pieces[pieceIndex] = Board.NewPiece(move.End, move.Promotion, move.Side);
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
            }

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            if (move.Promoting())
            {
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
                _board.Pieces[pieceIndex] = Board.NewPiece(move.Start, PieceTypes.PAWN, move.Side);
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
            }
        }
    }
}
