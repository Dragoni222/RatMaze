// 0 = open 1 = wall 2 = goal 3 = player

using System.Diagnostics;
using System.Numerics;

int[,] map =
{
    {1,1,1,1,1}, //y = 0
    {1,2,0,0,1},
    {1,0,3,0,1},
    {1,0,0,0,1},
    {1,1,1,1,1}  //y = 4
};


bool end = false;

while (end == false)
{
    LoadFrame();
    string answer = Console.ReadLine();
    int answerInt = 1;
    if (answer == null)
    {

    }
    else
    {
        try
        {
            answerInt = int.Parse(answer);
        }
        catch(FormatException e)
        {
            answerInt = 1;
        }

        if (answer == "matrixtest")
        {
            LoadMatrixTest();
        }
        else if (answer == "aitest")
        {
            LoadAiTest();
        }
    }
        


    for (int i = 0; i < answerInt; i++)
    {
        
    }
    
}


Coordinant playerPos = GetPlayerPos();

void Move(int direction) //n=1 e=2, s=3, w=4
{
    if (CanMoveInDirection(direction))
    {
        map[playerPos.Y, playerPos.X] = 0;
        Coordinant targetPos = DirectionToCoordinate(direction, playerPos);
        map[targetPos.Y, targetPos.X] = 3;

    }
}

void LoadMap()
{
    for (int y = 0; y < map.GetLength(0); y++)
    {
        for (int x = 0; x < map.GetLength(1); x++)
        {
            Console.Write(map[x,y] + " ");
        }
        Console.WriteLine();
    }
}

void LoadFrame()
{
    Console.Clear();
    LoadMap();
    LoadQuestion();
}

void LoadMatrixTest()
{
    AIDimension testMatrix = new AIDimension(1);

    Console.Write("Cube side length:");
    ulong sideLength = ulong.Parse(Console.ReadLine());
    Console.Write("\n dimensions:");
    ulong dimensions = ulong.Parse(Console.ReadLine());
    for (ulong i = 0; i < dimensions ; i++)
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();
        if (i == 0)
        {
            Weight[] sideValues = new Weight[sideLength];
            for (ulong j = 0; j < sideLength; j++)
            {
                sideValues[j].GiveDopamine(j, sideValues);
            }
            testMatrix = new AIDimension(sideValues);
        }
        else
        {
        
            testMatrix = new AIDimension(sideLength, testMatrix, true);
        }
        timer.Stop();
        Console.WriteLine("Dimension: " + i + " completed. Time:" + timer.Elapsed);

    }

    Console.WriteLine($"Completed. Get a value: {dimensions} parameters needed, max value of {sideLength - 1}");
    int[] address = new int[dimensions];
    for (ulong i = 0; i < dimensions; i++)
    {
        for (ulong j = 0; j < i; j++)
        {
            Console.Write(address[j] + ", ");
        }
        address[i] = int.Parse(Console.ReadLine());
    }

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    Console.WriteLine("Value: " + AIDimension.GetSpecificWeight(testMatrix, address));
    stopwatch.Stop();
    Console.WriteLine("Time elapsed getting value: " + stopwatch.Elapsed);
    Console.ReadLine();
}

void LoadAiTest()//loads a test for learning addition.
{
    Console.Clear();
    AI testAI = new AI(200, new List<Input>() {new Input(100, 1), new Input(100,1)});
    Console.Write("Seed: ");
    Random random = new Random(int.Parse(Console.ReadLine()));
    Console.Write("\n Cycles: ");
    int cycles = int.Parse(Console.ReadLine());
    Console.Write("\n Train Speed: ");
    double trainSpeed = double.Parse(Console.ReadLine());
    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();
    for (int i = 0; i < cycles; i++)
    {
        if (i % 1000000 == 0)
        {
            Console.WriteLine("Cycle: " + i);
        }

        testAI.Inputs[0].SetValue((ulong)random.Next(0,100));
        testAI.Inputs[1].SetValue((ulong)random.Next(0,100));
        int expectedValue = (int)testAI.Inputs[0].GetValue() + (int)testAI.Inputs[1].GetValue();
        AI.TrainByDifferenceKnown(testAI, expectedValue, random, trainSpeed);
    }

    stopwatch.Stop();
    Console.WriteLine($"Completed. Time: {stopwatch.Elapsed}.     Test? (Y/N)");
    string input = Console.ReadLine();
    if (input == "y" || input == "Y")
    {
        bool end = false;
        while (!end)
        {
            Console.Write("\nTest input 1: (0-100): ");
            testAI.Inputs[0].SetValue(ulong.Parse(Console.ReadLine()));
            Console.Write("\nTest input 2: (0-100): ");
            testAI.Inputs[1].SetValue(ulong.Parse(Console.ReadLine()));
            Console.WriteLine(AI.GetBestActionKnown(testAI.AiMatrix, testAI.Inputs ) + "  End? (Y/N)");
            input = Console.ReadLine();
            if (input == "Y" || input == "y")
            {
                end = true;
            }
        }
       
        
    }

}

