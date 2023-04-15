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
        //Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() {'0', '1'};
        DFAPopulation population = new DFAPopulation(minSize, maxSize, chromosome, alphabet);
        
        //Act
        population.CreateInitialGeneration();

        //Assert
        Assert.Equal(expected, population.Generations[0].Chromosomes.Count);
    }

    [Theory]
    [InlineData(new []{'0'}, 1)]
    [InlineData(new []{'0', '1'}, 2)]
    [InlineData(new []{'0', '1', '2'}, 3)]
    [InlineData(new []{'0', '1', '2', '3'}, 4)]
    [InlineData(new []{'0', '1', '2', '3', '4'}, 5)]
    public void InitialIndividualsHasNumberOfStatesEqualToAlphabetSize(char[] alphabet, int expected)
    {
        //Arrange
        DFAChromosome chromosome = new DFAChromosome();
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet.ToList());
        
        //Act
        population.CreateInitialGeneration();
        
        //Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.Equal(expected, dfaChromosome.States.Count);
        });
    }

    [Fact]
    public void InitialIndividualsStatesHasUniqueID()
    {
        //Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() {'0', '1', '2', '3'};
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet);
        
        //Act
        population.CreateInitialGeneration();
        
        //Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.All(dfaChromosome.States, s => Assert.Single(dfaChromosome.States, s2 => s2.ID == s.ID));
        });
    }

    [Theory]
    [InlineData(new []{'0'}, 1)]
    [InlineData(new []{'0', '1'}, 2)]
    [InlineData(new []{'0', '1', '2'}, 3)]
    [InlineData(new []{'0', '1', '2', '3'}, 4)]
    [InlineData(new []{'0', '1', '2', '3', '4'}, 5)]
    public void InitialIndividualsNextStateIDPropertyIsCorrect(char[] alphabet, int expected)
    {
        //Arrange
        DFAChromosome chromosome = new DFAChromosome();
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet.ToList());
        
        //Act
        population.CreateInitialGeneration();
        
        //Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.Equal(expected, dfaChromosome.NextStateID);
        });
        
    }
    
    [Fact]
    public void InitialIndividualsHasAtleastOneAcceptState()
    {
        //Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() {'0', '1', '2'};
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet);
        
        //Act
        population.CreateInitialGeneration();
        
        //Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.Contains(dfaChromosome.States, s => s.IsAccept);
        });
    }

    [Fact]
    public void InitialIndividualsHasStartStateAssigned()
    {
        //Arrange
        DFAChromosome chromosome = new DFAChromosome();
        List<char> alphabet = new List<char>() {'0', '1', '2'};
        DFAPopulation population = new DFAPopulation(100, 100, chromosome, alphabet);
        
        //Act
        population.CreateInitialGeneration();
        
        //Assert
        Assert.All(population.Generations[0].Chromosomes, c =>
        {
            DFAChromosome dfaChromosome = (DFAChromosome) c;
            Assert.NotNull(dfaChromosome.StartState);
        });
    }
    
    
    
}
