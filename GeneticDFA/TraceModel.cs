namespace GeneticDFA;

public class TraceModel
{
    public TraceModel(string trace, int length, bool isAccepting)
    {
        Trace = trace;
        Length = length;
        IsAccepting = isAccepting;
    }

    public string Trace { get; set; }
    public int Length { get; set; }
    public bool IsAccepting { get; set; }
    
}
