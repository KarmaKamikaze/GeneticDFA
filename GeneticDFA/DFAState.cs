namespace GeneticDFA;

public class DFAState
{
    public DFAState(int id, bool isAccept)
    {
        Id = id;
        IsAccept = isAccept;
    }

    public int Id { get; }
    public bool IsAccept { get; set; }
}
