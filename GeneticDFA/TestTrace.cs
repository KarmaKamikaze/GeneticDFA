namespace GeneticDFA;

public class TestTrace
{
    public TestTrace(string trace, bool isAccepting)
    {
        Trace = trace;
        IsAccepting = isAccepting;
    }

    public string Trace { get; }
    public bool IsAccepting { get; }
    
}
