using GeneticSharp;

namespace GeneticDFA;

public class DFAFitness : IFitness
{
    public DFAFitness(List<TraceModel> traces, List<string> alphabet, double wTp, double wTn, double wFp, double wFn, double wNonDetEdges, double wMissingDetEdges, double wSize)
    {
        Traces = traces;
        Alphabet = alphabet;
        wTP = wTp;
        wTN = wTn;
        wFP = wFp;
        wFN = wFn;
        this.wNonDetEdges = wNonDetEdges;
        this.wMissingDetEdges = wMissingDetEdges;
        this.wSize = wSize;
    }

    public DFAFitness()
    {
        
    }

    private List<TraceModel> Traces { get; set; }
    private List<string> Alphabet { get; set; }
    private double wTP { get; set; }
    private double wTN { get; set; }
    private double wFP { get; set; }
    private double wFN { get; set; }
    private double wNonDetEdges { get; set; }
    private double wMissingDetEdges { get; set; }
    private double wSize { get; set; }
    
    private enum Verdicts
    {
        TP,
        TN,
        FP,
        FN
    }
    
    
    public double Evaluate(IChromosome chromosome)
    {
        DFAChromosome chrom = (DFAChromosome) chromosome;
        
        
        return - NonDetEdges(chrom) - Size(chrom);
    }

    private double NonDetEdges(DFAChromosome chromosome)
    {
        List<DFAEdgeModel> edges = chromosome.Edges;
        
        edges.Sort(delegate(DFAEdgeModel edge1, DFAEdgeModel edge2)
        {
            int sourceEqual = edge1.Source.ID.CompareTo(edge2.Source.ID);
            return sourceEqual == 0 ? edge1.Input.CompareTo(edge2.Input) : sourceEqual;
        });

        int prevEdgeSource = edges[0].Source.ID;
        string prevEdgeInput = edges[0].Input;
        int nonDetEdgesCounter = 0;

        for (int i = 1; i < edges.Count; i++)
        {
            if (edges[i].Source.ID == prevEdgeSource && edges[i].Input == prevEdgeInput)
                nonDetEdgesCounter++;
            else
            {
                prevEdgeSource = edges[i].Source.ID;
                prevEdgeInput = edges[i].Input;
            }
        }

        return nonDetEdgesCounter;
    }


    private double MissingDetEdges(DFAChromosome chromosome)
    {
        int missingDetEdgesCounter = 0;
        int missingDetEdgesForState = Alphabet.Count;
        
        foreach (DFAStateModel state in chromosome.States)
        {
            foreach (string symbol in Alphabet)
            {
                if (chromosome.Edges.Any(e => e.Source.ID == state.ID && e.Input == symbol))
                    missingDetEdgesForState -= 1;
            }

            missingDetEdgesCounter += missingDetEdgesForState;
            missingDetEdgesForState = Alphabet.Count;
        }
        
        
        return missingDetEdgesCounter;
    }
    
    
    
    private double Size(DFAChromosome chromosome)
    {
        return chromosome.States.Count + chromosome.Edges.Count;
    }
    
    
    
}
