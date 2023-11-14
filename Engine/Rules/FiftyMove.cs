namespace Engine.Rules
{
    public class FiftyMove : IRule
    {
        public int Clock { get; set; }
        private Board _board;

        public FiftyMove(Board board)
        {
            Clock = 0;
            _board = board;
        }

        public FiftyMove(string[] fenParts, Board board)
        {
            Clock = Int32.Parse(fenParts[4]);
            _board = board;
        }

        public string Fen()
        {
            return Clock.ToString();
        }

        public Move ApplyMove(Move m, int pieceIndex)
        {
            m.HalfMoves = Clock;
            if (m.Capture || _board.Pieces[pieceIndex].Type == PieceTypes.PAWN)
            {
                Clock = 0;
            }
            else
            {
                Clock++;
            }

            if (Clock >= 150)
            {
                _board.State = GameState.DRAW;
            }
            else if (Clock >= 100)
            {
                _board.drawAvailable = true;
            }

            return m;
        }

        public void ReverseMove(Move m, int _pieceIndex)
        {
            Clock = m.HalfMoves;
            if (Clock <= 100)
                _board.drawAvailable = false;
        }
    }
}
