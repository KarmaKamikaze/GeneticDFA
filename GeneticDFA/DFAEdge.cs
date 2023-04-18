namespace GeneticDFA;

public class DFAEdge
{
    public DFAEdge(int id, DFAState source, DFAState target, char input)
    {
        Id = id;
        Source = source;
        Target = target;
        Input = input;
    }

    public int Id { get; }
    public DFAState Source { get; set; }
    public DFAState Target { get; set; }
    public char Input { get; set; }
}
