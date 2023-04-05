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

        return FindNonDeterministicEdges(chromosome).Count;
    }

    //Can be moved to the Chromosome class.
    private int NumberOfNonDeterministicEdges(DFAChromosome chromosome)
    {
        List<DFAEdgeModel> edges = chromosome.Edges;
        //var query = edges.GroupBy(x => new {x.Source.ID, x.Input}).Where(y => y.Count()>1);

        //return query.Sum(element => element.Count());
        
        edges.Sort(delegate(DFAEdgeModel edge1, DFAEdgeModel edge2)
        {
            int areSourcesEqual = edge1.Source.ID.CompareTo(edge2.Source.ID);
            return areSourcesEqual == 0 ? string.Compare(edge1.Input, edge2.Input, StringComparison.Ordinal) : areSourcesEqual;
        });

        int nonDeterministicEdgesCounter = 0;
        List<string> nonDeterministicEdgesFound = new List<string>();

        for (int i = 0; i < edges.Count-1; i++)
        {
            if (edges[i].Source.ID == edges[i + 1].Source.ID && edges[i].Input == edges[i + 1].Input)
            {
                nonDeterministicEdgesCounter++;
                if(!nonDeterministicEdgesFound.Contains(edges[i].Source.ID + edges[i].Input))
                    nonDeterministicEdgesFound.Add(edges[i].Source.ID + edges[i].Input);
            }
        }

        nonDeterministicEdgesCounter += nonDeterministicEdgesFound.Count;
        
        return nonDeterministicEdgesCounter;
    }
    
    
    //Can be moved to the Chromosome class.
    private List<DFAEdgeModel> FindNonDeterministicEdges(DFAChromosome chromosome)
    {
        List<DFAEdgeModel> edges = chromosome.Edges;

        edges.Sort(delegate(DFAEdgeModel edge1, DFAEdgeModel edge2)
        {
            int areSourcesEqual = edge1.Source.ID.CompareTo(edge2.Source.ID);
            return areSourcesEqual == 0 ? string.Compare(edge1.Input, edge2.Input, StringComparison.Ordinal) : areSourcesEqual;
        });

        List<DFAEdgeModel> nonDetEdges = new List<DFAEdgeModel>();

        
        if(edges[0].Source.ID == edges[1].Source.ID && edges[0].Input == edges[1].Input)
            nonDetEdges.Add(edges[0]);
        
        for (int i = 1; i < edges.Count-1; i++)
        {
            if ((edges[i].Source.ID == edges[i + 1].Source.ID && edges[i].Input == edges[i + 1].Input) ||
                (edges[i].Source.ID == edges[i - 1].Source.ID && edges[i].Input == edges[i - 1].Input))
            {
                nonDetEdges.Add(edges[i]);
            }
        }

        if(edges[^1].Source.ID == edges[^2].Source.ID && edges[^1].Input == edges[^2].Input)
            nonDetEdges.Add(edges[^1]);
        
        /*
        for (int i = 0; i < edges.Count-1; i++)
        {
            if (edges[i].Source.ID == edges[i + 1].Source.ID && edges[i].Input == edges[i + 1].Input)
            {
                if (!nonDetEdges.Contains(edges[i]))
                    nonDetEdges.Add(edges[i]);
                nonDetEdges.Add(edges[i+1]);
            }
        }
        */
        return nonDetEdges;
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
