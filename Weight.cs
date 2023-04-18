namespace RatMaze;

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