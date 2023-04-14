using GeneticSharp;

namespace GeneticDFA;

public class DFAChromosome : IChromosome
{
    public List<DFAState> States { get; } = new List<DFAState>();
    public List<DFAEdge> Edges { get; } = new List<DFAEdge>();
    public List<DFAEdge> NonDeterministicEdges { get; private set; } = new List<DFAEdge>();
    public DFAState StartState { get; set; }
    public double? Fitness { get; set; }
    public int Size => States.Count + Edges.Count;
    public int NextStateID { get; set; } = 0;
    public int NextEdgeID { get; set; } = 0;

    public DFAChromosome(List<DFAState> states, List<DFAEdge> edges, DFAState startState)
    {
        States = states;
        Edges = edges;
        StartState = startState;
    }

    public DFAChromosome()
    {
        
    }


    public IChromosome CreateNew()
    {
        return new DFAChromosome();
    }
    
    
    private List<DFAState> FindReachableStates()
    {
        List<DFAState> reachableStates = new List<DFAState> {StartState};

        while (true)
        {
            List<DFAEdge> edgesFromReachableStates = Edges.Where(e => reachableStates.Contains(e.Source)).ToList();
            List<DFAState> newReachableStates = States.Where(s => edgesFromReachableStates.Any(e => e.Target == s)).ToList();
            newReachableStates.RemoveAll(s => reachableStates.Contains(s));
            if (newReachableStates.Count == 0)
                break;
            reachableStates.AddRange(newReachableStates);
        }

        return reachableStates;
    }

    public void FixUnreachability(List<char> alphabet)
    {
        while (true)
        {
            List<DFAState> reachableStates = FindReachableStates();
            if (reachableStates.Count == States.Count)
                break;
            List<DFAState> unreachableStates = States.Where(s => !reachableStates.Contains(s)).ToList();
            DFAState source = reachableStates[RandomizationProvider.Current.GetInt(0, reachableStates.Count)];
            char input = alphabet[RandomizationProvider.Current.GetInt(0, alphabet.Count)];
            DFAState target = unreachableStates[RandomizationProvider.Current.GetInt(0, unreachableStates.Count)];
            Edges.Add(new DFAEdge(NextEdgeID, source, target, input));
            NextEdgeID++;
        }
    }

    //Move to mutation and crossover
    public void FindAndAssignNonDeterministicEdges()
    {
        NonDeterministicEdges = new List<DFAEdge>();

        if (Edges.Count < 2)
            return;
        
        //Sort the edges so that non-deterministic edges are grouped.
        //Example with edges as (Source, Input, Target):
        //{(A,1,B), (B,0,A), (A,0,B), (A,1,C), (C,0,C), (B,1,C)}->{(A,1,B), (A,1,C), (A,0,B), (B,0,A), (B,1,C), (C,0,C)}
        Edges.Sort(delegate(DFAEdge edge1, DFAEdge edge2)
        {
            int areSourcesEqual = edge1.Source.ID.CompareTo(edge2.Source.ID);
            return areSourcesEqual == 0 ? edge1.Input.CompareTo(edge2.Input) : areSourcesEqual;
        });

        //Iterate through the edges and check if each edge has same source and input as a neighbor
        //Special cases for the first and last edge to avoid indexing out of range,
        //since they are at the ends of the array and thus only have 1 neighbor
        if(Edges[0].Source.ID == Edges[1].Source.ID && Edges[0].Input == Edges[1].Input)
            NonDeterministicEdges.Add(Edges[0]);
        
        for (int i = 1; i < Edges.Count-1; i++)
        {
            if ((Edges[i].Source.ID == Edges[i + 1].Source.ID && Edges[i].Input == Edges[i + 1].Input) ||
                (Edges[i].Source.ID == Edges[i - 1].Source.ID && Edges[i].Input == Edges[i - 1].Input))
            {
                NonDeterministicEdges.Add(Edges[i]);
            }
        }

        if(Edges[^1].Source.ID == Edges[^2].Source.ID && Edges[^1].Input == Edges[^2].Input)
            NonDeterministicEdges.Add(Edges[^1]);
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
