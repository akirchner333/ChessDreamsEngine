namespace Engine.Rules
{
    public interface IRule
    {
        Move ApplyMove(Move move, int pieceIndex);
        void ReverseMove(Move move, int pieceIndex);
    }
}