void LoadQuestion()
{
    Console.WriteLine("/n Frames to skip? (enter if none)");
}

bool CanMoveInDirection(int direction)
{
    Coordinant targetPos = DirectionToCoordinate(direction, playerPos);
    
    return CanMoveIntoTile(map[targetPos.Y, targetPos.X ]);

}

bool CanMoveIntoTile(int tile)
{
    if (tile == 0 || tile == 2)
        return true;
    else
        return false;
}

Coordinant GetPlayerPos()
{
    for (int y = 0; y < map.GetLength(0); y++)
    {
        for (int x = 0; x < map.GetLength(1); x++)
        {
            if (map[x,y] == 3)
            {
                return new Coordinant(x, y);
            }
        }
        
    }

    return new Coordinant(-1, -1);
}

Coordinant DirectionToCoordinate(int direction, Coordinant position)
{
    if (direction == 1)
    {
        return new Coordinant(position.X, position.Y - 1);

    }
    else if (direction == 2)
    {
        return new Coordinant(position.X + 1, position.Y);


    }
    else if (direction == 3)
    {
        return new Coordinant(position.X, position.Y + 1);


    }
    else if (direction == 4)
    {
        return new Coordinant(position.X - 1, position.Y);


    }

    return new Coordinant(0, 0);
}

struct Coordinant 
{
    public Coordinant(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; }
    public int Y { get; }
}

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

    public static int GetBestActionKnown(AIDimension matrix, List<Input> inputs)
    {
        int[] inputArray = new int[inputs.Count];
        for (int i = 0; i < inputs.Count; i++)
        {
            inputArray[i] = (int)inputs[i].GetValue();
        }

        double[] weights = AIDimension.GetWeightsDouble(matrix, inputArray);
        
        return weights.ToList().IndexOf(weights.Max());
    }
    public static int GetRandomActionKnown(AIDimension matrix, List<Input> inputs, Random random)
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
                return i;
            }
            else
            {
                currentRandomSum += weights[i].GetValue();
            }
        }
        return -1;
    }

    public static int GetBestActionUnknown(AIDimension matrix, List<Input> inputsWithNulls, string consolidateType) //input the ordered with with some null values, will attempt to approximate
    {
        AIDimension abstraction = Abstract(matrix, inputsWithNulls); //gives a new matrix with all the possible weights for unknown values
        if (consolidateType == "mean")
        {
            double[] weights = ConsolidateAbstractionMean(abstraction);
            
            return weights.ToList().IndexOf(weights.Max());
        }
        else
        {
            Console.WriteLine("did not recognise consolidation");
            return -1;
        }
    }

    public static int TrainByDifferenceKnown(AI ai, int expected, Random random, double trainSpeed) // trains with current input values, valuing by difference between expected and given values.
    {
        int[] inputArray = new int[ai.Inputs.Count];
        for (int i = 0; i < ai.Inputs.Count; i++)
        {
            inputArray[i] = (int)ai.Inputs[i].GetValue();
        }

        int action = GetRandomActionKnown(ai.AiMatrix, ai.Inputs, random);
        int[] inputArrayWeight = new int[ai.Inputs.Count + 1];
        for (int i = 0; i < ai.Inputs.Count; i++)
        {
            inputArrayWeight[i] = inputArray[i];
        }
        
        inputArrayWeight[^1] = action;
        double difference = Math.Abs(expected - action);
        
        AIDimension.ChangeSpecificWeight(ai.AiMatrix, inputArrayWeight, 1/(difference+0.25d) * trainSpeed);
        return action;
    }

    private static AIDimension Abstract(AIDimension matrix, List<Input> inputsWithNulls) //^^
    {
        if (inputsWithNulls.Count == 0)
        {
            
        }
        else if ((int)inputsWithNulls[0].GetValue() == null)
        {
            List<AIDimension> dimensions = new List<AIDimension>();
            for (int i = 0; i < matrix.Positions.Count(); i++)
            {
                 dimensions.Add(Abstract(matrix.Positions[i], inputsWithNulls.GetRange(1, inputsWithNulls.Count - 1)));
            }

            return new AIDimension(dimensions);
        }
        else
        {
            return Abstract(matrix.Positions[(int)inputsWithNulls[0].GetValue()],
                inputsWithNulls.GetRange(1, inputsWithNulls.Count - 1));
        }

        return null;
    }

    private static double[] ConsolidateAbstractionMean(AIDimension abstraction)
    {
        if (abstraction.Weights != null)
        {
            double[] final = new double[abstraction.Weights.Length];
            for(int i = 0; i < abstraction.Weights.Length; i++)
            {
                final[i] = abstraction.Weights[i].GetValue();
            }
            return final;
        }
        else
        {
            List<double> finalWeights = new List<double>();
            int totalWeightSets = abstraction.Positions.Count;
            for (int i = 0; i < abstraction.Positions.Count(); i++) 
            {
                double[] lowerWeights = ConsolidateAbstractionMean(abstraction.Positions[i]); //gets a weight set

                for (int j = 0; j < lowerWeights.Length; j++) //adds all the individual weights
                {
                    finalWeights[j] += lowerWeights[j];
                }

                if (i == 0)
                {
                    totalWeightSets *= lowerWeights.Length;
                }
                
            }

            for(int i = 0; i < finalWeights.Count; i++)
            {
                finalWeights[i]/=totalWeightSets;
            }

            return finalWeights.ToArray();
        }
        
    }
    


}

