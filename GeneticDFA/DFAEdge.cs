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

    public int Id { get; set; }
    public DFAState Source { get; set; }
    public DFAState Target { get; set; }
    public char Input { get; set; }

    public DFAEdge Clone(DFAState source, DFAState target)
    {
        return new DFAEdge(Id, source, target, Input);
    }
}
