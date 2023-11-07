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
        public Board Board { get; private set; }

        public EnPassant(Board board)
        {
            Board = board;
        }

        public EnPassant(string[] fenParts, Board board)
        {
            Board = board;
            var passant = fenParts[3];
            if (passant != "-")
            {
                PassantSquare = BitUtil.AlgebraicToBit(passant);
                TargetSquare = Board.Turn ? PassantSquare >> 8 : PassantSquare << 8;
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
            m.PassantTarget = TargetSquare;
            m.PassantSquare = PassantSquare;

            if (p.Type == PieceTypes.PAWN && (m.Start >> 16 == m.End || m.Start << 16 == m.End))
            {
                TargetSquare = m.End;
                PassantSquare = Board.Turn ? m.Start << 8 : m.Start >> 8;
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
            TargetSquare = m.PassantTarget;
            PassantSquare = m.PassantSquare;
        }


        // Provide a mask of squares this piece can attack
        // Returns true if that mask overlaps with the passant square
        public bool Attack(ulong mask)
        {
            return BitUtil.Overlap(PassantSquare, mask);
        }
    }
}