class Input
{
    public ulong MaxValue;
    private ulong CurrentValue;
    private bool Unknown;

    public Input(ulong maxValue, ulong currentValue)
    {
        MaxValue = maxValue;
        CurrentValue = currentValue;
        Unknown = false;
    }

    public Input()
    {
        Unknown = true;
        MaxValue = 1;
        CurrentValue = 0;
    }

    public void SetValue(ulong value)
    {
        if (value < MaxValue)
        {
            CurrentValue = value;
        }
    }

    public ulong? GetValue()
    {
        if (Unknown)
        {
            return null;
        }
        return CurrentValue;
    }
    
}

class AIDimension
{
    public List<AIDimension> Positions;
    public Weight[] Weights; //only the lowest dimension has weights

    public AIDimension(ulong numOfPossibleValues, AIDimension currentMatrix ,bool copy)
    {

        Positions = new List<AIDimension>();
        Positions.Add(currentMatrix);
        if (copy)
        {
            for (ulong i = 1; i < numOfPossibleValues; i++)
            {
                Positions.Add(CopyDimension(currentMatrix, false));
            }
        }
        else
        {
            AIDimension unweighted = UnweightedMatrix(currentMatrix);
            for (ulong i = 1; i < numOfPossibleValues; i++)
            {
                Positions.Add(CopyDimension(unweighted, false));
            }
        }
        
        
    }

    public AIDimension(Weight[] weights)
    {
        Weights = weights;
        Positions = new List<AIDimension>();
    }

    public AIDimension(int length)
    {
        Weights = new Weight[length];
        for (int i = 0; i < length; i++)
        {
            Weights[i] = new Weight((1/(double)length), 0, 1);
        }

        Positions = new List<AIDimension>();
    }

    public AIDimension(List<AIDimension> positions)
    {
        Positions = positions;
    }
    
    public static AIDimension CopyDimension(AIDimension copy, bool copyTimesUsed)
    {
        if (copy.Weights != null)
        {
            Weight[] weightCopy = new Weight[copy.Weights.Length]; //WARNING THIS ASSIGNS INSTEAD OF COPYS WEIGHTS
            for(int i = 0; i < weightCopy.Length; i++)
            {
                weightCopy[i] = copy.Weights[i].Copy(copyTimesUsed);
            }
            return new AIDimension(weightCopy);
        }
        else
        {
            List<AIDimension> positions = new List<AIDimension>();
            for (int i = 0; i < copy.Positions.Count; i++)
            {
                positions.Add(CopyDimension(copy.Positions[i], copyTimesUsed));
            }
            return new AIDimension(positions);
        }
        
    }

    public static double GetWeightsDopamine(Weight[] weights)
    {
        double total = 0;
        foreach (var weight in weights)
        {
            total += weight.GetDopamine();
        }

        return total;
    }

