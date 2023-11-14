using System.Numerics;

namespace Engine.Pieces.Movers
{
    // This is a Mover and a Factory, combined into one. Whatever! Rules are fake!
    public class PassantMover
    {
        public bool Side { get; set; }
        public PassantMover() { }

        // This calls the Passant rule in the board directly
        // So if I tried to put this mover into a game that didn't allow En Passant (i.e. didn't have that rule), it would fail
        // I think the solution to this is Events and messages - something to look into. Later.
        public ulong MoveMask(int index, Board b, Leaper attack)
        {
            return b.Passant.PassantSquare & attack.RawMask(index);
        }


        public Move[] ConvertMask(Board b, ulong start, ulong mask)
        {
            var moves = new Move[BitOperations.PopCount(mask)];
            BitUtil.SplitBitsNew(mask, (ulong bit, int i) =>
            {
                moves[i] = new PassantMove(start, b.Passant.PassantSquare, Side, b.Passant.TargetSquare);
            });

            return moves;
        }
    }
}
