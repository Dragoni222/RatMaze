using RatMaze.Games;

namespace RatMaze;

class LogicAI //basically an AI trained to make SeriesTransforms to solve problems
{
    public List<Transform> AllTransforms;
    public AI TransformChooserAI; // output 0 stops adding new transforms to the list
    public List<Input> GivenSituation; //We can change the inputs themselves after initializing, but not how many of them
    private List<Input> SituationPlusOutputChoosing;
    public void AddTransform(Transform newTransform, List<Input> situationUsedIn) //only add a transform when you are very sure it is useful
    {
        AllTransforms.Add(newTransform);
        AIDimension.AddWeight(TransformChooserAI.AiMatrix, 1);
        int[] weightList = new int[situationUsedIn.Count - 1];
        for (int i = 0; i < situationUsedIn.Count; i++)
        {
            weightList[i] = (int)situationUsedIn[i].GetValue();
        }
        
        Weight[] currentWeights = AIDimension.GetWeights(TransformChooserAI.AiMatrix, weightList);
        currentWeights[^1].GiveDopamine(AIDimension.GetWeightsDopamine(currentWeights)/2, currentWeights);
    }

    public void AddTransform(Transform newTransform)
    {
        AllTransforms.Add(newTransform);
        AIDimension.AddWeight(TransformChooserAI.AiMatrix, 1);
    }

    public LogicAI(List<Input> inputs)
    {
        SituationPlusOutputChoosing = new List<Input>();
        SituationPlusOutputChoosing.AddRange(GivenSituation);
        SituationPlusOutputChoosing.Add(new Input(3, 0)); //adds the part that says whether it's choosing inputs or outputs
        SituationPlusOutputChoosing.Add(new Input(100, 0)); //adds the part that says what transform it's putting values into
        SituationPlusOutputChoosing.Add(new Input(100, 0)); //adds the part that says which parameter in the transform it's choosing
        TransformChooserAI = new AI(0, SituationPlusOutputChoosing);
        GivenSituation = inputs;
        AllTransforms = new List<Transform>();
    }

    public LogicAI(List<Input> inputs, List<Transform> startTransforms)
    {
        SituationPlusOutputChoosing = new List<Input>();
        SituationPlusOutputChoosing.AddRange(GivenSituation);
        SituationPlusOutputChoosing.Add(new Input(3, 0)); //adds the part that says whether it's choosing inputs or outputs
        SituationPlusOutputChoosing.Add(new Input(100, 0)); //adds the part that says what transform it's putting values into
        SituationPlusOutputChoosing.Add(new Input(100, 0)); //adds the part that says which parameter in the transform it's choosing
        TransformChooserAI = new AI(0, SituationPlusOutputChoosing);
        GivenSituation = inputs;
        AllTransforms = new List<Transform>();
        for (int i = 0; i < startTransforms.Count; i++)
        {
            AddTransform(startTransforms[i]);
        }
    }
    
