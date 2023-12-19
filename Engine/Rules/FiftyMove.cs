namespace Engine.Rules
{
    public class FiftyMove : IRule
    {
        public int Clock { get; set; }
        private FastStack<int> _clockStack = new FastStack<int>(128);
        public bool DrawAvailable { get; private set; } = false;
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
            if (m.Capture() || _board.Pieces[pieceIndex].Type == PieceTypes.PAWN)
            {
                _clockStack.Push(Clock);
                Clock = 0;
                m.ClockReset = true;
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
                DrawAvailable = true;
            }

            return m;
        }

        public void ReverseMove(Move m, int _pieceIndex)
        {
            if (m.ClockReset)
            {
                Clock = _clockStack.Pop();
            }
            else
            {
                Clock--;
            }

            if (Clock <= 100)
                DrawAvailable = false;
        }

        public void Save()
        {
            _clockStack.Clear();
        }
    }
}
