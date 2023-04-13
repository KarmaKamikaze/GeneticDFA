namespace GeneticDFA;

public class DFAEdge
{
    public DFAEdge(int id, DFAState source, DFAState target, char input)
    {
        ID = id;
        Source = source;
        Target = target;
        Input = input;
    }
    
    public int ID { get; }
    public DFAState Source { get; set; }
    public DFAState Target { get; set; }
    public char Input { get; set; } 

}
