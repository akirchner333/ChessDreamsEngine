namespace Engine.Pieces.Movers
{
    public class KillerLeaper : Leaper
    {
        public KillerLeaper(int[] directions, string key) : base(directions, key) { }

        public override ulong MoveMask(int index, Board board)
        {
            return GetMoveDatabase(index) & board.SidePieces(!Side);
        }
    }
}
