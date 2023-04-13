namespace GeneticDFA;

public class DFAState
{
    public DFAState(int id, bool isAccept)
    {
        ID = id;
        IsAccept = isAccept;
    }

    public int ID { get; }
    public bool IsAccept { get; set; }
}
