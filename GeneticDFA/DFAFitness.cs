using GeneticSharp;

namespace GeneticDFA;

public class DFAFitness : IFitness
{
    public DFAFitness(List<TraceModel> traces, List<char> alphabet, double weightTruePositive, double weightTrueNegative, double weightFalsePositive, double weightFalseNegative, double weightNonDeterministicEdges, double weightMissingDeterministicEdges, double weightSize)
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

    private List<TraceModel> Traces { get; }
    private List<char> Alphabet { get; }
    private double WeightTruePositive { get; }
    private double WeightTrueNegative { get; }
    private double WeightFalsePositive { get; }
    private double WeightFalseNegative { get; }
    private double WeightNonDeterministicEdges { get; }
    private double WeightMissingDeterministicEdges { get; }
    private double WeightSize { get; }
    
    private enum Verdict
    {
        TruePositive,
        TrueNegative,
        FalsePositive,
        FalseNegative
    }

    public double Evaluate(IChromosome paramChromosome)
    {
        DFAChromosome chromosome = (DFAChromosome) paramChromosome;
        double fitnessScore = 0;
        
        foreach (TraceModel trace in Traces)
        {
            Verdict verdict = CheckTrace(chromosome, trace);
            switch (verdict)
            {
                case Verdict.TruePositive:
                    fitnessScore += WeightTruePositive;
                    break;
                case Verdict.TrueNegative:
                    fitnessScore += WeightTrueNegative;
                    break;
                case Verdict.FalsePositive:
                    fitnessScore += WeightFalsePositive;
                    break;
                default:
                    fitnessScore += WeightFalseNegative;
                    break;
            }
        }
        
        //THIS CALL SHOULD BE MOVED TO THE END OF MUTATION AND CROSSOVERS!
        chromosome.FindAndAssignNonDeterministicEdges();

        return fitnessScore - WeightNonDeterministicEdges*chromosome.NonDeterministicEdges.Count - 
               WeightMissingDeterministicEdges*NumberOfMissingDeterministicEdges(chromosome) -
               WeightSize*chromosome.Size;
    }

    private Verdict CheckTrace(DFAChromosome chromosome, TraceModel trace)
    {
        DFAStateModel startState = chromosome.StartState;
        bool isTraceAccepted = ExploreState(chromosome, startState, trace.Trace);

        switch (isTraceAccepted)
        {
            case true when trace.IsAccepting:
                return Verdict.TruePositive;
            case false when trace.IsAccepting:
                return Verdict.FalseNegative;
            case true when !trace.IsAccepting:
                return Verdict.FalsePositive;
            default:
                return Verdict.TrueNegative;
        }
    }

    //Can be moved to chromosome class
    private bool ExploreState(DFAChromosome chromosome, DFAStateModel state, string traceString)
    {
        if (traceString == "")
        {
            return state.IsAccept;
        }

        char nextInput = traceString[0];
        string remainingTrace = traceString[1..];
        foreach (DFAEdgeModel edge in chromosome.Edges)
        {
            if (edge.Source.ID == state.ID && edge.Input == nextInput)
            {
                if (ExploreState(chromosome, edge.Target, remainingTrace))
                    return true;
            }    
        }

        return false;
    }
    
    
    //Can be moved to chromosome class, but it requires passing of the alphabet.
    private int NumberOfMissingDeterministicEdges(DFAChromosome chromosome)
    {
        int missingDeterministicEdgesCounter = 0;
        int missingDeterministicEdgesForState = Alphabet.Count;
        
        foreach (DFAStateModel state in chromosome.States)
        {
            List<DFAEdgeModel> edgesWithCurrentStateAsSource = chromosome.Edges.Where(e => e.Source.ID == state.ID).ToList();
            foreach (char symbol in Alphabet)
            {
                if (edgesWithCurrentStateAsSource.Any(e => e.Input == symbol))
                    missingDeterministicEdgesForState -= 1;
            }
            
            missingDeterministicEdgesCounter += missingDeterministicEdgesForState;
            missingDeterministicEdgesForState = Alphabet.Count;
        }
        
        return missingDeterministicEdgesCounter;
    }
}
