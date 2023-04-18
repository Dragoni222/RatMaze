namespace RatMaze.Games;

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