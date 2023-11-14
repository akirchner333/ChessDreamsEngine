namespace Engine.Pieces.Movers
{
    public interface IMover
    {
        ulong MoveMask(int Index, Board board);
    }
    //In case it ever becomes useful to have a mover base class
    //public class Movers
    //{
    //}
}
