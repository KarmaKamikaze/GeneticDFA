using System.Collections.Generic;
using System.Linq;
using GeneticDFA;
using Xunit;

namespace GeneticDFATests;

public class DFAFitnessTests
{
    [Theory]
    [MemberData(nameof(SizeTestData))]
    public void FitnessScoreIsCorrectWhenOnlySizeWeighs(DFAChromosome chromosome, double expected)
    {
        //Arrange
        DFAFitness fitness = new DFAFitness(new List<TraceModel>(), new List<char>(), 0, 0, 0, 0, 0, 0, 1);

        //Act 
        double actual = fitness.Evaluate(chromosome);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(MissingDeterministicEdgesTestData))]
    public void FitnessScoreIsCorrectWhenOnlyNumberOfMissingDeterministicEdgesWeighs(DFAChromosome chromosome, List<char> alphabet, double expected)
    {
        //Arrange
        DFAFitness fitness = new DFAFitness(new List<TraceModel>(), alphabet, 0, 0, 0, 0, 0, 1, 0);

        //Act 
        double actual = fitness.Evaluate(chromosome);

        //Assert
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [MemberData(nameof(NonDeterministicEdgesTestData))]
    public void FitnessScoreIsCorrectWhenOnlyNumberOfNonDeterministicEdgesWeighs(DFAChromosome chromosome, double expected)
    {
        //Arrange
        DFAFitness fitness = new DFAFitness(new List<TraceModel>(), new List<char>(), 0, 0, 0, 0, 1, 0, 0);
        chromosome.FindAndAssignNonDeterministicEdges();
        
        //Act 
        double actual = fitness.Evaluate(chromosome);

        //Assert
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [MemberData(nameof(VerdictTestData))]
    public void FitnessScoreIsCorrectWhenOnlyOneVerdictCategoryWeighs(DFAChromosome chromosome, int[] verdictWeights, 
        double expected)
    {
        //Arrange
        DFAFitness fitness = new DFAFitness(SampleTraces, new List<char>(), verdictWeights[0], 
            verdictWeights[1], verdictWeights[2], verdictWeights[3], 0, 0, 0);

        //Act 
        double actual = fitness.Evaluate(chromosome);

        //Assert
        Assert.Equal(expected, actual);
    }   
    
    [Theory]
    [MemberData(nameof(OverallFitnessTestData))]
    public void FitnessScoreIsCorrect(DFAChromosome chromosome, List<char> alphabet, double expected)
    {
        //Arrange
        DFAFitness fitness = new DFAFitness(SampleTraces, alphabet, 1, 1, 1, 1, 1, 1, 1);
        chromosome.FindAndAssignNonDeterministicEdges();
        
        //Act 
        double actual = fitness.Evaluate(chromosome);

        //Assert
        Assert.Equal(expected, actual);
    }   
    
    
    public static readonly IEnumerable<object[]> SizeTestData = new List<object[]>()
    {
        new object[]{ TestDFAs.SmallDFA, -9},
        new object[]{ TestDFAs.NFA, -12 }
    };
    
    public static readonly IEnumerable<object[]> MissingDeterministicEdgesTestData = new List<object[]>()
    {
        new object[]{ TestDFAs.SmallDFA, new List<char>() {'0', '1'} , 0},
        new object[]{ TestDFAs.NFA, new List<char>() {'0', '1'}, -2 }
    };
    
    public static readonly IEnumerable<object[]> NonDeterministicEdgesTestData = new List<object[]>()
    {
        new object[]{ TestDFAs.SmallDFA, 0},
        new object[]{ TestDFAs.NFA, -4 }
    };
    
    public static readonly IEnumerable<object[]> VerdictTestData = new List<object[]>()
    {
        new object[]{ TestDFAs.SmallDFA, new[] {1,0,0,0}, 5},
        new object[]{ TestDFAs.NFA, new[] {1,0,0,0}, 2 },
        new object[]{ TestDFAs.SmallDFA, new[] {0,1,0,0}, 5},
        new object[]{ TestDFAs.NFA, new[] {0,1,0,0}, 4 },
        new object[]{ TestDFAs.SmallDFA, new[] {0,0,1,0}, 0},
        new object[]{ TestDFAs.NFA, new[] {0,0,1,0}, -1 },
        new object[]{ TestDFAs.SmallDFA, new[] {0,0,0,1}, 0},
        new object[]{ TestDFAs.NFA, new[] {0,0,0,1}, -3 },
    };
    
    public static readonly IEnumerable<object[]> OverallFitnessTestData = new List<object[]>()
    {
        new object[]{ TestDFAs.SmallDFA, new List<char>() {'0', '1'}, 1},
        new object[]{ TestDFAs.NFA, new List<char>() {'0', '1'}, -16 }
    };
    
    private static readonly List<TraceModel> SampleTraces = new List<TraceModel>()
    {
        new TraceModel("11", 2, true),
        new TraceModel("00011", 5, true),
        new TraceModel("110011", 6, true),
        new TraceModel("1011", 4, true),
        new TraceModel("0101101011", 10, true),
        new TraceModel("110", 3, false),
        new TraceModel("01", 1, false),
        new TraceModel("00111", 5, false),
        new TraceModel("1010", 4, false),
        new TraceModel("00000000000111", 14, false),
    };
}
