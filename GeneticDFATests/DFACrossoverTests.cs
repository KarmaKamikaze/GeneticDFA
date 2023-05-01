using System.Collections.Generic;
using System.Linq;
using GeneticDFA;
using GeneticSharp;
using Xunit;

namespace GeneticDFATests;

public class DFACrossoverTests
{
    [Theory]
    [MemberData(nameof(Alphabets))]
    public void CrossoverMaintainsNumberOfStates(List<char> alphabet)
    {
        //Arrange
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAPopulation population =
            new DFAPopulation(2, 2, new DFAChromosome(), alphabet, new PerformanceGenerationStrategy());
        population.CreateInitialGeneration();

        //Act
        List<DFAChromosome> parents = new List<DFAChromosome>()
        {
            (DFAChromosome) population.CurrentGeneration.Chromosomes[0],
            (DFAChromosome) population.CurrentGeneration.Chromosomes[1]
        };
        IList<IChromosome> children = crossover.Cross(new List<IChromosome>()
            { parents[0], parents[1] });
        DFAChromosome child1 = (DFAChromosome) children[0];
        DFAChromosome child2 = (DFAChromosome) children[1];

        //Assert
        Assert.Equal(parents[0].States.Count + parents[1].States.Count,
            child1.States.Count + child2.States.Count);
    }

    [Theory]
    [MemberData(nameof(Alphabets))]
    public void CrossoverMaintainsStateUniqueness(List<char> alphabet)
    {
        //Arrange
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAPopulation population =
            new DFAPopulation(100, 100, new DFAChromosome(), alphabet, new PerformanceGenerationStrategy());
        population.CreateInitialGeneration();

        //Act
        List<IChromosome> children = new List<IChromosome>();
        for (int i = 0; i < population.CurrentGeneration.Chromosomes.Count - 1; i += 2)
        {
            children.AddRange(crossover.Cross(new List<IChromosome>()
            {
                population.CurrentGeneration.Chromosomes[i],
                population.CurrentGeneration.Chromosomes[i + 1]
            }));
        }

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.All(chromosome.States, s => { Assert.Single(chromosome.States, s2 => s2.Id == s.Id); });
        });
    }

    [Theory]
    [MemberData(nameof(Alphabets))]
    public void CrossoverMaintainsEdgeIDUniqueness(List<char> alphabet)
    {
        //Arrange
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAPopulation population =
            new DFAPopulation(100, 100, new DFAChromosome(), alphabet, new PerformanceGenerationStrategy());
        population.CreateInitialGeneration();

        //Act
        List<IChromosome> children = new List<IChromosome>();
        for (int i = 0; i < population.CurrentGeneration.Chromosomes.Count - 1; i += 2)
        {
            children.AddRange(crossover.Cross(new List<IChromosome>()
            {
                population.CurrentGeneration.Chromosomes[i],
                population.CurrentGeneration.Chromosomes[i + 1]
            }));
        }

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.All(chromosome.Edges, e => { Assert.Single(chromosome.Edges, e2 => e2.Id == e.Id); });
        });
    }

    [Theory]
    [MemberData(nameof(Alphabets))]
    public void CrossoverMaintainsEdgePropertiesUniqueness(List<char> alphabet)
    {
        //Arrange
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAPopulation population =
            new DFAPopulation(100, 100, new DFAChromosome(), alphabet, new PerformanceGenerationStrategy());
        population.CreateInitialGeneration();

        //Act
        List<IChromosome> children = new List<IChromosome>();
        for (int i = 0; i < population.CurrentGeneration.Chromosomes.Count - 1; i += 2)
        {
            children.AddRange(crossover.Cross(new List<IChromosome>()
            {
                population.CurrentGeneration.Chromosomes[i],
                population.CurrentGeneration.Chromosomes[i + 1]
            }));
        }

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

    [Theory]
    [MemberData(nameof(Alphabets))]
    public void CrossoverDoesNotCauseEdgesToPointOutsideTheSetOfStates(List<char> alphabet)
    {
        //Arrange
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAPopulation population =
            new DFAPopulation(100, 100, new DFAChromosome(), alphabet, new PerformanceGenerationStrategy());
        population.CreateInitialGeneration();

        //Act
        List<IChromosome> children = new List<IChromosome>();
        for (int i = 0; i < population.CurrentGeneration.Chromosomes.Count - 1; i += 2)
        {
            children.AddRange(crossover.Cross(new List<IChromosome>()
            {
                population.CurrentGeneration.Chromosomes[i],
                population.CurrentGeneration.Chromosomes[i + 1]
            }));
        }

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.All(chromosome.Edges,
                e => { Assert.True(chromosome.States.Contains(e.Source) && chromosome.States.Contains(e.Target)); });
        });
    }

    [Theory]
    [MemberData(nameof(Alphabets))]
    public void CrossoverEnsuresPresenceOfAnAcceptState(List<char> alphabet)
    {
        //Arrange
        DFACrossover crossover = new DFACrossover(2, 2, 0, alphabet);
        DFAPopulation population =
            new DFAPopulation(100, 100, new DFAChromosome(), alphabet, new PerformanceGenerationStrategy());
        population.CreateInitialGeneration();

        //Act
        List<IChromosome> children = new List<IChromosome>();
        for (int i = 0; i < population.CurrentGeneration.Chromosomes.Count - 1; i += 2)
        {
            children.AddRange(crossover.Cross(new List<IChromosome>()
            {
                population.CurrentGeneration.Chromosomes[i],
                population.CurrentGeneration.Chromosomes[i + 1]
            }));
        }

        //Assert
        Assert.All(children, c =>
        {
            DFAChromosome chromosome = (DFAChromosome) c;
            Assert.Contains(chromosome.States, s => s.IsAccept);
        });
    }

    public static readonly IEnumerable<object[]> Alphabets = new List<object[]>()
    {
        new object[] { new List<char>() { '0' } },
        new object[] { new List<char>() { '0', '1' } },
        new object[] { new List<char>() { '0', '1', '2' } },
        new object[] { new List<char>() { '0', '1', '2', '3' } },
        new object[] { new List<char>() { '0', '1', '2', '3', '4' } },
        new object[] { new List<char>() { '0', '1', '2', '3', '4', '5' } },
    };
}
