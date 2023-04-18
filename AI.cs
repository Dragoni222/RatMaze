namespace RatMaze;

class AI
{
    public AIDimension AiMatrix;
    private int Outputs;
    public List<Input> Inputs;

    public AI(int outputs, List<Input> inputs)
    {
        Outputs = outputs;
        Inputs = inputs;
        Weight[] baseWeights = new Weight[outputs];
        for (int i = 0; i < outputs; i++) //each output has a weight
        {
            baseWeights[i] = new Weight(1/(double)outputs, 0, 1);
        }
        AiMatrix = new AIDimension(baseWeights);
        foreach (Input input in inputs) //each input is a new dimension
        {
            AiMatrix = new AIDimension(input.MaxValue, AiMatrix, true); //creates a symetrical matrix with base weights
        }
    }

    public void AddInput(Input input)
    {
         Inputs.Add(input);
         AiMatrix = new AIDimension(input.MaxValue, AiMatrix, true);
    }

    public void AddOutput()
    {
        
    }
    public static Action GetBestActionKnown(AIDimension matrix, List<Input> inputs)
    {
        int[] inputArray = new int[inputs.Count];
        for (int i = 0; i < inputs.Count; i++)
        {
            inputArray[i] = (int)inputs[i].GetValue();
        }
        Weight[] weights = AIDimension.GetWeights(matrix, inputArray);
        Weight currentMax = new Weight(0);
        int currentMaxIndex = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i].GetValue() > currentMax.GetValue())
            {
                currentMax = weights[i];
                currentMaxIndex = i;
            }
        }
        return new Action(currentMaxIndex, inputs, currentMax, weights);
    }
    public static Action GetRandomActionKnown(AIDimension matrix, List<Input> inputs, Random random)
    {
        int[] inputArray = new int[inputs.Count];
        for (int i = 0; i < inputs.Count; i++)
        {
            inputArray[i] = (int)inputs[i].GetValue();
        }
        Weight[] weights = AIDimension.GetWeights(matrix, inputArray);
        double randomCheck = random.NextDouble();
        double currentRandomSum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            if ((randomCheck >= currentRandomSum && randomCheck < currentRandomSum + weights[i].GetValue()) || i + 1 == weights.Length)
            {
                return new Action(i, inputs, weights[i], weights);
            }
            else
            {
                currentRandomSum += weights[i].GetValue();
            }
        }
        return null;
    }
    public static void TrainByDifferenceKnown(AI ai, int expected, Random random, double trainSpeed) // trains with current input values, valuing by difference between expected and given values.
    {
        int[] inputArray = new int[ai.Inputs.Count];
        for (int i = 0; i < ai.Inputs.Count; i++)
        {
            inputArray[i] = (int)ai.Inputs[i].GetValue();
        }

        Action action = GetRandomActionKnown(ai.AiMatrix, ai.Inputs, random);
        int[] inputArrayWeight = new int[ai.Inputs.Count + 1];
        for (int i = 0; i < ai.Inputs.Count; i++)
        {
            inputArrayWeight[i] = inputArray[i];
        }
        
        inputArrayWeight[^1] = action.Output;
        double difference = Math.Abs(expected - action.Output);
        
        AIDimension.ChangeSpecificWeight(ai.AiMatrix, inputArrayWeight, 1/(difference+0.25d) * trainSpeed);
    }

    public static Weight[] PartialAbstraction(AIDimension matrix, List<Input> situation, int dimensionOutput)
    {
        AIDimension abstractedMatrix = AIDimension.Abstract(matrix, situation);
        List<Weight[]> weightsList = AIDimension.GetAbstractedWeights(abstractedMatrix);
        return weightsList[dimensionOutput];

    }


}