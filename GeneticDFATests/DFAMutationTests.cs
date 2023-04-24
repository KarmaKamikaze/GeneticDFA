using System;
using System.Collections.Generic;
using System.Linq;
using GeneticDFA;
using Xunit;

namespace GeneticDFATests;

public class DFAMutationTests
{

    [Theory]
    [MemberData(nameof(RemoveEdgesTestData))]
    public void RemoveEdgeCorrectBehaviorWithEdges(DFAChromosome chromosome, int expected)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0,0,0,1,0,0,0,0,0,0);

        //Act
        mutation.Mutate(chromosome, 0);

        
        //Assert
        Assert.Equal(expected, chromosome.Edges.Count);
    }
    
    public static readonly IEnumerable<object[]> RemoveEdgesTestData = new List<object[]>()
    {
        new object[] { TestDFAs.SmallDFA.Clone(), 5 },
        new object[] { TestDFAs.NFA.Clone(), 7 }
    };
    /*
    [Fact]
    public void RemoveEdgeCorrectBehaviorNoEdges()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0,0,0,1,0,0,0,0,0,0);
        List<DFAState> states = new List<DFAState>() {new DFAState(1, true), new DFAState(2, false)};
        DFAChromosome chromosome = new DFAChromosome(states, new List<DFAEdge>(), states[0]);
        
        //Act
        Exception? exception = Record.Exception(() => mutation.Mutate(chromosome, 0));
        
        //Assert
        Assert.Null(exception);
    }
    */

    [Theory]
    [MemberData(nameof(AddAcceptStateTestData))]
    public void AddAcceptStateCorrectBehavior(DFAChromosome chromosome, int expected)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0,0,0,0,0,0,1,0,0,0);
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        Assert.Equal(expected, chromosome.States.Count(s => s.IsAccept));
    }
    
    public static readonly IEnumerable<object[]> AddAcceptStateTestData = new List<object[]>()
    {
        new object[] { TestDFAs.SmallDFA.Clone(), 2 },
        new object[] { TestDFAs.NFA.Clone(), 2 },
    };

    [Theory]
    [MemberData(nameof(RemoveAcceptStateCountTestData))]
    public void RemoveAcceptStateCorrectNumberOfAcceptState(DFAChromosome chromosome, int expected)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0,0,0,0,0,0,0,1,0,0);
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        Assert.Equal(expected, chromosome.States.Count(s => s.IsAccept));
        
    }
    
    private static List<DFAState> States = new List<DFAState>()
    {
        new DFAState(1, true),
        new DFAState(2, true),
        new DFAState(3, false)
    };
    
    public static readonly IEnumerable<object[]> RemoveAcceptStateCountTestData = new List<object[]>()
    {
        new object[] { TestDFAs.SmallDFA.Clone(), 1 },
        new object[] { TestDFAs.NFA.Clone(), 1 },
        new object[] { new DFAChromosome(States, new List<DFAEdge>(), States[0]), 1 },
    };
    
    [Theory]
    [MemberData(nameof(RemoveAcceptStateMoveAccStateTestData))]
    public void RemoveAcceptStateMovesAcceptState(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0,0,0,0,0,0,0,1,0,0);
        DFAState currentAcceptState = chromosome.States.First(s => s.IsAccept);
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        Assert.NotEqual(currentAcceptState, chromosome.States.First(s => s.IsAccept));
    }
    
    public static readonly IEnumerable<object[]> RemoveAcceptStateMoveAccStateTestData = new List<object[]>()
    {
        new object[] { TestDFAs.SmallDFA.Clone()},
        new object[] { TestDFAs.NFA.Clone() },
    };

    [Fact]
    public void AddStateAddsExactlyOneState()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        DFAChromosome chromosome = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        int expected = chromosome.States.Count + 1;
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        Assert.Equal(expected, chromosome.States.Count);
    }
    
    [Fact]
    public void AddStateAddsExactlyTwoEdges()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        DFAChromosome chromosome = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        int expected = chromosome.Edges.Count + 2;
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        Assert.Equal(expected, chromosome.Edges.Count);
    }
    
    [Fact]
    public void AddStateEnsuresReachability()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        DFAChromosome chromosome = (DFAChromosome) TestDFAs.SmallDFA.Clone();

        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        //Assert.True(expected, chromosome.Edges.Count);
    }
    
    
}
