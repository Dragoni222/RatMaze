namespace RatMaze;
using RatMaze;
class CausalityAI
{
    public AI ai;

    public CausalityAI(List<Input> situation)
    {
        situation.Add(new Input((ulong)situation.Count, 0));
        ai = new AI(situation.Count - 1, situation);
    }
    
    
}