// 0 = open 1 = wall 2 = goal 3 = player

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
        try
        {
            answerInt = int.Parse(answer);
        }
        catch(FormatException e)
        {
            answerInt = 1;
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
    private List<AIDimension> aiMatrix = new List<AIDimension>();
}

class AIDimension
{
    public List<AIDimension> Positions;
    public double[] Weights; //only the lowest dimension has weights

    public AIDimension(int NumOfPossibleValues, AIDimension currentMatrix, int currentValue ,bool copy)
    {

        Positions = new List<AIDimension>();
        if (copy)
        {
            for (int i = 0; i < NumOfPossibleValues; i++)
            {
                Positions.Add(currentMatrix);
            }
        }
        else
        {
            AIDimension unweighted = AIDimension.UnweightedMatrix(currentMatrix);
            for (int i = 0; i < NumOfPossibleValues; i++)
            {
                Positions.Add(unweighted);
            }
        }
        
        
    }

    public AIDimension(double[] weights)
    {
        Weights = weights;
    }

    public AIDimension(int length)
    {
        Weights = new double[length];
        for (int i = 0; i < length; i++)
        {
            Weights[i] = 0;
        }
    }

    public static AIDimension UnweightedMatrix(AIDimension weightedMatrix)
    {
        AIDimension unweighted = weightedMatrix;
        if (unweighted.Weights != null)
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
                unweighted.Positions[i] = AIDimension.UnweightedMatrix(unweighted.Positions[i]);
            }
        }

        return unweighted;

    }

    public void ChangeWeight(int[] address, double newValue)
    {
        if (address.Length == 1)
        {
            try
            {
                Weights[address[0]] = newValue;
            }
            catch (Exception e)
            {
                Console.Write("Wrong Address");
            }
        }
    }

}





