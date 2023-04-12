using System.Collections.Generic;
using System.Linq;
using GeneticDFA;
using Xunit;

namespace GeneticDFATests;

public class DFAChromosomeTests
{
    [Theory]
    [MemberData(nameof(SetsOfTestEdges))]
    public void FindAndAssignNonDeterministicEdgesCorrectBehavior(List<DFAEdgeModel> edges, 
        List<int> expectedNonDeterministicEdgeIDs)
    {
        //Arrange
        List<DFAEdgeModel> expected = edges.Where(e => expectedNonDeterministicEdgeIDs.Contains(e.ID)).ToList();
        DFAChromosome chromosome = new DFAChromosome(States, edges, States[0]);
        
        //Act
        chromosome.FindAndAssignNonDeterministicEdges();

        //Assert
        Assert.Equal(chromosome.NonDeterministicEdges, expected);
        
    }

    private static readonly List<DFAStateModel> States = new List<DFAStateModel>()
    {
        new DFAStateModel(1, false),
        new DFAStateModel(2, false),
        new DFAStateModel(3, true)
    };

    public static IEnumerable<object[]> SetsOfTestEdges =>
        new List<object[]>()
        {
            new object[]
            {
                new List<DFAEdgeModel>()
                {
                    new DFAEdgeModel(1, States[0], States[0], '0'),
                    new DFAEdgeModel(2, States[0], States[1], '1'),
                    new DFAEdgeModel(3, States[1], States[0], '0'),
                    new DFAEdgeModel(4, States[1], States[2], '1'),
                    new DFAEdgeModel(5, States[2], States[0], '1'),
                    new DFAEdgeModel(6, States[2], States[0], '0'),
                },
                new List<int>()
            },
            new object[]
            {
                new List<DFAEdgeModel>()
                {
                    new DFAEdgeModel(1, States[0], States[0], '0'),
                },
                new List<int>()
            },
            new object[]
            {
                new List<DFAEdgeModel>()
                {
                    new DFAEdgeModel(1, States[0], States[0], '0'),
                    new DFAEdgeModel(2, States[0], States[1], '0'),
                    new DFAEdgeModel(3, States[1], States[0], '0'),
                    new DFAEdgeModel(4, States[1], States[2], '1'),
                    new DFAEdgeModel(5, States[2], States[0], '1'),
                    new DFAEdgeModel(6, States[2], States[0], '0'),
                },
                new List<int>() {1,2}
            },
            new object[]
            {
                new List<DFAEdgeModel>()
                {
                    new DFAEdgeModel(1, States[0], States[0], '0'),
                    new DFAEdgeModel(2, States[0], States[1], '1'),
                    new DFAEdgeModel(3, States[1], States[0], '0'),
                    new DFAEdgeModel(4, States[1], States[2], '1'),
                    new DFAEdgeModel(5, States[2], States[0], '1'),
                    new DFAEdgeModel(6, States[2], States[0], '0'),
                    new DFAEdgeModel(7, States[1], States[0], '1'),
                    new DFAEdgeModel(8, States[1], States[1], '1'),
                },
                new List<int>() {4,7,8}
            },
            new object[]
            {
                new List<DFAEdgeModel>()
                {
                    new DFAEdgeModel(1, States[0], States[0], '0'),
                    new DFAEdgeModel(2, States[0], States[1], '0'),
                    new DFAEdgeModel(3, States[1], States[0], '0'),
                    new DFAEdgeModel(4, States[1], States[2], '1'),
                    new DFAEdgeModel(5, States[2], States[0], '1'),
                    new DFAEdgeModel(6, States[2], States[1], '1'),
                },
                new List<int>() {1,2,5,6}
            },
            new object[]
            {
                new List<DFAEdgeModel>()
                {
                    new DFAEdgeModel(1, States[0], States[0], '0'),
                    new DFAEdgeModel(2, States[0], States[1], '0'),
                },
                new List<int>() {1,2}
            },
            new object[]
            {
                new List<DFAEdgeModel>()
                {
                    new DFAEdgeModel(1, States[0], States[0], '0'),
                    new DFAEdgeModel(2, States[0], States[1], '1'),
                    new DFAEdgeModel(3, States[0], States[2], '0'),
                },
                new List<int>() {1,3}
            },
        };
}
