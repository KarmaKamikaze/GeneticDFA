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
        
        return - NumberOfNonDeterministicEdges(chromosome) - NumberOfMissingDeterministicEdges(chromosome) 
                                                           - chromosome.Size;
    }

    //Can be moved to the Chromosome class.
    private int NumberOfNonDeterministicEdges(DFAChromosome chromosome)
    {
        List<DFAEdgeModel> edges = chromosome.Edges;
        
        edges.Sort(delegate(DFAEdgeModel edge1, DFAEdgeModel edge2)
        {
            int areSourcesEqual = edge1.Source.ID.CompareTo(edge2.Source.ID);
            return areSourcesEqual == 0 ? string.Compare(edge1.Input, edge2.Input, StringComparison.Ordinal) : areSourcesEqual;
        });

        int previousEdgeSource = edges[0].Source.ID;
        string previousEdgeInput = edges[0].Input;
        int nonDeterministicEdgesCounter = 0;

        for (int i = 1; i < edges.Count; i++)
        {
            if (edges[i].Source.ID == previousEdgeSource && edges[i].Input == previousEdgeInput)
                nonDeterministicEdgesCounter++;
            else
            {
                previousEdgeSource = edges[i].Source.ID;
                previousEdgeInput = edges[i].Input;
            }
        }

        return nonDeterministicEdgesCounter;
    }
    
    //Can be moved to chromosome class, but it requires passing of the alphabet.
    private int NumberOfMissingDeterministicEdges(DFAChromosome chromosome)
    {
        int missingDeterministicEdgesCounter = 0;
        int missingDeterministicEdgesForState = Alphabet.Count;
        
        foreach (DFAStateModel state in chromosome.States)
        {
            foreach (string symbol in Alphabet)
            {
                if (chromosome.Edges.Any(e => e.Source.ID == state.ID && e.Input == symbol))
                    missingDeterministicEdgesForState -= 1;
            }

            missingDeterministicEdgesCounter += missingDeterministicEdgesForState;
            missingDeterministicEdgesForState = Alphabet.Count;
        }
        
        return missingDeterministicEdgesCounter;
    }

}