    public LogicOutput GetActionRandom(Random random)
    {
        bool stop = false;
        List<Transform> transformsUndergone = new List<Transform>();
        List<int> transformIndexes = new List<int>();
        SituationPlusOutputChoosing[^3].SetValue(0); 
        SituationPlusOutputChoosing[^2].SetValue(0); 
        while (!stop)
        {
            
            SituationPlusOutputChoosing[^1].SetValue((ulong)transformsUndergone.Count);//in this context, the last index is reused to say which transform it is on
            int chosenTransform = AI.GetRandomActionKnown(TransformChooserAI.AiMatrix, SituationPlusOutputChoosing, random).Output;
            if (chosenTransform != 0)
            {
                transformsUndergone.Add(AllTransforms[chosenTransform]);
                transformIndexes.Add(chosenTransform);
            }
            else
            {
                stop = true;
            }
            
        }

        List<int> inputIndexes = new List<int>();
        SituationPlusOutputChoosing[^3].SetValue(1); 
        for(int j = 0; j < transformsUndergone.Count; j++)
        {
            SituationPlusOutputChoosing[^2].SetValue((ulong)j);
            for (int i = 0; i < transformsUndergone[j].NumOfInputs; i++)
            {
                SituationPlusOutputChoosing[^1].SetValue((ulong)i);
                int chosenIndex = AI.GetRandomActionKnown(TransformChooserAI.AiMatrix, SituationPlusOutputChoosing, random).Output;
                while (chosenIndex >=GivenSituation.Count)
                {
                    
                    chosenIndex = AI.GetRandomActionKnown(TransformChooserAI.AiMatrix, SituationPlusOutputChoosing, random).Output;
                }
                inputIndexes.Add(chosenIndex);
            }
        }
        List<int> outputIndexes = new List<int>();
        SituationPlusOutputChoosing[^3].SetValue(2); 
        for(int j = 0; j < transformsUndergone.Count; j++)
        {
            SituationPlusOutputChoosing[^2].SetValue((ulong)j);
            for (int i = 0; i < transformsUndergone[j].NumOfOutputs; i++)
            {
                SituationPlusOutputChoosing[^1].SetValue((ulong)i);
                int chosenIndex = AI.GetRandomActionKnown(TransformChooserAI.AiMatrix, SituationPlusOutputChoosing, random).Output;
                while (chosenIndex >=GivenSituation.Count)
                {
                    chosenIndex = AI.GetRandomActionKnown(TransformChooserAI.AiMatrix, SituationPlusOutputChoosing, random).Output;
                }
                outputIndexes.Add(chosenIndex);
            }
        }

        return new LogicOutput(transformsUndergone, transformIndexes, inputIndexes, outputIndexes);

    }

    public void Train(List<Input> desiredOutput, Random random, double trainSpeed)
    {
        LogicOutput output = GetActionRandom(random);
        DoTransforms(output.Transforms, output.TransformInputIndexes, output.TransformOutputIndexes);
        int incorrectOutputs = 0;
        for (int i = 0; i < desiredOutput.Count; i++)
        {
            if (GivenSituation[i].GetValue() != desiredOutput[i].GetValue())
            {
                incorrectOutputs++;
            }
        }

        for(int i = 0; i < output.TransformIndexes.Count; i++)
        {
            List<Weight> weightsToChange = new List<Weight>();
            List<Weight[]> weightListsToChange = new List<Weight[]>();
            //Changes the weight at each transform (hard bit: it should at some level of incorrectness increase the chance of choosing no transform)
            
            
            
            for (int j = 0; j < weightsToChange.Count; j++)
            {
                weightsToChange[j].GiveDopamine(1/(double)(incorrectOutputs + 1) * trainSpeed, weightListsToChange[j]); //weights it based on how right it was.
            }
  
        }
    }

    public Transform CreateTransform(Game gameTransformPlays)
    {
        return new BaseTransform(gameTransformPlays.MakeSuitableAi(), gameTransformPlays);
    }

    public Transform CreateTransform(List<Transform> transforms)
    {
        return new MetaTransform(transforms);
    }

    public void DoTransforms(List<Transform> transforms, List<int> inputs, List<int> outputs)
    {
        MetaTransform transform = new MetaTransform(transforms);
        transform.DoTransform(GivenSituation, inputs, outputs);

    }
    

}

struct LogicOutput
{
    public List<Transform> Transforms;
    public List<int> TransformIndexes;
    public List<int> TransformInputIndexes;
    public List<int> TransformOutputIndexes;
    
    public LogicOutput(List<Transform> transforms, List<int> transformIndexes, List<int> transformInputIndexes, List<int> transformOutputIndexes)
    {
        Transforms = transforms;
        TransformIndexes = transformIndexes;
        TransformOutputIndexes = transformOutputIndexes;
        TransformInputIndexes = transformInputIndexes;
    }
    
}