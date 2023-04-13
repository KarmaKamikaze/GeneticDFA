using GeneticSharp;

namespace GeneticDFA;

public class DFAPopulation : Population
{
    public DFAPopulation(int minSize, int maxSize, IChromosome adamChromosome, List<char> alphabet) : base(minSize, maxSize, adamChromosome)
    {
        Alphabet = alphabet;
    }

    public List<char> Alphabet { get; }

    public override void CreateInitialGeneration()
    {
        Generations = new List<Generation>();
        GenerationsNumber = 0;
        List<IChromosome> chromosomes = new List<IChromosome>();
        for (int index = 0; index < MinSize; ++index)
        {
            IChromosome chromosome = AdamChromosome.CreateNew();
            if (chromosome == null)
                throw new InvalidOperationException("Adam chromosome's 'CreateNew' method created a null chromosome.");
            InitializeChromosome(chromosome);
            chromosomes.Add(chromosome);
        }
        CreateNewGeneration(chromosomes);
        
        
    }

    private void InitializeChromosome(IChromosome paramChromosome)
    {
        DFAChromosome chromosome = (DFAChromosome) paramChromosome;
        int numberOfAcceptStates = RandomizationProvider.Current.GetInt(1, Alphabet.Count);
        for (int i = 0; i < numberOfAcceptStates; i++)
        {
            chromosome.States.Add(new DFAState(chromosome.NextStateID, true));
            chromosome.NextStateID++;
        }
        for (int i = numberOfAcceptStates; i < Alphabet.Count; i++)
        {
            chromosome.States.Add(new DFAState(chromosome.NextStateID, false));
            chromosome.NextStateID++;
        }

        int startStateID = RandomizationProvider.Current.GetInt(0, chromosome.States.Count-1);
        chromosome.StartState = chromosome.States.First(s => s.ID == startStateID);
        
        
        
        
    }
    
    //Copied from source code, but without gene validation, since we do not use the gene properties on chromosomes
    public override void CreateNewGeneration(IList<IChromosome> chromosomes)
    {
        ExceptionHelper.ThrowIfNull(nameof (chromosomes), chromosomes);
        chromosomes.ValidateGenes();
        CurrentGeneration = new Generation(++GenerationsNumber, chromosomes);
        Generations.Add(CurrentGeneration);
        GenerationStrategy.RegisterNewGeneration( this);
    }
    
    
    
}
