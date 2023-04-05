using GeneticSharp;

namespace GeneticDFA;

public class DFAFitness : IFitness
{
    public DFAFitness(List<TraceModel> traces, List<string> alphabet, double weightTruePositive, double weightTrueNegative, double weightFalsePositive, double weightFalseNegative, double weightNonDeterministicEdges, double weightMissingDeterministicEdges, double weightSize)
    {
        Traces = traces;
        Alphabet = alphabet;
        WeightTruePositive = weightTruePositive;
        WeightTrueNegative = weightTrueNegative;
        WeightFalsePositive = weightFalsePositive;
        WeightFalseNegative = weightFalseNegative;
        WeightNonDeterministicEdges = weightNonDeterministicEdges;
        WeightMissingDeterministicEdges = weightMissingDeterministicEdges;
        WeightSize = weightSize;
    }

    private List<TraceModel> Traces { get; set; }
    private List<string> Alphabet { get; set; }
    private double WeightTruePositive { get; set; }
    private double WeightTrueNegative { get; set; }
    private double WeightFalsePositive { get; set; }
    private double WeightFalseNegative { get; set; }
    private double WeightNonDeterministicEdges { get; set; }
    private double WeightMissingDeterministicEdges { get; set; }
    private double WeightSize { get; set; }
    
    private enum Verdicts
    {
        TruePositive,
        TrueNegative,
        FalsePositive,
        FalseNegative
    }

    public double Evaluate(IChromosome paramChromosome)
    {
        DFAChromosome chromosome = (DFAChromosome) paramChromosome;

        return NumberOfMissingDeterministicEdges(chromosome);
    }

    //Can be moved to the Chromosome class.
    private List<DFAEdgeModel> FindNonDeterministicEdges(DFAChromosome chromosome)
    {
        List<DFAEdgeModel> edges = chromosome.Edges;
        List<DFAEdgeModel> nonDeterministicEdges = new List<DFAEdgeModel>();
        
        edges.Sort(delegate(DFAEdgeModel edge1, DFAEdgeModel edge2)
        {
            int areSourcesEqual = edge1.Source.ID.CompareTo(edge2.Source.ID);
            return areSourcesEqual == 0 ? string.Compare(edge1.Input, edge2.Input, StringComparison.Ordinal) : areSourcesEqual;
        });

        if(edges[0].Source.ID == edges[1].Source.ID && edges[0].Input == edges[1].Input)
            nonDeterministicEdges.Add(edges[0]);
        
        for (int i = 1; i < edges.Count-1; i++)
        {
            if ((edges[i].Source.ID == edges[i + 1].Source.ID && edges[i].Input == edges[i + 1].Input) ||
                (edges[i].Source.ID == edges[i - 1].Source.ID && edges[i].Input == edges[i - 1].Input))
            {
                nonDeterministicEdges.Add(edges[i]);
            }
        }

        if(edges[^1].Source.ID == edges[^2].Source.ID && edges[^1].Input == edges[^2].Input)
            nonDeterministicEdges.Add(edges[^1]);
        
        return nonDeterministicEdges;
    }
    
    
    //Can be moved to chromosome class, but it requires passing of the alphabet.
    private int NumberOfMissingDeterministicEdges(DFAChromosome chromosome)
    {
        int missingDeterministicEdgesCounter = 0;
        int missingDeterministicEdgesForState = Alphabet.Count;
        
        foreach (DFAStateModel state in chromosome.States)
        {
            List<DFAEdgeModel> edgesWithCurrentStateAsSource = chromosome.Edges.Where(e => e.Source.ID == state.ID).ToList();
            foreach (string symbol in Alphabet)
            {
                if (edgesWithCurrentStateAsSource.Any(e => e.Input == symbol))
                    missingDeterministicEdgesForState -= 1;
            }
            
            /*
            foreach (string symbol in Alphabet)
            {
                if (chromosome.Edges.Any(e => e.Source.ID == state.ID && e.Input == symbol))
                    missingDeterministicEdgesForState -= 1;
            }
            */
            missingDeterministicEdgesCounter += missingDeterministicEdgesForState;
            missingDeterministicEdgesForState = Alphabet.Count;
        }
        
        return missingDeterministicEdgesCounter;
    }

}
