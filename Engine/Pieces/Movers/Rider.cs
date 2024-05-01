namespace Engine.Pieces.Movers
{
    // A rider is a piece that moves an unlimited distance in direction until it hits
    // another piece or an edge
    // https://en.wikipedia.org/wiki/Fairy_chess_piece#Rider
    public interface IRiderCalc
    {
        ulong EmptyMask(int i);
        ulong CalculateMask(int i, ulong pieces);
    }

    public interface IRider
    {
        ulong EmptyMask();
        ulong Mask(ulong occ);
    }
}
