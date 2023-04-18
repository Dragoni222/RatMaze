using RatMaze.Games;

namespace RatMaze;

abstract class Transform //this is the lowest level of transform, performed a by pre-trained feeling AI
{
    public int NumOfInputs;
    public int NumOfOutputs;
    public abstract void DoTransform(List<Input> inputs,List<int> inputIndex, List<int> outputIndex);

    public abstract void TrainTransform(Random random);

    public List<Input> GetInputs(List<Input> allInputs, List<int> inputIndexes)
    {
        List<Input> final = new List<Input>();
        foreach (var inputIndex in inputIndexes)
        {
            final.Add(allInputs[inputIndex]);
        }

        return final;
    }

}

class BaseTransform : Transform //this is the lowest level of transform, performed a by pre-trained feeling AI
{
    public AI TransformAi;
    public Game TrainingGame;
    
    public BaseTransform(AI ai, Game trainingGame)
    {
        TransformAi = ai;
        NumOfInputs = ai.Inputs.Count;
        NumOfOutputs = 1;
        TrainingGame = trainingGame;
    }

    public BaseTransform(Game trainingGame)
    {
        TransformAi = trainingGame.MakeSuitableAi();
        NumOfInputs = TransformAi.Inputs.Count;
        NumOfOutputs = 1;
        TrainingGame = trainingGame;
    }

    public BaseTransform(int numOfInputs, int numOfPotentialOutputs, Game trainingGame)
    {
        NumOfInputs = numOfInputs;
        TransformAi = new AI(numOfPotentialOutputs, new List<Input>());
        NumOfOutputs = 1;
        TrainingGame = trainingGame;
    }

    public override void DoTransform(List<Input> inputs, List<int> inputIndex, List<int> outputIndex)
    {
        List<Input> specificInputs = GetInputs(inputs, inputIndex);
        
        inputs[outputIndex[0]].SetValue((ulong)AI.GetBestActionKnown(TransformAi.AiMatrix, specificInputs).Output);
    }

    public override void TrainTransform(Random random)
    {
        TrainingGame.TrainAi(random);
    }
}

class MetaTransform : Transform
{
    private List<Transform> Transforms;
    public MetaTransform(List<Transform> transforms)
    {
        Transforms = transforms;
        foreach (var transform in transforms)
        {
            NumOfInputs += transform.NumOfInputs;
            NumOfOutputs += transform.NumOfOutputs;
        }
    }

    public override void DoTransform(List<Input> inputs, List<int> inputIndex, List<int> outputIndex)
    {
        int inputIndexRangeStart = 0;
        for (int i = 0; i < Transforms.Count; i++)
        {
            Transforms[i].DoTransform(inputs,inputIndex.GetRange(inputIndexRangeStart, Transforms[i].NumOfInputs),new List<int> {outputIndex[i]});
            inputIndexRangeStart += Transforms[i].NumOfInputs;
        }
    }

    public override void TrainTransform(Random random)
    {
        for (int i = 0; i < Transforms.Count; i++)
        {
            Transforms[i].TrainTransform(random);
        }
    }
}