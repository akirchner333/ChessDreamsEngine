namespace Engine.Rules
{
    // A rule is something that is applied to the board when a move happens
    // ApplyMove returns a move because it may annotate the moves with information
    // relevant to reversal
    public interface IRule
    {
        Move ApplyMove(Move move, int pieceIndex);
        void ReverseMove(Move move, int pieceIndex);
    }
}
