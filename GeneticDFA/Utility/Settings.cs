﻿namespace GeneticDFA.Utility;

public class Settings
{
    public int MinPopulation { get; set; }
    public int MaxPopulation { get; set; }
    public int ConvergenceGenerationNumber { get; set; }
    public int MaximumGenerationNumber { get; set; }
    public int EliteSelectionScalingFactor { get; set; }
    public int NumberOfFittestIndividualsAcrossAllGenerations => Convert.ToInt32(0.05 * MinPopulation);
    public int RewardTruePositive { get; set; }
    public int RewardTrueNegative { get; set; }
    public double PenaltyFalsePositive { get; set; }
    public double PenaltyFalseNegative { get; set; }
    public double WeightNonDeterministicEdges { get; set; }
    public double WeightUnreachableStates { get; set; }
    public double WeightSize { get; set; }
    public double CrossoverProbability => 1 - MutationProbability;
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
        MinPopulation = 3500;
        MaxPopulation = 3500;
        ConvergenceGenerationNumber = 100;
        MaximumGenerationNumber = 400;
        EliteSelectionScalingFactor = 2;
        RewardTruePositive = 10;
        RewardTrueNegative = 10;
        PenaltyFalsePositive = 10;
        PenaltyFalseNegative = 10;
        WeightNonDeterministicEdges = 2;
        WeightUnreachableStates = 5;
        WeightSize = 5;
        MutationProbability = 0.5;
        NonDeterministicBehaviorProbability = 0.5;
        ChangeTargetProbability = 0.11;
        ChangeSourceProbability = 0.11;
        ChangeInputProbability = 0.11;
        RemoveEdgeProbability = 0.11;
        AddEdgeProbability = 0.12;
        AddStateProbability = 0.11;
        AddAcceptStateProbability = 0.11;
        RemoveAcceptStateProbability = 0.11;
        MergeStatesProbability = 0.11;
    }
}