//todo add partial abstractions and implement them in the logic training system


using System.Diagnostics;
using System.Drawing;
using System.Numerics;

int[,] map =
{
    {1,1,1,1,1}, //y = 0
    {1,2,0,0,1},
    {1,0,3,0,1},
    {1,0,0,0,1},
    {1,1,1,1,1}  //y = 4
};

AIDimension testMatrix = new AIDimension(4);
testMatrix = new AIDimension(3, testMatrix, true);
testMatrix = new AIDimension(2, testMatrix, true);
testMatrix.Positions[0].Positions[0].Weights[0].GiveDopamine(1, testMatrix.Positions[0].Positions[0].Weights);
List<Input> testSituation = new List<Input>();
testSituation.Add( new Input(2, 0));
testSituation.Add(new Input(3, 0));
testSituation.Add( new Input(4, 0));
testSituation[1].SetUnknown(true);
testSituation[2].SetUnknown(true);
AIDimension abstractTestMatrix = AIDimension.Abstract(testMatrix, testSituation);

List<Weight[]> aTestMatrixWeightsList = AIDimension.GetAbstractedWeights(abstractTestMatrix);

for (int i = 0; i < aTestMatrixWeightsList.Count; i++)
{
    Console.Write("\nD" + i);
    double totalValue = 0;
    for (int j = 0; j < aTestMatrixWeightsList[i].Length; j++)
    {
        Console.Write(" " + j +"( " );
        Console.Write(aTestMatrixWeightsList[i][j].GetValue());
        Console.Write(", " +aTestMatrixWeightsList[i][j].GetDopamine());
        Console.Write(")");
        totalValue += aTestMatrixWeightsList[i][j].GetValue();
    }
    Console.Write( "  Total Val: " + totalValue);
}

Console.ReadLine();

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
        else if (answer == "tic")
        {
            LoadAiTicTacToe();
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
            Console.WriteLine(AI.GetBestActionKnown(testAI.AiMatrix, testAI.Inputs ).Output + "  End? (Y/N)");
            input = Console.ReadLine();
            if (input == "Y" || input == "y")
            {
                end = true;
            }
        }
       
        
    }

}

void LoadAiTicTacToe()
{
    Console.Clear();

    Console.Write("Seed: ");
    Random random = new Random(int.Parse(Console.ReadLine()));
    Console.Write("\n Cycles: ");
    int cycles = int.Parse(Console.ReadLine());
    Console.Write("\n Train Speed: ");
    double trainSpeed = double.Parse(Console.ReadLine());
    Stopwatch stopwatch = new Stopwatch();
    TicTacToe game = new TicTacToe(trainSpeed, 0);
    stopwatch.Start();
    for (int i = 0; i < cycles; i++)
    {
        if (i % 1000000 == 0)
        {
            Console.WriteLine("Cycle: " + i);
        }

        game.TrainAi(random);
        game.Reset();
    }

    stopwatch.Stop();
    Console.WriteLine($"Completed. Time: {stopwatch.Elapsed}.     Test? (Y/N)");
    string input = Console.ReadLine();
    if (input == "y" || input == "Y")
    {
        bool endGame = false;
        while (!endGame)
        {
            Console.WriteLine("Would you like to play first or second? (1/2)");
            TicTacToe gameVsPlayer = new TicTacToe(game.Ai, 0, int.Parse(Console.ReadLine()));
            gameVsPlayer.PlayerVsAi();
            Console.WriteLine("Play again? (y/n)");
            input = Console.ReadLine();
            if (input != "Y" && input != "y")
            {
                endGame = true;
            }
        }

    }
}

