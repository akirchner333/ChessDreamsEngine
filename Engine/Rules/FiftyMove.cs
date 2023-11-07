using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rules
{
    public class FiftyMove
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

        public Move ApplyMove(Move m, Piece p)
        {
            m.HalfMoves = Clock;
            if(m.Capture || p.Type == PieceTypes.PAWN)
            {
                Clock = 0;
            }
            else
            {
                Clock++;
            }

            if(Clock >= 150)
            {
                _board.State = GameState.DRAW;
            }
            else if(Clock >= 100)
            {
                _board.drawAvailable = true;
            }

            return m;
        }

        public void ReverseMove(Move m)
        {
            Clock = m.HalfMoves;
            if (Clock <= 100)
                _board.drawAvailable = false;
        }
    }
}
