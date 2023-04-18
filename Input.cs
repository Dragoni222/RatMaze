namespace RatMaze;

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