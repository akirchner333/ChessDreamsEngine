using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Rules
{
    public class EnPassant
    {
        public ulong PassantSquare {  get; private set; } = 0;
        public ulong TargetSquare { get; private set; } = 0;
        private Board _board;
        private static ulong[] _values;

        static EnPassant()
        {
            _values = Rand.RandULongArray(8);
        }

        public EnPassant(Board board)
        {
            _board = board;
        }

        public EnPassant(string[] fenParts, Board board)
        {
            _board = board;

            var passant = fenParts[3];
            if (passant != "-")
            {
                PassantSquare = BitUtil.AlgebraicToBit(passant);
                TargetSquare = _board.Turn ? PassantSquare >> 8 : PassantSquare << 8;
                _board.Hash ^= _values[HashIndex()];
            }
        }

        public string Fen()
        {
            if (PassantSquare == 0)
                return "-";

            return BitUtil.BitToAlgebraic((ulong)PassantSquare);
        }
        public Move ApplyMove(Move m, Piece p)
        {
            //This is awkward. Need some manner of handling hash of no-passant elegantly
            if (PassantSquare != 0)
            {
                _board.Hash ^= _values[HashIndex()];
            }
            m.PassantTarget = TargetSquare;
            m.PassantSquare = PassantSquare;

            if (p.Type == PieceTypes.PAWN && (m.Start >> 16 == m.End || m.Start << 16 == m.End))
            {
                TargetSquare = m.End;
                PassantSquare = _board.Turn ? m.Start << 8 : m.Start >> 8;
                _board.Hash ^= _values[HashIndex()];
            }
            else
            {
                TargetSquare = 0;
                PassantSquare = 0;
            }

            return m;
        }

        public void ReverseMove(Move m)
        {
            if(PassantSquare != 0)
            {
                _board.Hash ^= _values[HashIndex()];
            }
            
            TargetSquare = m.PassantTarget;
            PassantSquare = m.PassantSquare;
            if (PassantSquare != 0)
            {
                _board.Hash ^= _values[HashIndex()];
            }
        }


        // Provide a mask of squares this piece can attack
        // Returns true if that mask overlaps with the passant square
        public bool Attack(ulong mask)
        {
            return BitUtil.Overlap(PassantSquare, mask);
        }

        public int HashIndex()
        {
            return BitUtil.BitToX(PassantSquare);
        }
    }
}
