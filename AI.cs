namespace RatMaze;
using RatMaze;
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

    public static List<int> RelevantInputIndexes(AIDimension matrix, int dimensionToCheck, double criticalValue)
    {
        double[] pVals = InputRelevancePVals(matrix, dimensionToCheck);
        List<int> final = new List<int>();
        for (int i = 0; i < pVals.Length; i++)
        {
            if (pVals[i] > 0 && pVals[i] <= criticalValue)
            {
                final.Add(i);
            }
        }

        return final;
    }
    public static double[] InputRelevancePVals(AIDimension matrix, int dimensionToCheck)
    {
        int totalDimensions = 1; //the 0th dimension is the weight dimension
        AIDimension currentDimension = matrix;
        int dimensionMaxValue = 0;
        bool bottom = false;
        while (bottom == false)
        {
            if (totalDimensions == dimensionToCheck)
            {
                dimensionMaxValue = currentDimension.Positions.Count;
            }
            if (currentDimension.Weights != null)
            {
                bottom = true;
            }
            else
            {
                totalDimensions++;
                currentDimension = currentDimension.Positions[0];
            }
            
        }

        double[] final = new double[totalDimensions];
        
        List<Input> currentAbstraction = new List<Input>();
        for (int j = 0; j < totalDimensions; j++)
        {
            if (j == dimensionToCheck)
            {
                currentAbstraction.Add(new Input((ulong)dimensionMaxValue + 1, 0));
            }
            else
            {
                currentAbstraction.Add(new Input());
            }
        }

        double[] chiSqsForEachDimension = new double[totalDimensions];
        int[] dfForEachDimension = new int[totalDimensions];
        for (int i = 0; i < dimensionMaxValue; i++) //gets the chi-sq at each dimension for each value of the observed dimension
        {
            currentAbstraction[dimensionToCheck].SetValue((ulong)i);
            AIDimension abstractMatrix = AIDimension.Abstract(matrix, currentAbstraction);
            List<Weight[]> weights = AIDimension.GetAbstractedWeights(abstractMatrix);
            for (int j = 0; j < weights.Count; j++)
            {
                double chiSq = 0; // we use a chi-sq test to determine if something correlates
                double expected = 1 / (double)weights[j].Length;
                for (int k = 0; k < weights[j].Length; k++)
                {
                    chiSq += Math.Pow(weights[j][k].GetValue() - expected, 2) / expected;
                }

                if (i == 0)
                {
                    chiSqsForEachDimension[j] = chiSq;
                    dfForEachDimension[j] = weights[j].Length - 1;
                }

                else
                    chiSqsForEachDimension[j] += chiSq;
            }
        }
        for(int i = 0; i < totalDimensions; i++) //computes whether or not each dimension impacts the observed one
        {
            if(i != dimensionToCheck)
                final[i] = ChiSq.ChiSqPval(chiSqsForEachDimension[i], dfForEachDimension[i]);
            else
            {
                final[i] = -1;
            }
            
        }

        return final;
    }

    public static double ConfidenceRatio(AIDimension matrix, List<Input> situation, int dimensionToAbstract)
    {
        Weight[] startWeights = AIDimension.GetWeights(matrix, Input.ConvertToAddress(situation));
        double startTimesUsed = 0;

        foreach (Weight weight in startWeights)
        {
            startTimesUsed += weight.timesUsed;
        }
        List<Input> abstractSituation = situation;
        List<Weight> endWeights = new List<Weight>();
        int dimensionPositions = AIDimension.DimensionPositions(matrix, dimensionToAbstract);
        for (int i = 0; i < dimensionPositions; i++)
        {
            abstractSituation[dimensionToAbstract].SetValue((ulong)i);
            endWeights.AddRange(AIDimension.GetWeights(matrix, Input.ConvertToAddress(abstractSituation)));
        }

        double endTimesUsed = 0;
        foreach (Weight weight in endWeights)
        {
            endTimesUsed += weight.timesUsed;
        }
        
        Console.WriteLine(endTimesUsed + ", " + startTimesUsed);
        return endTimesUsed / startTimesUsed;

    }

    public static List<int> NotConfidentInputIndexes(AIDimension matrix,  List<Input> situation, double minimumRatio)
    {
        List<int> final = new List<int>();
        for (int i = 0; i < situation.Count; i++)
        {
            if (ConfidenceRatio(matrix, situation, i) > minimumRatio)
            {
                final.Add(i);
            }
        }

        return final;




    }
    
    
}

