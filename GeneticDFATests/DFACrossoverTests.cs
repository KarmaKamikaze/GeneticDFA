using System.Collections.Generic;
using System.Linq;
using GeneticDFA;
using GeneticSharp;
using Xunit;

namespace GeneticDFATests;

public class DFACrossoverTests
{
    [Fact]
    public void CrossoverMaintainsNumberOfStates()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'0', '1'};
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAChromosome chromosome1 = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        DFAChromosome chromosome2 = (DFAChromosome) TestDFAs.NFA.Clone();

        //Act
        IList<IChromosome> children = crossover.Cross(new List<IChromosome>() {chromosome1, chromosome2});
        DFAChromosome child1 = (DFAChromosome) children[0];
        DFAChromosome child2 = (DFAChromosome) children[1];

        //Assert
        Assert.Equal(chromosome1.States.Count + chromosome2.States.Count,
            child1.States.Count + child2.States.Count);
    }

    [Fact]
    public void CrossoverMaintainsAtLeastNumberOfEdges()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'0', '1'};
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAChromosome chromosome1 = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        DFAChromosome chromosome2 = (DFAChromosome) TestDFAs.NFA.Clone();

        //Act
        IList<IChromosome> children = crossover.Cross(new List<IChromosome>() {chromosome1, chromosome2});
        DFAChromosome child1 = (DFAChromosome) children[0];
        DFAChromosome child2 = (DFAChromosome) children[1];

        //Assert
        Assert.True(chromosome1.Edges.Count + chromosome2.Edges.Count <=
                    child1.Edges.Count + child2.Edges.Count);
    }

    [Fact]
    public void CrossoverMaintainsStateUniqueness()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'0', '1'};
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAChromosome chromosome1 = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        DFAChromosome chromosome2 = (DFAChromosome) TestDFAs.NFA.Clone();

        //Act
        IList<IChromosome> children = crossover.Cross(new List<IChromosome>() {chromosome1, chromosome2});

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.All(chromosome.States, s => { Assert.Single(chromosome.States, s2 => s2.Id == s.Id); });
        });
    }

    [Fact]
    public void CrossoverMaintainsEdgeIDUniqueness()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'0', '1'};
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAChromosome chromosome1 = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        DFAChromosome chromosome2 = (DFAChromosome) TestDFAs.NFA.Clone();

        //Act
        IList<IChromosome> children = crossover.Cross(new List<IChromosome>() {chromosome1, chromosome2});

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.All(chromosome.Edges, e => { Assert.Single(chromosome.Edges, e2 => e2.Id == e.Id); });
        });
    }

    [Fact]
    public void CrossoverMaintainsEdgePropertiesUniqueness()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'0', '1'};
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAChromosome chromosome1 = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        DFAChromosome chromosome2 = (DFAChromosome) TestDFAs.NFA.Clone();

        //Act
        IList<IChromosome> children = crossover.Cross(new List<IChromosome>() {chromosome1, chromosome2});

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.All(chromosome.Edges,
                e =>
                {
                    Assert.Single(chromosome.Edges,
                        e2 => e2.Source == e.Source && e2.Input == e.Input && e2.Target == e.Target);
                });
        });
    }

    [Fact]
    public void CrossoverDoesNotCauseEdgesToPointOutsideTheSetOfStates()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'0', '1'};
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAChromosome chromosome1 = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        DFAChromosome chromosome2 = (DFAChromosome) TestDFAs.NFA.Clone();

        //Act
        IList<IChromosome> children = crossover.Cross(new List<IChromosome>() {chromosome1, chromosome2});

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.All(chromosome.Edges,
                e => { Assert.True(chromosome.States.Contains(e.Source) && chromosome.States.Contains(e.Target)); });
        });
    }

    [Fact]
    public void CrossoverEnsuresPresenceOfAnAcceptState()
    {
        //Arrange
        List<char> alphabet = new List<char>() {'0', '1'};
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAChromosome chromosome1 = (DFAChromosome) TestDFAs.SmallDFA.Clone();
        DFAChromosome chromosome2 = (DFAChromosome) TestDFAs.NFA.Clone();

        //Act
        IList<IChromosome> children = crossover.Cross(new List<IChromosome>() {chromosome1, chromosome2});

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.Contains(chromosome.States, s => s.IsAccept);
        });
    }
}
