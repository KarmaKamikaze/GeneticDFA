using GeneticSharp;

namespace GeneticDFA;

public class DFAFitness : IFitness
{
    public DFAFitness(List<TestTrace> traces, List<char> alphabet, double weightTruePositive, 
        double weightTrueNegative, double weightFalsePositive, double weightFalseNegative, 
        double weightNonDeterministicEdges, double weightMissingDeterministicEdges, double weightSize)
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

    private List<TestTrace> Traces { get; }
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
        
        foreach (TestTrace trace in Traces)
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
                    fitnessScore -= WeightFalsePositive;
                    break;
                default:
                    fitnessScore -= WeightFalseNegative;
                    break;
            }
        }
        
        //THIS CALL SHOULD BE MOVED TO THE END OF MUTATION AND CROSSOVERS!
        DFAChromosomeHelper.FindAndAssignNonDeterministicEdges(chromosome);

        return fitnessScore - WeightNonDeterministicEdges*chromosome.NonDeterministicEdges.Count - 
               WeightMissingDeterministicEdges*NumberOfMissingDeterministicEdges(chromosome) -
               WeightSize*chromosome.Size;
    }

    private Verdict CheckTrace(DFAChromosome chromosome, TestTrace testTrace)
    {
        DFAState startState = chromosome.StartState;
        bool isTraceAccepted = ExploreState(chromosome, startState, testTrace.Trace);

        switch (isTraceAccepted)
        {
            case true when testTrace.IsAccepting:
                return Verdict.TruePositive;
            case false when testTrace.IsAccepting:
                return Verdict.FalseNegative;
            case true when !testTrace.IsAccepting:
                return Verdict.FalsePositive;
            default:
                return Verdict.TrueNegative;
        }
    }

    private bool ExploreState(DFAChromosome chromosome, DFAState state, string traceString)
    {
        if (traceString == "")
        {
            return state.IsAccept;
        }

        //Assuming inputs are chars
        char nextInput = traceString[0];
        string remainingTrace = traceString[1..];
        foreach (DFAEdge edge in chromosome.Edges)
        {
            if (edge.Source.ID == state.ID && edge.Input == nextInput)
            {
                if (ExploreState(chromosome, edge.Target, remainingTrace))
                    return true;
            }    
        }

        return false;
    }
    
    private int NumberOfMissingDeterministicEdges(DFAChromosome chromosome)
    {
        int missingDeterministicEdgesCounter = 0;
        int missingDeterministicEdgesForState = Alphabet.Count;
        
        foreach (DFAState state in chromosome.States)
        {
            List<DFAEdge> edgesWithCurrentStateAsSource = chromosome.Edges.Where(e => e.Source.ID == state.ID).ToList();
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
