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

        return fitnessScore - chromosome.NonDeterministicEdges.Count - NumberOfMissingDeterministicEdges(chromosome) - chromosome.Size;
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

    private bool ExploreState(DFAChromosome chromosome, DFAStateModel state, string traceString)
    {
        return true;
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
            
            missingDeterministicEdgesCounter += missingDeterministicEdgesForState;
            missingDeterministicEdgesForState = Alphabet.Count;
        }
        
        return missingDeterministicEdgesCounter;
    }
}