    public static AIDimension UnweightedMatrix(AIDimension weightedMatrix)
    {
        AIDimension unweighted =  CopyDimension(weightedMatrix, false);
        if (unweighted.Weights is { Length: > 0 })
        {
            for (int i = 0; i < unweighted.Weights.Length; i++)
            {
                unweighted.Weights[i].GiveDopamine(-unweighted.Weights[i].GetDopamine() + 1, unweighted.Weights);
            }
        }
        else
        {
            for(int i = 0; i < unweighted.Positions.Count; i++)
            {
                unweighted.Positions[i] = UnweightedMatrix(unweighted.Positions[i]);
            }
        }

        return unweighted;

    }

    public static void ChangeSpecificWeight(AIDimension matrix, int[] address, double dopamine)
    {

        if (address.Length == 1)
        {
            try
            {
                double originalValue = matrix.Weights[address[0]].GetValue();
                matrix.Weights[address[0]].GiveDopamine(dopamine, matrix.Weights);
                double newValue = matrix.Weights[address[0]].GetValue();
                
                
            }
            catch (Exception e)
            {
                Console.Write("Change Wrong Address: "  + address[0]);
            }
        }
        else
        {
            ChangeSpecificWeight(matrix.Positions[address[0]], address.Skip(1).ToArray(), dopamine);
        }
    }

    public static int GetDimensions(AIDimension matrix, int startValue) //startValue should be 1, returns number of dimensions (not counting weights)
    {
        if (matrix.Positions.Any())
        { 
             return GetDimensions(matrix.Positions[0], startValue + 1);
        }
        else
        {
            return startValue;
        }
    }
    
    public static double GetSpecificWeight(AIDimension matrix, int[] address)
    {

        if (address.Length == 1)
        {

            try
            {
                return matrix.Weights[address[0]].GetValue();
            }
            catch (Exception e)
            {
                Console.Write("Get Wrong Address: " + address[0]);
            }
        }
        else
        {
            return GetSpecificWeight(matrix.Positions[address[0]], address.Skip(1).ToArray());
        }

        return -1;
    }

    public static Weight[] GetWeights(AIDimension matrix, int[] address)
    {
        
        if (address.Length == 1)
        {
            try
            {
                return matrix.Positions[address[0]].Weights;
            }
            catch (Exception e)
            {
                Console.Write("Get Wrong multi Address");
            }
        }
        else
        {
            return GetWeights(matrix.Positions[address[0]], address.Skip(1).ToArray());
        }
        Console.WriteLine("GetWeightsFailed");
        return null;
    }

    public static double[] GetWeightsDouble(AIDimension matrix, int[] address)
    {
        if (address.Length == 1)
        {
            try
            {
                double[] finalWeights = new double[matrix.Positions[address[0]].Weights.Length];
                for (int i = 0; i < finalWeights.Length; i++)
                {
                    finalWeights[i] = matrix.Positions[address[0]].Weights[i].GetValue();
                }
                return finalWeights;
            }
            catch (Exception e)
            {
                Console.Write("Get Wrong multi Address");
            }
        }
        else
        {
            return GetWeightsDouble(matrix.Positions[address[0]], address.Skip(1).ToArray());
        }
        Console.WriteLine("GetWeightsFailed");
        return null;
    }

}

class Weight
{
    private double value;
    public int timesUsed;
    private double dopamine;
    public Weight(double startValue)
    {
        value = startValue;
        dopamine = 1;
        timesUsed = 0;
    }
    public Weight(double startValue, int startTimesUsed)
    {
        value = startValue;
        dopamine = 1;
        timesUsed = startTimesUsed;
    }
    public Weight(double startValue, int startTimesUsed, double startDopamine)
    {
        value = startValue;
        dopamine = startDopamine;
        timesUsed = startTimesUsed;
        
    }

    public void GiveDopamine(double dope, Weight[] weights)
    {
        dopamine += dope;
        double totalDopamine = AIDimension.GetWeightsDopamine(weights);
        SetValue(dopamine/totalDopamine);
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] != this)
            {
                weights[i].SetValue(weights[i].GetDopamine()/totalDopamine);
            }
        }
        timesUsed++;
    }

    public double GetDopamine()
    {
        return dopamine;
    }
    
    private void SetValue(double newValue)
    {
        if (newValue <= 1 && newValue >= 0)
        {
            value = newValue;
        }
        else
        {
            Console.WriteLine("Attempted to change weight to " + newValue);
        }
    }
    

    public double GetValue()
    {
        return value;
    }
    

    public Weight Copy(bool copyTimesUsed)
    {
        if (copyTimesUsed)
            return new Weight(value,timesUsed,dopamine);
        else return new Weight(value, 0, dopamine);
    }

}





