namespace Engine.Pieces.MoveFactories
{
    public abstract class Base
    {
        // All of the squares which this particular mover applies for
        // Promotions is a good example - it only generates moves for the last/first row
        public ulong ApplicableSquares { get; set; } = ulong.MaxValue;
        public bool Side { get; set; }
        public Base() { }

        public void RemoveSquares(ulong remove)
        {
            ApplicableSquares = BitUtil.Remove(ApplicableSquares, remove);
        }

        public abstract Move[] ConvertMask(Board b, ulong start, ulong mask);
    }
}
