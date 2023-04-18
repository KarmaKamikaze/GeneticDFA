using GeneticSharp;

namespace GeneticDFA;

public static class DFAChromosomeHelper
{
    // Does not fit in this class, but the method will be used by both Population class and Crossover class
    public static void FixUnreachability(DFAChromosome chromosome, List<char> alphabet)
    {
        while (true)
        {
            List<DFAState> reachableStates = FindReachableStates(chromosome);
            if (reachableStates.Count == chromosome.States.Count)
                break;
            List<DFAState> unreachableStates = chromosome.States.Where(s => !reachableStates.Contains(s)).ToList();

            // Note that we do not check whether the edge we are going to add is unique
            // (contrast to InitializeChromosomeEdges() in DFAPopulation). This is because we know that if a state is
            // unreachable, that must mean that no reachable state has an outgoing edge to it.
            // Thus all edges pointing to unreachable states will be unique.
            DFAState source = reachableStates[RandomizationProvider.Current.GetInt(0, reachableStates.Count)];
            char input = alphabet[RandomizationProvider.Current.GetInt(0, alphabet.Count)];
            DFAState target = unreachableStates[RandomizationProvider.Current.GetInt(0, unreachableStates.Count)];
            chromosome.Edges.Add(new DFAEdge(chromosome.NextEdgeId, source, target, input));
            chromosome.NextEdgeId++;
        }
    }

    private static List<DFAState> FindReachableStates(DFAChromosome chromosome)
    {
        List<DFAState> reachableStates = new List<DFAState> { chromosome.StartState! };

        while (true)
        {
            List<DFAEdge> edgesFromReachableStates =
                chromosome.Edges.Where(e => reachableStates.Contains(e.Source)).ToList();
            List<DFAState> newReachableStates =
                chromosome.States.Where(s => edgesFromReachableStates.Any(e => e.Target == s)).ToList();
            newReachableStates.RemoveAll(s => reachableStates.Contains(s));
            if (newReachableStates.Count == 0)
                break;
            reachableStates.AddRange(newReachableStates);
        }

        return reachableStates;
    }

    // Does not fit in this class, but the method will be used by both Population class and Mutation class
    public static void FindAndAssignNonDeterministicEdges(DFAChromosome chromosome)
    {
        chromosome.NonDeterministicEdges = new List<DFAEdge>();

        if (chromosome.Edges.Count < 2)
            return;

        // Sort the edges so that non-deterministic edges are grouped.
        // Example with edges as (Source, Input, Target):
        // {(A,1,B), (B,0,A), (A,0,B), (A,1,C), (C,0,C), (B,1,C)}->{(A,1,B), (A,1,C), (A,0,B), (B,0,A), (B,1,C), (C,0,C)}
        chromosome.Edges.Sort(delegate(DFAEdge edge1, DFAEdge edge2)
        {
            int areSourcesEqual = edge1.Source.Id.CompareTo(edge2.Source.Id);
            return areSourcesEqual == 0 ? edge1.Input.CompareTo(edge2.Input) : areSourcesEqual;
        });

        // Iterate through the edges and check if each edge has same source and input as a neighbor
        // Special cases for the first and last edge to avoid indexing out of range,
        // since they are at the ends of the array and thus only have 1 neighbor
        if (chromosome.Edges[0].Source.Id == chromosome.Edges[1].Source.Id &&
            chromosome.Edges[0].Input == chromosome.Edges[1].Input)
            chromosome.NonDeterministicEdges.Add(chromosome.Edges[0]);

        for (int i = 1; i < chromosome.Edges.Count - 1; i++)
        {
            if ((chromosome.Edges[i].Source.Id == chromosome.Edges[i + 1].Source.Id &&
                 chromosome.Edges[i].Input == chromosome.Edges[i + 1].Input) ||
                (chromosome.Edges[i].Source.Id == chromosome.Edges[i - 1].Source.Id &&
                 chromosome.Edges[i].Input == chromosome.Edges[i - 1].Input))
            {
                chromosome.NonDeterministicEdges.Add(chromosome.Edges[i]);
            }
        }

        if (chromosome.Edges[^1].Source.Id == chromosome.Edges[^2].Source.Id &&
            chromosome.Edges[^1].Input == chromosome.Edges[^2].Input)
            chromosome.NonDeterministicEdges.Add(chromosome.Edges[^1]);
    }
}
