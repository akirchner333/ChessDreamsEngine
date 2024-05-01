namespace Engine.Rules
{
    // Handles 3 and 5 fold repetition
    // https://en.wikipedia.org/wiki/Threefold_repetition
    public class Repetition
    {
        private Board _board;
        private FastStack<ulong> _positions = new FastStack<ulong>(160);
        public bool DrawAvailable { get; private set; } = false;
        // Make repeat count available so I can punish naughty AIs for returning to the same position
        public int RepeatCount = 0;
        public Repetition(Board board)
        {
            _board = board;
            _positions.Push(_board.Hash);
        }

        public Move ApplyMove(Move move, int pieceIndex)
        {
            //try
            //{
                _positions.Push(_board.Hash);
            //}
            //catch
            //{
            //    throw new Exception($"Repetition push issue, {_positions.Count()}, {_board.Clock.Clock}");
            //}
            
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
            RepeatCount = _positions.CountValues(_board.Hash);
            if (RepeatCount >= 5)
            {
                _board.State = GameState.DRAW;
            }
            else if (RepeatCount >= 3)
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
                var currentPosition = _positions.Pop();
                _positions.Clear();
                _positions.Push(currentPosition);
            }
        }

        public int StoredPositions()
        {
            return _positions.Count();
        }
    }
}
