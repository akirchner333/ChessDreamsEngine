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
            if (move is PromotionMove promo)
            {
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
                _board.Pieces[pieceIndex] = Board.NewPiece(promo.End, promo.Promotion, promo.Side);
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
            }

            return move;
        }

        public void ReverseMove(Move move, int pieceIndex)
        {
            if (move is PromotionMove)
            {
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
                _board.Pieces[pieceIndex] = Board.NewPiece(move.Start, PieceTypes.PAWN, move.Side);
                _board.Move.TogglePiece(_board.Pieces[pieceIndex]);
            }
        }
    }
}
