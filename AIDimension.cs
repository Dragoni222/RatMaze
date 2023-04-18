namespace RatMaze;

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
        }
        return final;

    }

}