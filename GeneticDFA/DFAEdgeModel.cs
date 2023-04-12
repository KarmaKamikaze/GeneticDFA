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
    public char Input { get; set; } 

}
