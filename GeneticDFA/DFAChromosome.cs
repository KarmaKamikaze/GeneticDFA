using GeneticSharp;

namespace GeneticDFA;

public class DFAChromosome : IChromosome
{
    public List<DFAStateModel> States { get; } = new List<DFAStateModel>();
    public List<DFAEdgeModel> Edges { get; } = new List<DFAEdgeModel>();
    public List<DFAEdgeModel> NonDeterministicEdges { get; private set; } = new List<DFAEdgeModel>();
    public DFAStateModel StartState { get; }
    public double? Fitness { get; set; }
    public int Size => States.Count + Edges.Count;
    private int _nextStateID = 0;
    private int _nextEdgeID = 0;
    

    public IChromosome CreateNew()
    {
        return new DFAChromosome();
    }

    public void FindAndAssignNonDeterministicEdges()
    {
        List<DFAEdgeModel> edges = Edges;
        NonDeterministicEdges = new List<DFAEdgeModel>();

        if (edges.Count < 2)
            return;
        
        edges.Sort(delegate(DFAEdgeModel edge1, DFAEdgeModel edge2)
        {
            int areSourcesEqual = edge1.Source.ID.CompareTo(edge2.Source.ID);
            return areSourcesEqual == 0 ? edge1.Input.CompareTo(edge2.Input) : areSourcesEqual;
        });

        if(edges[0].Source.ID == edges[1].Source.ID && edges[0].Input == edges[1].Input)
            NonDeterministicEdges.Add(edges[0]);
        
        for (int i = 1; i < edges.Count-1; i++)
        {
            if ((edges[i].Source.ID == edges[i + 1].Source.ID && edges[i].Input == edges[i + 1].Input) ||
                (edges[i].Source.ID == edges[i - 1].Source.ID && edges[i].Input == edges[i - 1].Input))
            {
                NonDeterministicEdges.Add(edges[i]);
            }
        }

        if(edges[^1].Source.ID == edges[^2].Source.ID && edges[^1].Input == edges[^2].Input)
            NonDeterministicEdges.Add(edges[^1]);
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
    
    
    //Code below here will not be used and is simply here to satisfy the interface.
    //Other classes dependent on these properties and methods are going to be rewritten.
    
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
