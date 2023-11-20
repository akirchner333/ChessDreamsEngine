namespace Engine.Rules
{
    public class Repetition
    {
        private Board _board;
        private ulong[] _positions = new ulong[155];
        public bool DrawAvailable { get; private set; } = false;
        public Repetition(Board board)
        {
            _board = board;
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            if (_board.Clock.Clock == 0)
            {
                _positions = new ulong[155];

            }
            _positions[_board.Clock.Clock] = _board.Hash;
            move.PastPositions = _positions;
            CheckDraw();
            return move;
        }

        public void ReverseMove(Move move)
        {
            _positions = move.PastPositions;
            CheckDraw();
        }

        public void CheckDraw()
        {
            var repetitions = _positions.Count(p => p == _board.Hash);
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
    }
}
