namespace RatMaze;

class Action
{
    public int Output;
    public List<Input> Inputs;
    public Weight Weight;
    public Weight[] WeightList;
    public AIDimension AbstractMatrix;

    public Action(int output, List<Input> inputs, Weight weight, Weight[] weightList)
    {
        Output = output;
        Inputs = inputs;
        Weight = weight;
        WeightList = weightList;
    }

    public Action(int output, List<Input> inputs, AIDimension abstractMatrix)
    {
        Output = output;
        Inputs = inputs;
        AbstractMatrix = abstractMatrix;
    }

    public void GiveDopamine(double dopamine)
    {
        if(AbstractMatrix != null) 
            Weight.GiveDopamine(dopamine, WeightList);
        else
        {
            
        }
    }

}