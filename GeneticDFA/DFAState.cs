namespace GeneticDFA;

public class DFAState
{
    public DFAState(int id, bool isAccept)
    {
        Id = id;
        IsAccept = isAccept;
    }

    public int Id { get; set; }
    public bool IsAccept { get; set; }

    public DFAState Clone()
    {
        return new DFAState(Id, IsAccept);
    }
}
