namespace Engine.Pieces.Movers
{
    public class MoverUtil
    {
        public static Func<int, ulong> Shifter(int steps, ulong start)
        {
            return steps > 0 ?
                (x) => start << (x * steps) :
                (x) => start >> (-x * steps);
        }

        public static Func<ulong, int, ulong> BareShifter(int steps)
        {
            return steps > 0 ?
                (start, x) => start << (x * steps) :
                (start, x) => start >> (-x * steps);
        }

        public static ulong Blocker(int direction)
        {
            var blocker = 0ul;
            int reduced = direction % 8;
            //I'm making a values call here - no piece is allowed to move more than 4 pieces in any direction
            //If you try to do that, I assume you're wrapping ilegally
            //Might be better to define directions as x, y - then I could know for sure what they wanted
            //As is, I don't think it's actually possible to determine blockers purely from the number
            if (reduced > 4)
                reduced -= 8;
            else if (reduced < -3)
                reduced += 8;

            var columns = "ABCDEFGH";
            if (reduced > 0)
            {
                for (var i = 0; i < reduced; i++)
                {
                    blocker |= Board.Columns[columns[i].ToString()];
                }
            }
            else if (reduced < 0)
            {
                for (var i = 7; i > 7 + reduced; i--)
                {
                    blocker |= Board.Columns[columns[i].ToString()];
                }
            }

            return blocker;
        }
    }
}
