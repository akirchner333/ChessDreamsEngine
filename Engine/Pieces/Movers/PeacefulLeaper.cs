namespace Engine.Pieces.Movers
{
    public class PeacefulLeaper : Leaper
    {
        public PeacefulLeaper(int[] directions, string key) : base(directions, key) { }

        public override ulong MoveMask(int index, Board board)
        {
            return BitUtil.Remove(GetMoveDatabase(index), board.AllPieces);
        }
    }
}
