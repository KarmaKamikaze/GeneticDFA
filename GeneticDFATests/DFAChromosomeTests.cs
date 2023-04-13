using System.Collections.Generic;
using System.Linq;
using GeneticDFA;
using Xunit;

namespace GeneticDFATests;

public class DFAChromosomeTests
{
    [Theory]
    [MemberData(nameof(SetsOfTestEdges))]
    public void FindAndAssignNonDeterministicEdgesCorrectBehavior(List<DFAEdge> edges, 
        List<int> expectedNonDeterministicEdgeIDs)
    {
        //Arrange
        List<DFAEdge> expected = edges.Where(e => expectedNonDeterministicEdgeIDs.Contains(e.ID)).ToList();
        DFAChromosome chromosome = new DFAChromosome(States, edges, States[0]);
        
        //Act
        chromosome.FindAndAssignNonDeterministicEdges();

        //Assert
        Assert.Equal(chromosome.NonDeterministicEdges, expected);
        
    }

    private static readonly List<DFAState> States = new List<DFAState>()
    {
        new DFAState(1, false),
        new DFAState(2, false),
        new DFAState(3, true)
    };

    public static IEnumerable<object[]> SetsOfTestEdges =>
        new List<object[]>()
        {
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, States[0], States[0], '0'),
                    new DFAEdge(2, States[0], States[1], '1'),
                    new DFAEdge(3, States[1], States[0], '0'),
                    new DFAEdge(4, States[1], States[2], '1'),
                    new DFAEdge(5, States[2], States[0], '1'),
                    new DFAEdge(6, States[2], States[0], '0'),
                },
                new List<int>()
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, States[0], States[0], '0'),
                },
                new List<int>()
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, States[0], States[0], '0'),
                    new DFAEdge(2, States[0], States[1], '0'),
                    new DFAEdge(3, States[1], States[0], '0'),
                    new DFAEdge(4, States[1], States[2], '1'),
                    new DFAEdge(5, States[2], States[0], '1'),
                    new DFAEdge(6, States[2], States[0], '0'),
                },
                new List<int>() {1,2}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, States[0], States[0], '0'),
                    new DFAEdge(2, States[0], States[1], '1'),
                    new DFAEdge(3, States[1], States[0], '0'),
                    new DFAEdge(4, States[1], States[2], '1'),
                    new DFAEdge(5, States[2], States[0], '1'),
                    new DFAEdge(6, States[2], States[0], '0'),
                    new DFAEdge(7, States[1], States[0], '1'),
                    new DFAEdge(8, States[1], States[1], '1'),
                },
                new List<int>() {4,7,8}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, States[0], States[0], '0'),
                    new DFAEdge(2, States[0], States[1], '0'),
                    new DFAEdge(3, States[1], States[0], '0'),
                    new DFAEdge(4, States[1], States[2], '1'),
                    new DFAEdge(5, States[2], States[0], '1'),
                    new DFAEdge(6, States[2], States[1], '1'),
                },
                new List<int>() {1,2,5,6}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, States[0], States[0], '0'),
                    new DFAEdge(2, States[0], States[1], '0'),
                },
                new List<int>() {1,2}
            },
            new object[]
            {
                new List<DFAEdge>()
                {
                    new DFAEdge(1, States[0], States[0], '0'),
                    new DFAEdge(2, States[0], States[1], '1'),
                    new DFAEdge(3, States[0], States[2], '0'),
                },
                new List<int>() {1,3}
            },
        };
}
