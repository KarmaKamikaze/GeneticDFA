namespace GeneticDFA;

public class DFAEdgeModel
{
    public DFAEdgeModel(int id, DFAStateModel source, DFAStateModel target, string input)
    {
        ID = id;
        Source = source;
        Target = target;
        Input = input;
    }
    
    public int ID { get; }
    public DFAStateModel Source { get; set; }
    public DFAStateModel Target { get; set; }
    public string Input { get; set; } //Can maybe be made a char, depends on how input alphabet will be defined.

}
