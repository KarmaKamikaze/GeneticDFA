namespace GeneticDFA;

public class DFAStateModel
{
    public DFAStateModel(int id, bool isAccept)
    {
        ID = id;
        IsAccept = isAccept;
    }

    public int ID { get; }
    public bool IsAccept { get; set; }

}
