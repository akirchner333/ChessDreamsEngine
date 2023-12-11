namespace Engine.Rules
{
    public class Repetition
    {
        private Board _board;
        private FastStack<ulong> _positions = new FastStack<ulong>(160);
        public bool DrawAvailable { get; private set; } = false;
        public Repetition(Board board)
        {
            _board = board;
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            _positions.Push(_board.Hash);
            CheckDraw();
            return move;
        }

        public void ReverseMove(Move move)
        {
            _positions.Pop();
            CheckDraw();
        }

        public void CheckDraw()
        {
            var repetitions = _positions.CountValues(_board.Hash);
            if (repetitions >= 5)
            {
                _board.State = GameState.DRAW;
            }
            else if (repetitions >= 3)
            {
                DrawAvailable = true;
            }
            else
            {
                // I'm not 100% sure but my reading of the rules is that if a player does not claim a draw during a repeated move
                // They can't claim a draw later if the board returns to a non-repeated state
                DrawAvailable = false;
            }
        }

        public void Save()
        {
            if (_board.Clock.Clock == 0)
            {
                _positions.Clear();
            }
        }
    }
}
