using GeneticSharp;

namespace GeneticDFA;

public class DFAFitness : IFitness
{
    public DFAFitness(List<TestTrace> traces, List<char> alphabet, double rewardTruePositive,
        double rewardTrueNegative, double penaltyFalsePositive, double penaltyFalseNegative,
        double weightNonDeterministicEdges, double weightUnreachableStates, double weightSize)
    {
        Traces = traces;
        Alphabet = alphabet;
        RewardTruePositive = rewardTruePositive;
        RewardTrueNegative = rewardTrueNegative;
        PenaltyFalsePositive = penaltyFalsePositive;
        PenaltyFalseNegative = penaltyFalseNegative;
        WeightNonDeterministicEdges = weightNonDeterministicEdges;
        WeightUnreachableStates = weightUnreachableStates;
        WeightSize = weightSize;
    }

    private List<TestTrace> Traces { get; }
    private List<char> Alphabet { get; }
    private double RewardTruePositive { get; }
    private double RewardTrueNegative { get; }
    private double PenaltyFalsePositive { get; }
    private double PenaltyFalseNegative { get; }
    private double WeightNonDeterministicEdges { get; }
    private double WeightUnreachableStates { get; }
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
                    fitnessScore += RewardTruePositive;
                    break;
                case Verdict.TrueNegative:
                    fitnessScore += RewardTrueNegative;
                    break;
                case Verdict.FalsePositive:
                    fitnessScore -= PenaltyFalsePositive;
                    break;
                default:
                    fitnessScore -= PenaltyFalseNegative;
                    break;
            }
        }

        return fitnessScore - WeightNonDeterministicEdges * chromosome.NonDeterministicEdges.Count -
               WeightUnreachableStates * (chromosome.States.Count - chromosome.ReachableStates.Count) -
               WeightSize * chromosome.Size;
    }

    private Verdict CheckTrace(DFAChromosome chromosome, TestTrace testTrace)
    {
        DFAState startState = chromosome.StartState!;
        bool isTraceAccepted = ExploreState(chromosome, startState, testTrace.Trace, 0);

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

    private bool ExploreState(DFAChromosome chromosome, DFAState state, string traceString, int traceStringIndex)
    {
        if (traceStringIndex == traceString.Length)
        {
            return state.IsAccept;
        }

        // Assuming inputs are chars
        char nextInput = traceString[traceStringIndex];
        traceStringIndex++;
        foreach (DFAEdge edge in chromosome.Edges)
        {
            if (edge.Source.Id == state.Id && edge.Input == nextInput)
            {
                if (ExploreState(chromosome, edge.Target, traceString, traceStringIndex))
                    return true;
            }
        }

        return false;
    }

    // Not in use
    private int NumberOfMissingDeterministicEdges(DFAChromosome chromosome)
    {
        int missingDeterministicEdgesCounter = 0;
        int missingDeterministicEdgesForState = Alphabet.Count;

        foreach (DFAState state in chromosome.States)
        {
            List<DFAEdge> edgesWithCurrentStateAsSource = chromosome.Edges.Where(e => e.Source.Id == state.Id).ToList();
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
