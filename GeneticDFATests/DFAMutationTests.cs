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
    [MemberData(nameof(TestDFAsData))]
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
    
    public static IEnumerable<object[]> TestDFAsData => new List<object[]>()
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
    public void AddStateAddsNewEdgesAreRelatedToTheNewState()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        DFAChromosome chromosome = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        DFAState newState = chromosome.States[3];
        Assert.True(chromosome.Edges.Count(e => e.Source == newState || e.Target == newState) == 2);
    }
    
    [Theory]
    [MemberData(nameof(TestDFAsData))]
    public void AddStateOnlyAddsUniqueEdges(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        Assert.All(chromosome.Edges, e =>
        {
            Assert.Single(chromosome.Edges,
                e2 => e2.Source == e.Source && e2.Input == e.Input && e2.Target == e.Target);
        });
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
        DFAState newState = chromosome.States[3];
        List<DFAState> reachableStates = DFAChromosomeHelper.FindReachableStates(chromosome);
        Assert.Contains(chromosome.Edges, e => reachableStates.Contains(e.Source) && e.Target == newState);
    }

    [Theory]
    [MemberData(nameof(AddEdgeExactlyOneTestData))]
    public void AddEdgeAddsExactlyOneEdge(DFAChromosome chromosome, int expected)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        Assert.Equal(expected, chromosome.Edges.Count);
        
    }
    
    public static readonly IEnumerable<object[]> AddEdgeExactlyOneTestData = new List<object[]>()
    {
        new object[] { TestDFAs.SmallDFA.Clone(), 7},
        new object[] { TestDFAs.NFA.Clone(), 9 },
    };
    
    [Theory]
    [MemberData(nameof(TestDFAsData))]
    public void AddEdgeOnlyAddsUniqueEdges(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0);
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        Assert.All(chromosome.Edges, e =>
        {
            Assert.Single(chromosome.Edges,
                e2 => e2.Source == e.Source && e2.Input == e.Input && e2.Target == e.Target);
        });
    }

    [Theory]
    [MemberData(nameof(TestDFAsData))]
    public void ChangeSourceEnsuresUniqueness(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0);

        //Act
        mutation.Mutate(chromosome, 0);

        //Assert
        Assert.All(chromosome.Edges, e =>
        {
            Assert.Single(chromosome.Edges,
                e2 => e2.Source == e.Source && e2.Input == e.Input && e2.Target == e.Target);
        });
    }

    [Theory]
    [MemberData(nameof(TestDFAsData))]
    public void ChangeSourceModifiesExactlyOneEdge(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0);
        DFAChromosome preMutationChromosome = (DFAChromosome) chromosome.Clone();
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        List<DFAEdge> modifiedEdges = chromosome.Edges
            .Where(e => preMutationChromosome.Edges.Any(e2 => e2.Id == e.Id && e2.Source.Id != e.Source.Id)).ToList();
        Assert.Single(modifiedEdges);
    }
    
    [Theory]
    [MemberData(nameof(TestDFAsData))]
    public void ChangeTargetEnsuresUniqueness(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0);

        //Act
        mutation.Mutate(chromosome, 0);

        //Assert
        Assert.All(chromosome.Edges, e =>
        {
            Assert.Single(chromosome.Edges,
                e2 => e2.Source == e.Source && e2.Input == e.Input && e2.Target == e.Target);
        });
    }
    
    [Theory]
    [MemberData(nameof(TestDFAsData))]
    public void ChangeTargetModifiesExactlyOneEdge(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0);
        DFAChromosome preMutationChromosome = (DFAChromosome) chromosome.Clone();
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        List<DFAEdge> modifiedEdges = chromosome.Edges
            .Where(e => preMutationChromosome.Edges.Any(e2 => e2.Id == e.Id && e2.Target.Id != e.Target.Id)).ToList();
        Assert.Single(modifiedEdges);
    }
    
    [Theory]
    [MemberData(nameof(TestDFAsData))]
    public void ChangeInputEnsuresUniqueness(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        //Act
        mutation.Mutate(chromosome, 0);

        //Assert
        Assert.All(chromosome.Edges, e =>
        {
            Assert.Single(chromosome.Edges,
                e2 => e2.Source == e.Source && e2.Input == e.Input && e2.Target == e.Target);
        });
    }
    
    [Theory]
    [MemberData(nameof(TestDFAsData))]
    public void ChangeInputModifiesExactlyOneEdge(DFAChromosome chromosome)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'1', '0'};
        DFAMutation mutation = new DFAMutation(alphabet, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
        DFAChromosome preMutationChromosome = (DFAChromosome) chromosome.Clone();
        
        //Act
        mutation.Mutate(chromosome, 0);
        
        //Assert
        List<DFAEdge> modifiedEdges = chromosome.Edges
            .Where(e => preMutationChromosome.Edges.Any(e2 => e2.Id == e.Id && e2.Input != e.Input)).ToList();
        Assert.Single(modifiedEdges);
    }
    
}
