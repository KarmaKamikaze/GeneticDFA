namespace GeneticDFA;

public class DFAEdgeModel
{
    public DFAEdgeModel(int id, DFAStateModel source, DFAStateModel target, char input)
    {
        ID = id;
        Source = source;
        Target = target;
        Input = input;
    }
    
    public int ID { get; }
    public DFAStateModel Source { get; set; }
    public DFAStateModel Target { get; set; }
    //Can be a string if input symbols are more than one letter.
    //HOWEVER, this will increase computational complexity in the ExploreState method,
    //which is most likely the method that will be called the most.
    //Therefore, we should probably convert input symbols to chars and force them to be one letter.
    public char Input { get; set; } 

}
