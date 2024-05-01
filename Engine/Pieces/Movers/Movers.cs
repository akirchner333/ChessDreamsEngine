namespace Engine.Pieces.Movers
{
    public interface IMover
    {
        ulong MoveMask(int Index, Board board);
    }
}
