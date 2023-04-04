using GeneticSharp;

namespace GeneticDFA;

public class DFAChromosome : IChromosome
{
    
    public List<DFAStateModel> States = new List<DFAStateModel>();
    public List<DFAEdgeModel> Edges = new List<DFAEdgeModel>();
    public int StartStateID;
    public double? Fitness { get; set; }
    private int _nextStateID = 0;
    private int _nextEdgeID = 0;

    public IChromosome CreateNew()
    {
        return new DFAChromosome();
    }

    public IChromosome Clone()
    {
        IChromosome chromosome = CreateNew();
        //Implement copying of states and edges and stuff. 
        chromosome.Fitness = Fitness;
        return chromosome;
    }
    

    public int CompareTo(IChromosome? other)
    {
        if (other == null)
            return -1;
        double? fitness1 = other.Fitness;
        double? nullable1 = this.Fitness;
        double? nullable2 = fitness1;
        if (nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault() & nullable1.HasValue == nullable2.HasValue)
            return 0;
        double? fitness2 = this.Fitness;
        nullable1 = fitness1;
        return !(fitness2.GetValueOrDefault() > nullable1.GetValueOrDefault() & fitness2.HasValue & nullable1.HasValue) ? -1 : 1;
    }
    
    
    public static bool operator ==(DFAChromosome first, DFAChromosome second)
    {
        if ((object) first == (object) second)
            return true;
        return (object) first != null && (object) second != null && first.CompareTo((IChromosome) second) == 0;
    }

    public static bool operator !=(DFAChromosome first, DFAChromosome second) => !(first == second);

    public static bool operator <(DFAChromosome first, DFAChromosome second)
    {
        if ((object) first == (object) second)
            return false;
        if ((object) first == null)
            return true;
        return (object) second != null && first.CompareTo((IChromosome) second) < 0;
    }

    public static bool operator >(DFAChromosome first, DFAChromosome second) => !(first == second) && !(first < second);
    
    
    
    
    
    public int Length { get; }
    
    public Gene GenerateGene(int geneIndex)
    {
        throw new NotImplementedException("// TODO: Generate a gene base on DFAChromosome representation.");
    }

    public void ReplaceGene(int index, Gene gene)
    {
        throw new NotImplementedException();
    }

    public void ReplaceGenes(int startIndex, Gene[] genes)
    {
        throw new NotImplementedException();
    }

    public void Resize(int newLength)
    {
        throw new NotImplementedException();
    }

    public Gene GetGene(int index)
    {
        throw new NotImplementedException();
    }

    public Gene[] GetGenes()
    {
        throw new NotImplementedException();
    }
    
}
