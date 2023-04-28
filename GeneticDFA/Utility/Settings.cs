namespace GeneticDFA.Utility;

public class Settings
{
    public int MinPopulation { get; set; }
    public int MaxPopulation { get; set; }
    public int ConvergenceGenerationNumber { get; set; }
    public int MaximumGenerationNumber { get; set; }
    public int EliteSelectionScalingFactor { get; set; }
    public int FitnessLowerBound { get; set; }
    public int NumberOfFittestIndividualsAcrossAllGenerations { get; set; }
    public int WeightTruePositive { get; set; }
    public int WeightTrueNegative { get; set; }
    public double WeightFalsePositive { get; set; }
    public double WeightFalseNegative { get; set; }
    public double WeightNonDeterministicEdges { get; set; }
    public double WeightMissingDeterministicEdges { get; set; }
    public double WeightSize { get; set; }
    public double CrossoverProbability { get; set; }
    public double MutationProbability { get; set; }
    public double NonDeterministicBehaviorProbability { get; set; }
    public double ChangeTargetProbability { get; set; }
    public double ChangeSourceProbability { get; set; }
    public double ChangeInputProbability { get; set; }
    public double RemoveEdgeProbability { get; set; }
    public double AddEdgeProbability { get; set; }
    public double AddStateProbability { get; set; }
    public double AddAcceptStateProbability { get; set; }
    public double RemoveAcceptStateProbability { get; set; }
    public double MergeStatesProbability { get; set; }

    public Settings()
    {
        // Default values
        MinPopulation = 10000;
        MaxPopulation = 10000;
        ConvergenceGenerationNumber = 30;
        MaximumGenerationNumber = 100;
        EliteSelectionScalingFactor = 2;
        FitnessLowerBound = 500;
        NumberOfFittestIndividualsAcrossAllGenerations = 500;
        WeightTruePositive = 10;
        WeightTrueNegative = 10;
        WeightFalsePositive = 10;
        WeightFalseNegative = 10;
        WeightNonDeterministicEdges = 0.5;
        WeightMissingDeterministicEdges = 0.5;
        WeightSize = 0.5;
        CrossoverProbability = 0.5;
        MutationProbability = 0.5;
        NonDeterministicBehaviorProbability = 0.5;
        ChangeTargetProbability = 0.1;
        ChangeSourceProbability = 0.1;
        ChangeInputProbability = 0.1;
        RemoveEdgeProbability = 0.1;
        AddEdgeProbability = 0.2;
        AddStateProbability = 0.1;
        AddAcceptStateProbability = 0.1;
        RemoveAcceptStateProbability = 0.1;
        MergeStatesProbability = 0.1;
    }
}
