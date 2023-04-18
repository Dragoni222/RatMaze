namespace RatMaze.Games;

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