using GeneticSharp;
using System.Threading;

namespace GeneticDFA;

public class DFAChromosome : IChromosome
{
    private static int _idIncrement;

    public List<DFAState> States { get; } = new List<DFAState>();
    public List<DFAEdge> Edges { get; } = new List<DFAEdge>();
    public List<DFAEdge> NonDeterministicEdges { get; set; } = new List<DFAEdge>();
    public DFAState? StartState { get; set; }
    public double? Fitness { get; set; }
    public int Size => States.Count + Edges.Count;
    public int NextStateId { get; set; } = 0;
    public int NextEdgeId { get; set; } = 0;
    public int Id { get; private set; }

    public DFAChromosome(List<DFAState> states, List<DFAEdge> edges, DFAState startState)
    {
        Id = Interlocked.Increment(ref _idIncrement);
        States = states;
        Edges = edges;
        StartState = startState;
    }

    public DFAChromosome(List<DFAState> states, List<DFAEdge> edges, DFAState startState, int id)
        : this(states, edges, startState)
    {
        _idIncrement = _idIncrement < id ? id : _idIncrement;
    }

    public DFAChromosome()
    {
    }

    public IChromosome CreateNew()
    {
        DFAChromosome chromosome = new DFAChromosome();
        chromosome.SetNewId();
        return chromosome;
    }

    public IChromosome Clone()
    {
        DFAChromosome chromosome = new DFAChromosome();

        chromosome.Id = Id;

        foreach (DFAState state in States)
        {
            chromosome.States.Add(state.Clone());
        }


        foreach (DFAEdge edge in Edges)
        {
            chromosome.Edges.Add(edge.Clone(
                chromosome.States.Find(s => s.Id == edge.Source.Id)!,
                chromosome.States.Find(s => s.Id == edge.Target.Id)!));
        }

        chromosome.StartState = chromosome.States.Find(s => s.Id == StartState!.Id);
        chromosome.Fitness = Fitness;
        chromosome.NextStateId = NextStateId;
        chromosome.NextEdgeId = NextEdgeId;

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
        return !(fitness2.GetValueOrDefault() > nullable1.GetValueOrDefault() & fitness2.HasValue & nullable1.HasValue)
            ? -1
            : 1;
    }

    public void SetNewId()
    {
        Id = Interlocked.Increment(ref _idIncrement);
    }


    // Code below here will not be used and is simply here to satisfy the interface.
    // Other classes dependent on these properties and methods are going to be rewritten.

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
