namespace GeneticDFA;

public class TraceModel
{
    public TraceModel(string trace, bool isAccepting)
    {
        Trace = trace;
        IsAccepting = isAccepting;
    }

    public string Trace { get; }
    public bool IsAccepting { get; }
    
}
