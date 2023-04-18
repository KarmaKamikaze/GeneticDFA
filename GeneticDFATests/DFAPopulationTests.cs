using System.Collections.Generic;
using System.Linq;
using GeneticDFA;
using Xunit;

namespace GeneticDFATests;

public class DFAPopulationTests
{
    [Theory]
    [InlineData(10, 10, 10)]
    [InlineData(100, 100, 100)]
    [InlineData(10, 100, 10)]
    [InlineData(1000, 1000, 1000)]
    public void InitialGenerationHasCorrectAmountOfIndividuals(int minSize, int maxSize, int expected)
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() { '0', '1' };
        DFAPopulation population = new DFAPopulation(minSize, maxSize, chromosome, alphabet);

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.Equal(expected, population.Generations[0].Chromosomes.Count);
    }

    [Theory]
    [InlineData(new[] { '0' }, 1)]
    [InlineData(new[] { '0', '1' }, 2)]
    [InlineData(new[] { '0', '1', '2' }, 3)]
    [InlineData(new[] { '0', '1', '2', '3' }, 4)]
    [InlineData(new[] { '0', '1', '2', '3', '4' }, 5)]
    public void InitialIndividualsHasNumberOfStatesEqualToAlphabetSize(char[] alphabet, int expected)
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet.ToList());

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.Equal(expected, dfaChromosome.States.Count);
        });
    }

    [Fact]
    public void InitialIndividualsStatesHasUniqueId()
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() { '0', '1', '2', '3' };
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet);

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.All(dfaChromosome.States, s => Assert.Single(dfaChromosome.States, s2 => s2.Id == s.Id));
        });
    }

    [Theory]
    [InlineData(new[] { '0' }, 1)]
    [InlineData(new[] { '0', '1' }, 2)]
    [InlineData(new[] { '0', '1', '2' }, 3)]
    [InlineData(new[] { '0', '1', '2', '3' }, 4)]
    [InlineData(new[] { '0', '1', '2', '3', '4' }, 5)]
    public void InitialIndividualsNextStateIdPropertyIsCorrect(char[] alphabet, int expected)
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet.ToList());

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.Equal(expected, dfaChromosome.NextStateId);
        });
    }

    [Fact]
    public void InitialIndividualsHasAtLeastOneAcceptState()
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() { '0', '1', '2' };
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet);

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.Contains(dfaChromosome.States, s => s.IsAccept);
        });
    }

    [Fact]
    public void InitialIndividualsHasStartStateAssigned()
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() { '0', '1', '2' };
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet);

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.NotNull(dfaChromosome.StartState);
        });
    }

    [Theory]
    [InlineData(new[] { '0' }, 1)]
    [InlineData(new[] { '0', '1' }, 4)]
    [InlineData(new[] { '0', '1', '2' }, 9)]
    [InlineData(new[] { '0', '1', '2', '3' }, 16)]
    [InlineData(new[] { '0', '1', '2', '3', '4' }, 25)]
    public void InitialIndividualsHasNumberOfEdgesAtLeastSquareOfAlphabetSize(char[] alphabet, int expectedLowerBound)
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet.ToList());

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.True(expectedLowerBound <= dfaChromosome.Edges.Count);
        });
    }

    [Theory]
    [InlineData(new[] { '0' }, 1)]
    [InlineData(new[] { '0', '1' }, 5)]
    [InlineData(new[] { '0', '1', '2' }, 11)]
    [InlineData(new[] { '0', '1', '2', '3' }, 19)]
    [InlineData(new[] { '0', '1', '2', '3', '4' }, 29)]
    public void InitialIndividualsHasNumberOfEdgesAtMostSquareOfAlphabetSizePlusAlphabetSizeMinusOne(char[] alphabet,
        int expectedUpperBound)
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet.ToList());

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.True(expectedUpperBound >= dfaChromosome.Edges.Count);
        });
    }


    [Fact]
    public void InitialIndividualsEdgesHasUniqueId()
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() { '0', '1', '2', '3' };
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet);

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.All(dfaChromosome.Edges, e => Assert.Single(dfaChromosome.Edges, e2 => e2.Id == e.Id));
        });
    }

    [Theory]
    [InlineData(new[] { '0' })]
    [InlineData(new[] { '0', '1' })]
    [InlineData(new[] { '0', '1', '2' })]
    [InlineData(new[] { '0', '1', '2', '3' })]
    [InlineData(new[] { '0', '1', '2', '3', '4' })]
    public void InitialIndividualsNextEdgeIdPropertyIsCorrect(char[] alphabet)
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet.ToList());

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.Equal(dfaChromosome.Edges.Count, dfaChromosome.NextEdgeId);
        });
    }

    [Fact]
    public void InitialIndividualsEdgesAreUnique()
    {
        // Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() { '0', '1', '2', '3' };
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet);

        // Act
        population.CreateInitialGeneration();

        // Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.All(dfaChromosome.Edges, e => Assert.Single(dfaChromosome.Edges, e2 => e2.Source == e.Source &&
                e2.Input == e.Input && e2.Target == e.Target));
        });
    }
}