void LoadLogicAiTest()
{
    Console.Clear();
    Console.WriteLine("Seed: ");
    string input = Console.ReadLine();
    Random random = new Random(int.Parse(input));
    Console.WriteLine("Training BaseTransforms. Num of training cycles per transform:");
    input = Console.ReadLine();
    int cycles = int.Parse(input);
    List<Transform> baseTransforms = new List<Transform>();
    Console.WriteLine("Maximum number in inputs?");
    input = Console.ReadLine();
    int maximum = int.Parse(input);
    baseTransforms.Add(new BaseTransform(new Addition(maximum)));
    baseTransforms.Add(new BaseTransform(new Subtraction(maximum)));

    Stopwatch timer = new Stopwatch();
    timer.Start();
    for (int i = 0; i < cycles; i++)
    {
        if (i % 1000000 == 0)
        {
            Console.WriteLine("Cycle: " + i + "    Time: "+ timer.Elapsed);
        }
        baseTransforms[0].TrainTransform(random);
        baseTransforms[1].TrainTransform(random);
    }
    timer.Stop();
    Console.WriteLine("Completed basic transform training.");
    Console.WriteLine("Cycles for Logic Training? ");
    input = Console.ReadLine();

    List<Input> aiInputs = new List<Input>();
    aiInputs.Add(new Input((ulong)maximum, 0)); // first number
    aiInputs.Add(new Input(2, 0)); //operator
    aiInputs.Add(new Input((ulong)maximum, 0));//second number
    aiInputs.Add(new Input((ulong)maximum * 2, 0));//output
    LogicAI testAi = new LogicAI(aiInputs);
    testAi.AddTransform(baseTransforms[0]);
    testAi.AddTransform(baseTransforms[1]);
    testAi.AddTransform(baseTransforms[2]);
    List<Input> desiredInputs = new List<Input>();
    foreach (var aiInput in aiInputs)
    {
        desiredInputs.Add(new Input(aiInput.MaxValue, 0));
    }


    for (int i = 0; i < cycles; i++)
    {
        if (i % 1000000 == 0)
        {
            Console.WriteLine("Cycle: " + i + "    Time: "+ timer.Elapsed);
        }
        aiInputs[0].SetValue((ulong)random.Next(0,maximum));
        aiInputs[1].SetValue((ulong)random.Next(0,1));
        if (aiInputs[1].GetValue() == 0)
            aiInputs[2].SetValue((ulong)random.Next(0, maximum));
        else
        {
            aiInputs[2].SetValue((ulong)random.Next(0, (int)aiInputs[0].GetValue()));
        }
        aiInputs[3].SetValue(0);
        for (int j = 0; j < aiInputs.Count - 1; j++)
        {
            desiredInputs[j].SetValue((ulong)aiInputs[j].GetValue());
        }

        if (aiInputs[1].GetValue() == 0)
        {
            desiredInputs[4].SetValue((ulong)(aiInputs[0].GetValue() + aiInputs[2].GetValue()) );
        }
        else
        {
            desiredInputs[4].SetValue((ulong)(aiInputs[0].GetValue() - aiInputs[2].GetValue()) );
        }
        
        testAi.Train(desiredInputs, random, 1);
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

    public Input(ulong currentValue)
    {
        MaxValue = currentValue + 1;
        CurrentValue = currentValue;
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
        else
        {
            Console.WriteLine("Attempted to set value over max.");
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

    public void SetUnknown(bool unknown)
    {
        Unknown = unknown;
    }

    public static int[] ConvertToAddress(List<Input> inputs)
    {
        int[] weightList = new int[inputs.Count - 1];
        for (int i = 0; i < inputs.Count; i++)
        {
            weightList[i] = (int)inputs[i].GetValue();
        }

        return weightList;
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

    public static void AddWeight(AIDimension matrix, double startDope)
    {
        if (matrix.Weights != null)
        {
            List<Weight> newWeights = matrix.Weights.ToList();
            newWeights.Add(new Weight(0, 0, 0));
            matrix.Weights = newWeights.ToArray();
            matrix.Weights[^1].GiveDopamine(startDope, matrix.Weights);
            
            
            
            return;
        }
        for (int i = 0; i < matrix.Positions.Count; i++)
        {
            AddWeight(matrix.Positions[i], startDope);
        }
    }

    //returns a new matrix of potential values for every unknown position
    public static AIDimension Abstract(AIDimension matrix, List<Input> situationUncopied)
    {
        List<Input> situation = new List<Input>(situationUncopied);
        if (situation[0].GetValue() == null)
        {
            situation.RemoveAt(0);
            List<AIDimension> finalPositions = new List<AIDimension>();
            //loops through each position in the dimension to create a new dimension with the lower, abstracted dimensions
            if (matrix.Weights == null)
            {
                for (int i = 0; i < matrix.Positions.Count; i++)
                {
                    finalPositions.Add(Abstract(matrix.Positions[i], situation));
                }

                AIDimension newMatrix = new AIDimension(finalPositions); 
                
                //we now have to remove all the layers that have only 1 position
                
                return RemoveExtraDimensions(newMatrix);
            }
            else
            {
                //sends the whole weight dimension
                return matrix;
            }
            

            
        }
        else if (matrix.Weights != null)
        {
            if (matrix.Weights.Length < (int)situation[0].GetValue())
            {
                throw new Exception("Bad Address Abstraction");
            }
            //sends a dimension with a single weight
            return new AIDimension(new Weight[] { matrix.Weights[(int)situation[0].GetValue()] }); 
        }
        else if((int)situation[0].GetValue() > matrix.Positions.Count)
        {
            throw new Exception("Bad Address Abstraction");
        }
        else
        {
            //if we know the value of this dimension, go to the next
            AIDimension nextDimensionDown = matrix.Positions[(int)situation[0].GetValue()];
            situation.RemoveAt(0);
            return Abstract(nextDimensionDown, situation);
        }
        
        
    }
    
    //takes an array with unneeded single-value dimensions and returns it without them
    public static AIDimension RemoveExtraDimensions(AIDimension matrix)
    {
        if (matrix.Weights == null)
        {
            if (matrix.Positions.Count > 1)
            {
                List<AIDimension> newPositions = new List<AIDimension>();
                for (int i = 0; i < matrix.Positions.Count; i++)
                {
                    AIDimension newMatrix = RemoveExtraDimensions(matrix.Positions[i]);
                    if (newMatrix.Weights != null)
                    {
                        if (newMatrix.Weights.Length == 1)
                        {
                            if (i == 0)
                            {
                                matrix.Weights = new Weight[matrix.Positions.Count];
                            }
                            matrix.Weights[i] = newMatrix.Weights[0];
                        }
                        else
                        {
                            newPositions.Add(newMatrix);
                        }
                    }
                    else
                    {
                        newPositions.Add(newMatrix);
                    }
                    
                }

                if (newPositions.Count > 0)
                {
                    return new AIDimension(newPositions);
                }
                else
                {
                    matrix.Positions = new List<AIDimension>();
                    return matrix;
                }
                
            }

            return RemoveExtraDimensions(matrix.Positions[0]);
            
        }

        return matrix;
        

        
    }

    //each array of weights this returns is the mean of the weights below it, therefore the weight that each position in this dimension is the correct one
    public static List<Weight[]> GetAbstractedWeights(AIDimension abstractMatrix)
    {
        List<Weight[]> final = new List<Weight[]>();

        if (abstractMatrix.Weights == null) //if this is not the lowest layer with the weights
        {
            List<double> totalDopeInEachPosition = new List<double>();
            double totalDope = 0;
            List<List<Weight[]>> allLowerWeights = new List<List<Weight[]>>();
            for (int i = 0; i < abstractMatrix.Positions.Count; i++) //for every position in this dimension
            {
                Console.WriteLine("Getting lower weights " + i);
                List<Weight[]> lowerWeights = GetAbstractedWeights(abstractMatrix.Positions[i]); //get the mean weight array
                allLowerWeights.Add(lowerWeights);
                double dope = GetWeightsDopamine(lowerWeights[0]); // gets the total dopamine of the next lowest dimension
                totalDopeInEachPosition.Add(dope);
                totalDope += dope;
            }

            Weight[] finalWeights = new Weight[abstractMatrix.Positions.Count];

            for (int i = 0; i < abstractMatrix.Positions.Count; i++) 
            {
                //creates a weight at each position in this array based on how much dope the lower positions have
                finalWeights[i] = new Weight(totalDopeInEachPosition[i]/totalDope, 0, totalDopeInEachPosition[i]);
                
                
            }
            List<Weight[]> finalLowerWeights = new List<Weight[]>(); 
            /* since we now have a list of weights from lower levels (allLowerWeights) we now need to create a single weight
             list for each dimension from our list of lists of weights from each dimension as split up by our current dimension's
             positions. We do that the same way we consolidate any set of weights: dopamine ratios.
             
             In allLowerWeights, the first list layer is of the abstracted weights from each dimension in each position in our 
             current dimension. The second list layer is each lower dimension itself. The weight[] layer is the weights
             for each potential position at the given dimension.
             
             Thus: at each dimensional layer (list layer 2) we take the weight lists from each of the current dimension's
             positions, and smash the values at each position in the weight[] together via dopamine ratio with respect to
             the total dopamine of each weight at that same position in their weight[] at that dimension, in all of the current
             dimension's positions
            
            */
            Console.WriteLine("Lower weights: " + allLowerWeights.Count);
            for (int i = 0; i < allLowerWeights[0].Count; i++) //each dimensional layer
            {
      
                List<Weight[]> currentDimensionsPositions = new List<Weight[]>(); //weight list from each of the current dimension's positions
                for (int j = 0; j < allLowerWeights.Count; j++) //each position in this top dimension
                {
                    currentDimensionsPositions.Add(allLowerWeights[j][i]);
                }

                totalDope = 0;
                List<double> dopeInEachLayer = new List<double>();
                for (int j = 0; j < currentDimensionsPositions[0].Length; j++) //each layer of weights in the current dimension and top position
                {
                    double totalLayerDope = 0;
                    for (int k = 0; k < currentDimensionsPositions.Count; k++)
                    {
                        //gets the dope for each layer of weights
                        totalLayerDope += currentDimensionsPositions[k][j].GetDopamine();
                        totalDope += currentDimensionsPositions[k][j].GetDopamine();
                    }
                    dopeInEachLayer.Add(totalLayerDope);
                }
                
                Weight[] smashedList = new Weight[currentDimensionsPositions[0].Length];
                for (int j = 0; j < smashedList.Length; j++) //each layer 
                {
                    smashedList[j] = new Weight(dopeInEachLayer[j] / totalDope, 0, dopeInEachLayer[j]);
                }
                
                finalLowerWeights.Add(smashedList);
                
            }
            final.Add(finalWeights); // the weight list for the current dimension
            final.AddRange(finalLowerWeights); //each weight list, now properly smashed into the other lists we now have from the current dimension
        }
        else
        {
            final.Add(abstractMatrix.Weights);
            Console.WriteLine("sent lowest weights " + final.Count);
        }
        return final;

    }

}

class Weight
{
    private double value;
    public double timesUsed; //a decimal value means it was used in an abstraction, and only added part of a time used.
    private double dopamine;
    public Weight(double startValue)
    {
        value = startValue;
        dopamine = 1;
        timesUsed = 0;
    }
    public Weight(double startValue, double startTimesUsed)
    {
        value = startValue;
        dopamine = 1;
        timesUsed = startTimesUsed;
    }
    public Weight(double startValue, double startTimesUsed, double startDopamine)
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

abstract class Game
{
    public List<Input> GameState;
    public AI Ai;
    public double TrainSpeed;
    public bool GameIsOver;

    public Game()
    {
        GameState = new List<Input>();
    }
    public abstract void UpdateGame();
    public abstract void UpdateGameAI(Action action);
    public abstract void UpdateGamePlayer(int action);
    public abstract void DisplayGame();

    public abstract void TrainAi(Random random);
    public abstract void PlayerVsAi();
    public abstract void GameOver(int winner);
    public abstract AI MakeSuitableAi();

    public abstract void Reset();

}

class TicTacToe : Game //player 1 is X player 2 is O
{
    private List<Action> AI1Actions;
    private List<Action> AI2Actions;
    private int currentTurn;
    private int RealPlayer;
    public TicTacToe(AI ai, double trainSpeed, int realPlayer)
    {
        for (int i = 0; i < 9; i++)
        {
            GameState.Add(new Input(3, 0));
        }

        Ai = ai;
        TrainSpeed = trainSpeed;
        AI1Actions = new List<Action>();
        AI2Actions = new List<Action>();
        RealPlayer = realPlayer;
        GameIsOver = false;
    }
    
    public TicTacToe( double trainSpeed, int realPlayer)
    {
        for (int i = 0; i < 9; i++)
        {
            GameState.Add(new Input(3, 0));
        }

        Ai = MakeSuitableAi();
        TrainSpeed = trainSpeed;
        AI1Actions = new List<Action>();
        AI2Actions = new List<Action>();
        RealPlayer = realPlayer;
        GameIsOver = false;
    }
    public override void TrainAi(Random random)
    {
        currentTurn = 1;
        while (!GameIsOver)
        {
            UpdateGameAI(AI.GetRandomActionKnown(Ai.AiMatrix, GameState, random));
        }

    }

    public override void PlayerVsAi()
    {
        currentTurn = 1;
        while (!GameIsOver)
        {
            Console.WriteLine("Current turn: " + currentTurn);
            if (RealPlayer == currentTurn)
            {
                DisplayGame();
                Console.WriteLine("It is your move. Squares are numbered 1-9, starting from the top left, input square to change. If you input something other than a number, the program will crash lol.");
                UpdateGamePlayer(int.Parse(Console.ReadLine()) - 1);
            }
            else 
            {
                Console.WriteLine("AI plays at:" + (AI.GetBestActionKnown(Ai.AiMatrix, GameState).Output + 1));
                UpdateGameAI(AI.GetBestActionKnown(Ai.AiMatrix, GameState));
            }
        }

    }

    public override void UpdateGamePlayer(int action)
    {
        if (GameState[action].GetValue() != 0)
        {
            Console.WriteLine("That space is taken, try again.");
        }
        else
        {
            if (currentTurn == 1)
            {
                currentTurn = 2;
                GameState[action].SetValue(1);
            }
            else
            {
                currentTurn = 1;
                GameState[action].SetValue(2);
            }
            UpdateGame();
        }
        
    }

    public override void UpdateGameAI(Action action)
    {
        if (GameState[action.Output].GetValue() != 0)
        {
            action.Weight.GiveDopamine(-1, action.WeightList);
        }
        else
        {
            if (currentTurn == 1)
            {
                AI1Actions.Add(action);
                currentTurn = 2;
                GameState[action.Output].SetValue(1);
            }
            else
            {
                AI2Actions.Add(action);
                currentTurn = 1;
                GameState[action.Output].SetValue(2);
            }
            UpdateGame();
        }
        
    }

    public override void UpdateGame()
    {
        for (int y = 0; y <= 2; y++)
        {
            if (GameState[0 + y * 3].GetValue() == GameState[1 + y * 3].GetValue() && GameState[0 + y * 3].GetValue() == GameState[2 + y * 3].GetValue() && GameState[0 + y * 3].GetValue() != 0) // triple on the row
            {
                GameOver((int)GameState[0 + y * 3].GetValue());
                
            }
            else if (GameState[0 + y].GetValue() == GameState[3 + y].GetValue() && GameState[0 + y].GetValue() == GameState[6 + y].GetValue()&& GameState[0 + y].GetValue() != 0) // triple on the column
            {
                GameOver((int)GameState[0 + y].GetValue());
                
            }
            
        }

        if (GameState[2].GetValue() == GameState[4].GetValue() && GameState[2].GetValue() == GameState[6].GetValue()&& GameState[2].GetValue() != 0) //triple on bottom-to-top diagonal
        {
            GameOver((int)GameState[2].GetValue());
            
        }
        else if (GameState[0].GetValue() == GameState[4].GetValue() &&
                 GameState[0].GetValue() == GameState[8].GetValue()&& GameState[0].GetValue() != 0) //triple on top-to-bottom diagonal
        {
            GameOver((int)GameState[0].GetValue());
            
        }

        for (int i = 0; i < 9; i++)
        {
            if (GameState[i].GetValue() == 0)
            {
                return;
            }
        }
        GameOver(0);


    }

    public override void GameOver(int winner)
    {
        double dopeMulti = 1;
        if (winner == 1)
        {
            if (RealPlayer == 1)
            {
                Console.WriteLine("Congratz! You won.");
            }
            else
            {
                if (RealPlayer == 2)
                {
                    Console.WriteLine("Lmao! You just got beat by someone who started playing less than a minute ago!");
                }
                for (int i = AI1Actions.Count - 1; i >= 0; i--)
                {
                    AI1Actions[i].Weight.GiveDopamine(TrainSpeed * dopeMulti, AI1Actions[i].WeightList);
                    dopeMulti /= 2;

                }
            }
            
        }
        else if (winner == 2)
        {
            if (RealPlayer == 2)
            {
                Console.WriteLine("Congratz! You won.");
            }
            else
            {
                if (RealPlayer == 1)
                {
                    Console.WriteLine("Lmao! You just got beat by someone who started playing less than a minute ago!");
                }
                for (int i = AI2Actions.Count - 1; i >= 0; i--)
                {
                    AI2Actions[i].Weight.GiveDopamine(TrainSpeed * dopeMulti, AI2Actions[i].WeightList);
                    dopeMulti /= 2;
                }
            }
            
            
        }
        else
        {
            if (RealPlayer == 1 || RealPlayer == 2)
            {
                Console.WriteLine("Cat's Game.");
            }
        }

        GameIsOver = true;
    }

    public override void DisplayGame()
    {
        Console.WriteLine();
        for (int i = 0; i < 9; i++)
        {
            
            if (GameState[i].GetValue() == 0)
            {
                Console.Write( (i +1) + "|");
            }
            else if (GameState[i].GetValue() == 1)
            {
                Console.Write("X|");
            }
            else if (GameState[i].GetValue() == 2)
            {
                Console.Write("O|");
            }
            if ((i + 1) % 3 == 0 && i != 8)
            {
                Console.WriteLine("\n_______");
            }
        }
        Console.WriteLine();

    }

    public override AI MakeSuitableAi()
    {
        return new AI(9,GameState);
    }

    public override void Reset()
    {
        AI1Actions = new List<Action>();
        AI2Actions = new List<Action>();
        GameState = new List<Input>();
        for (int i = 0; i < 9; i++)
        {
            GameState.Add(new Input(3, 0));
        }

        GameIsOver = false;
    }
}

class Addition : Game
{
    private int MaxInputSize = 0;

    public Addition(int maxInputSize)
    {
        MaxInputSize = maxInputSize;
        GameState = new List<Input>();
        GameState.Add(new Input((ulong)MaxInputSize, 0));
    }
    
    public override void Reset()
    {
        GameState[0].SetValue(0);
        GameState[1].SetValue(0);
    }
    

    public override void TrainAi(Random random)
    {
        GameState[0].SetValue((ulong)random.Next(MaxInputSize));
        GameState[1].SetValue((ulong)random.Next(MaxInputSize));
        Action output = AI.GetRandomActionKnown(Ai.AiMatrix, GameState, random);
        if (output.Output == (int)(GameState[0].GetValue() + GameState[1].GetValue()))
        {
            output.GiveDopamine(1 * TrainSpeed);
        }
    }

    public override void DisplayGame()
    {
        Console.WriteLine($"{GameState[0]} + {GameState[1]} = ");
    }

    

    public override void PlayerVsAi()
    {
        Console.WriteLine("Input 1st number to add (integer):");
        string input = Console.ReadLine();
        GameState[0].SetValue((ulong)int.Parse(input));
        Console.WriteLine("Second number (integer):");
        input = Console.ReadLine();
        GameState[1].SetValue((ulong)int.Parse(input));
        Console.WriteLine("AI says: " + AI.GetBestActionKnown(Ai.AiMatrix, GameState));
    }

    public override AI MakeSuitableAi()
    {
        return new AI(MaxInputSize * 2, GameState);
    }
    
    public override void GameOver(int winner)
    {
        throw new NotImplementedException();
    }

    public override void UpdateGameAI(Action action)
    {
        throw new NotImplementedException();
    }

    public override void UpdateGame()
    {
        throw new NotImplementedException();
    }

    public override void UpdateGamePlayer(int action)
    {
        throw new NotImplementedException();
    }
}
class Subtraction : Game
{
    private int MaxInputSize = 0;

    public Subtraction(int maxInputSize)
    {
        MaxInputSize = maxInputSize;
        GameState = new List<Input>();
        GameState.Add(new Input((ulong)MaxInputSize, 0));
    }
    
    public override void Reset()
    {
        GameState[0].SetValue(0);
        GameState[1].SetValue(0);
    }
    

    public override void TrainAi(Random random)
    {
        GameState[0].SetValue((ulong)random.Next(MaxInputSize));
        GameState[1].SetValue((ulong)random.Next(MaxInputSize));
        Action output = AI.GetRandomActionKnown(Ai.AiMatrix, GameState, random);
        if (output.Output == (int)(GameState[0].GetValue() - GameState[1].GetValue()))
        {
            output.GiveDopamine(1 * TrainSpeed);
        }
    }

    public override void DisplayGame()
    {
        Console.WriteLine($"{GameState[0]} - {GameState[1]} = ");
    }

    

    public override void PlayerVsAi()
    {
        Console.WriteLine("Input 1st number to subtract (integer):");
        string input = Console.ReadLine();
        GameState[0].SetValue((ulong)int.Parse(input));
        Console.WriteLine("Second number (integer, less than 1st):");
        input = Console.ReadLine();
        GameState[1].SetValue((ulong)int.Parse(input));
        Console.WriteLine("AI says: " + AI.GetBestActionKnown(Ai.AiMatrix, GameState));
    }

    public override AI MakeSuitableAi()
    {
        return new AI(MaxInputSize, GameState);
    }
    
    public override void GameOver(int winner)
    {
        throw new NotImplementedException();
    }

    public override void UpdateGameAI(Action action)
    {
        throw new NotImplementedException();
    }

    public override void UpdateGame()
    {
        throw new NotImplementedException();
    }

    public override void UpdateGamePlayer(int action)
    {
        throw new NotImplementedException();
    }
}

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



abstract class Transform //this is the lowest level of transform, performed a by pre-trained feeling AI
{
    public int NumOfInputs;
    public int NumOfOutputs;
    public abstract void DoTransform(List<Input> inputs, List<int> outputIndex);

    public abstract void TrainTransform(Random random);



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

    public override void DoTransform(List<Input> inputs, List<int> outputIndex)
    {
        inputs[outputIndex[0]].SetValue((ulong)AI.GetBestActionKnown(TransformAi.AiMatrix, inputs).Output);
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
    }

    public override void DoTransform(List<Input> inputs, List<int> outputIndex)
    {
        for (int i = 0; i < Transforms.Count; i++)
        {
            Transforms[i].DoTransform(inputs, outputIndex);
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


class LogicAI //basically an AI trained to make SeriesTransforms to solve problems
{
    public List<Transform> AllTransforms;
    public AI TransformChooserAI; // output 0 stops adding new transforms to the list
    public AI InputChooserAI; //Chooses which inputs to put into each transform
    public List<Input> GivenSituation; //We can change the inputs themselves after initializing, but not how many of them

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
        TransformChooserAI = new AI(0, inputs);
        GivenSituation = inputs;
        AllTransforms = new List<Transform>();
    }
    public LogicOutput GetActionRandom(Random random)
    {
        bool stop = false;
        List<Transform> transformsUndergone = new List<Transform>();
        List<int> transformIndexes = new List<int>();
        while (!stop)
        {
            int chosenTransform = AI.GetRandomActionKnown(TransformChooserAI.AiMatrix, GivenSituation, random).Output;
            if (chosenTransform != 0)
            {
                transformsUndergone.Add(AllTransforms[chosenTransform]);
                transformIndexes.Add(chosenTransform);
            }
            else
            {
                return new LogicOutput(transformsUndergone, transformIndexes);
            }
            
        }

        throw new Exception("should never happen.");
    }

    public void Train(List<Input> desiredOutput, Random random, double trainSpeed)
    {
        LogicOutput output = GetActionRandom(random);
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
            int transformIndex = output.TransformIndexes[i];
            Weight[] weights = AIDimension.GetWeights(TransformChooserAI.AiMatrix,
                Input.ConvertToAddress(GivenSituation)); // gets the Ai's opinion on each situation
            weights[transformIndex].GiveDopamine(1/(double)(incorrectOutputs + 1) * trainSpeed, weights); //weights it based on how right it was.
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
    

    

}

class LogicOutput
{
    public List<Transform> Transforms;
    public List<int> TransformIndexes;

    public LogicOutput(List<Transform> transforms, List<int> transformIndexes)
    {
        Transforms = transforms;
        TransformIndexes = transformIndexes;
    }
    
}











