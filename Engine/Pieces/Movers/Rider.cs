namespace Engine.Pieces.Movers
{
    public interface IRiderCalc
    {
        ulong EmptyMask(int i);
        ulong CalculateMask(int i, ulong pieces);
    }
}
