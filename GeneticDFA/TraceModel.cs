namespace GeneticDFA;

public class TraceModel
{
    public TraceModel(string trace, int length, bool isAccepting)
    {
        Trace = trace;
        Length = length;
        IsAccepting = isAccepting;
    }

    public string Trace { get; }
    public int Length { get; set; } //Not currently in use. Maybe later, but cannot see use case.
    public bool IsAccepting { get; }
    
}
