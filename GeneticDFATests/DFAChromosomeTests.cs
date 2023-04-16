using System;
using System.Collections.Generic;
using System.Linq;
using GeneticDFA;
using GeneticSharp;
using Xunit;

namespace GeneticDFATests;

public class DFAChromosomeTests 
{
    [Theory]
    [MemberData(nameof(SetsOfTestEdgesForFindAndAssignNonDeterministicEdgesTests))]
    public void FindAndAssignNonDeterministicEdgesCorrectBehavior(List<DFAEdge> edges, 
        List<int> expectedNonDeterministicEdgeIDs)
    {
        //Arrange
        List<DFAEdge> expected = edges.Where(e => expectedNonDeterministicEdgeIDs.Contains(e.ID)).ToList();
        DFAChromosome chromosome = new DFAChromosome(States, edges, State1);
        
        //Act
        chromosome.FindAndAssignNonDeterministicEdges();

        //Assert
        Assert.Equal(chromosome.NonDeterministicEdges, expected);
        
    }

    [Theory]
    [MemberData(nameof(SetsOfTestEdgesForPerfectReachabilityTests))]
    public void FixUnreachabilityDoesNothingWhenPerfectReachability(List<DFAEdge> edges)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'A', 'B', 'C'};
        DFAChromosome chromosome = new DFAChromosome(States, edges, State1);
        List<DFAEdge> expected = new List<DFAEdge>(edges);
        
        //Act
        chromosome.FixUnreachability(alphabet);
        
        //Assert
        Assert.Equal(expected, chromosome.Edges);
    }

    [Theory]
    [MemberData(nameof(SetsOfTestEdgesForExactlyOneEdgeAddedPerUnreachableStateTests))]
    public void FixUnreachabilityAddsExactlyOneEdgeToEachUnreachableStateWhenTheyHaveNoEdgesToEachOther(List<DFAEdge> edges, 
        List<DFAState> unreachableStates)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'A', 'B', 'C'};
        List<DFAChromosome> chromosomes = new List<DFAChromosome>();
        for (int i = 0; i < 100; i++)
        {
            chromosomes.Add(new DFAChromosome(States, new List<DFAEdge>(edges), State1));
        }
        
        //Act
        foreach (DFAChromosome chromosome in chromosomes)
        {
            chromosome.FixUnreachability(alphabet);
        }
        
        //Assert
        Assert.All(chromosomes, chromosome =>
        {
            List<DFAEdge> newEdges = chromosome.Edges.Where(e => !edges.Contains(e)).ToList();
            Assert.All(unreachableStates, s => Assert.Single(newEdges, e => e.Target == s));
        });
    }
    
    
    [Theory]
    [MemberData(nameof(SetsOfTestEdgesForAtMostOneEdgeAddedPerUnreachableStateTests))]
    public void FixUnreachabilityAddsAtMostOneEdgeToUnreachableStatesWhenTheyHaveEdgesToEachOther(List<DFAEdge> edges, 
        List<DFAState> unreachableStates)
    {
        //Arrange
        List<char> alphabet = new List<char>() {'A', 'B', 'C'};
        List<DFAChromosome> chromosomes = new List<DFAChromosome>();
        for (int i = 0; i < 100; i++)
        {
            chromosomes.Add(new DFAChromosome(States, new List<DFAEdge>(edges), State1));
        }
        
        //Act
        foreach (DFAChromosome chromosome in chromosomes)
        {
            chromosome.FixUnreachability(alphabet);
        }
        
        //Assert
        Assert.All(chromosomes, chromosome =>
        {
            List<DFAEdge> newEdges = chromosome.Edges.Where(e => !edges.Contains(e)).ToList();
            Assert.All(unreachableStates, s => Assert.True(newEdges.Count(e => e.Target == s) <= 1));
        });
    }

    [Theory]
    [MemberData(nameof(SetsOfTestEdgesForFixUnreachabilityCorrectBehaviorTests))]
    public void FixUnreachabilityCorrectBehaviorWithTestRandomizer(List<DFAEdge> edges, List<DFAEdge> expectedNewEdges)
    {
        //Arrange
        RandomizationProvider.Current = new TestRandomization();
        List<char> alphabet = new List<char>() {'A', 'B', 'C'};
        DFAChromosome chromosome = new DFAChromosome(States, new List<DFAEdge>(edges), State1);
        
        //Act
        chromosome.FixUnreachability(alphabet);
        RandomizationProvider.Current = new FastRandomRandomization();

        //Assert
        List<DFAEdge> newEdges = chromosome.Edges.Where(e => !edges.Contains(e)).ToList();
        Assert.All(newEdges, e => Assert.Contains(expectedNewEdges, e2 => e2.Source == e.Source && e2.Input == e.Input
            && e2.Target == e.Target));
    }
    
    
    private static readonly DFAState State1 = new DFAState(1, false);
    private static readonly DFAState State2 = new DFAState(2, false);
    private static readonly DFAState State3 = new DFAState(3, true);
    
    private static readonly List<DFAState> States = new List<DFAState>()
    {
        State1,
        State2,
        State3
    };

   
    
    
    public static IEnumerable<object[]> SetsOfTestEdgesForFindAndAssignNonDeterministicEdgesTests =>
        new List<object[]>()
        {
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State1, '0'),
                    new DFAEdge(2, State1, State2, '1'),
                    new DFAEdge(3, State2, State1, '0'),
                    new DFAEdge(4, State2, State3, '1'),
                    new DFAEdge(5, State3, State1, '1'),
                    new DFAEdge(6, State3, State1, '0'),
                },
                new List<int>()
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State1, '0'),
                },
                new List<int>()
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State1, '0'),
                    new DFAEdge(2, State1, State2, '0'),
                    new DFAEdge(3, State2, State1, '0'),
                    new DFAEdge(4, State2, State3, '1'),
                    new DFAEdge(5, State3, State1, '1'),
                    new DFAEdge(6, State3, State1, '0'),
                },
                new List<int>() {1,2}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State1, '0'),
                    new DFAEdge(2, State1, State2, '1'),
                    new DFAEdge(3, State2, State1, '0'),
                    new DFAEdge(4, State2, State3, '1'),
                    new DFAEdge(5, State3, State1, '1'),
                    new DFAEdge(6, State3, State1, '0'),
                    new DFAEdge(7, State2, State1, '1'),
                    new DFAEdge(8, State2, State2, '1'),
                },
                new List<int>() {4,7,8}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State1, '0'),
                    new DFAEdge(2, State1, State2, '0'),
                    new DFAEdge(3, State2, State1, '0'),
                    new DFAEdge(4, State2, State3, '1'),
                    new DFAEdge(5, State3, State1, '1'),
                    new DFAEdge(6, State3, State2, '1'),
                },
                new List<int>() {1,2,5,6}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State1, '0'),
                    new DFAEdge(2, State1, State2, '0'),
                },
                new List<int>() {1,2}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State1, '0'),
                    new DFAEdge(2, State1, State2, '1'),
                    new DFAEdge(3, State1, State3, '0'),
                },
                new List<int>() {1,3}
            },
        };

    public static IEnumerable<object[]> SetsOfTestEdgesForPerfectReachabilityTests =>
        new List<object[]>()
        {
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State2, 'A'),
                    new DFAEdge(2, State1, State3, 'A')
                }
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State2, 'A'),
                    new DFAEdge(2, State2, State3, 'B')
                }
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State2, 'A'),
                    new DFAEdge(2, State1, State3, 'A'),
                    new DFAEdge(3, State3, State1, 'C'),
                    new DFAEdge(4, State2, State2, 'B'),
                }
            },
        };

    public static IEnumerable<object[]> SetsOfTestEdgesForExactlyOneEdgeAddedPerUnreachableStateTests =>
        new List<object[]>()
        {
            new object[]
            {
                new List<DFAEdge>(),
                new List<DFAState>() {State2, State3}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State1, 'A')
                },
                new List<DFAState>() {State2, State3}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State2, 'A')
                },
                new List<DFAState>() {State3}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State2, 'A'),
                    new DFAEdge(2, State3, State1, 'C'),
                    new DFAEdge(3, State2, State1, 'B'),
                    new DFAEdge(4, State3, State3, 'C')
                },
                new List<DFAState>() {State3}
            },
        };

    public static IEnumerable<object[]> SetsOfTestEdgesForAtMostOneEdgeAddedPerUnreachableStateTests =>
        new List<object[]>()
        {
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State2, State3, 'B'),
                },
                new List<DFAState>() {State2, State3}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State2, State3, 'B'),
                    new DFAEdge(2, State3, State2, 'A'),
                },
                new List<DFAState>() {State2, State3}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State2, State3, 'B'),
                    new DFAEdge(2, State3, State1, 'C'),
                    new DFAEdge(3, State2, State1, 'B'),
                    new DFAEdge(4, State3, State3, 'C')
                },
                new List<DFAState>() {State2, State3}
            }
        };
    
    public static IEnumerable<object[]> SetsOfTestEdgesForFixUnreachabilityCorrectBehaviorTests =>
        new List<object[]>()
        {
            new object[]
            {
                new List<DFAEdge>(),
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State2, 'A'),
                    new DFAEdge(2, State1, State3, 'A')
                }
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State2, 'C'),
                },
                new List<DFAEdge>()
                {
                    new DFAEdge(2, State1, State3, 'A')
                }
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State1, State3, 'C'),
                    new DFAEdge(2, State1, State1, 'A'),
                },
                new List<DFAEdge>()
                {
                    new DFAEdge(3, State1, State2, 'A')
                }
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State2, State3, 'B'),
                },
                new List<DFAEdge>()
                {
                    new DFAEdge(2, State1, State2, 'A')
                }
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State3, State2, 'B'),
                },
                new List<DFAEdge>()
                {
                    new DFAEdge(2, State1, State2, 'A'),
                    new DFAEdge(3, State1, State3, 'A')
                }
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, State2, State3, 'B'),
                    new DFAEdge(2, State3, State3, 'C'),
                    new DFAEdge(3, State3, State1, 'A'),
                    new DFAEdge(4, State3, State2, 'A'),
                },
                new List<DFAEdge>()
                {
                    new DFAEdge(5, State1, State2, 'A')
                }
            },
        };
}
