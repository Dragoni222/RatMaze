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
            double[] sideValues = new double[sideLength];
            for (ulong j = 0; j < sideLength; j++)
            {
                sideValues[j] = j;
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
        AiMatrix = new AIDimension(4);
        Inputs = inputs;
        foreach (Input input in inputs) //each input is a new dimension
        {
            AiMatrix = new AIDimension(input.MaxValue, AiMatrix, false);
        }
    }

    public static int GetBestActionKnown(AIDimension matrix, List<Input> inputs)
    {
        int[] inputArray = new int[inputs.Count];
        for (int i = 0; i < inputs.Count; i++)
        {
            inputArray[i] = (int)inputs[i].GetValue();
        }

        double[] weights = AIDimension.GetWeights(matrix, inputArray);
        return weights.ToList().IndexOf(weights.Max());
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
            return abstraction.Weights;
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
                finalWeights[i] /= totalWeightSets;
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
    public double[] Weights; //only the lowest dimension has weights

    public AIDimension(ulong numOfPossibleValues, AIDimension currentMatrix ,bool copy)
    {

        Positions = new List<AIDimension>();
        Positions.Add(currentMatrix);
        if (copy)
        {
            for (ulong i = 1; i < numOfPossibleValues; i++)
            {
                Positions.Add(currentMatrix);
            }
        }
        else
        {
            AIDimension unweighted = UnweightedMatrix(currentMatrix);
            for (ulong i = 1; i < numOfPossibleValues; i++)
            {
                Positions.Add(CopyDimension(unweighted));
            }
        }
        
        
    }

    public AIDimension(double[] weights)
    {
        Weights = weights;
        Positions = new List<AIDimension>();
    }

    public AIDimension(int length)
    {
        Weights = new double[length];
        for (int i = 0; i < length; i++)
        {
            Weights[i] = 0;
        }

        Positions = new List<AIDimension>();
    }

    public AIDimension(List<AIDimension> positions)
    {
        Positions = positions;
    }
    
    public static AIDimension CopyDimension(AIDimension copy)
    {
        if (copy.Weights != null)
        {
            double[] weightCopy = new double[copy.Weights.Length];
            Array.Copy(copy.Weights, weightCopy, copy.Weights.Length);
            return new AIDimension(weightCopy);
        }
        else
        {
            List<AIDimension> positions = new List<AIDimension>();
            for (int i = 0; i < copy.Positions.Count; i++)
            {
                positions.Add(CopyDimension(copy.Positions[i]));
            }
            return new AIDimension(positions);
        }
        
    }

    public static AIDimension UnweightedMatrix(AIDimension weightedMatrix)
    {
        AIDimension unweighted =  CopyDimension(weightedMatrix);
        if (unweighted.Weights is { Length: > 0 })
        {
            for (int i = 0; i < unweighted.Weights.Length; i++)
            {
                unweighted.Weights[i] = 0;
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

    public static void ChangeSpecificWeight(AIDimension matrix, int[] address, double newValue)
    {
        
        if (address.Length == 1)
        {
            try
            {
                matrix.Weights[address[0]] = newValue;
            }
            catch (Exception e)
            {
                Console.Write("Change Wrong Address");
            }
        }
        else
        {
            ChangeSpecificWeight(matrix.Positions[address[0]], address.Skip(1).ToArray(), newValue);
        }
    }
    
    public static double GetSpecificWeight(AIDimension matrix, int[] address)
    {
        if (address.Length == 1)
        {
            try
            {
                return matrix.Weights[address[0]];
            }
            catch (Exception e)
            {
                Console.Write("Get Wrong Address");
            }
        }
        else
        {
            return GetSpecificWeight(matrix.Positions[address[0]], address.Skip(1).ToArray());
        }

        return -1;
    }

    public static double[] GetWeights(AIDimension matrix, int[] address)
    {
        if (address.Length == 2)
        {
            try
            {
                return matrix.Weights;
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

        return null;
    }
    
    

}





