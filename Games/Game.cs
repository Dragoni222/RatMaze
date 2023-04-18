namespace RatMaze.Games;

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