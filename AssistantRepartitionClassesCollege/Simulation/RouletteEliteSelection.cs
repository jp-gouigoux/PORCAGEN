using AForge.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssistantRepartitionClassesCollege
{
    class RouletteEliteSelection : ISelectionMethod
    {
        public void ApplySelection(List<IChromosome> chromosomes, int size)
        {
            // On commence par reprendre systématiquement le meilleur chromosome
            List<IChromosome> NouvelleGeneration = new List<IChromosome>();
            double BestFitness = 0.0;
            foreach (IChromosome Chromosome in chromosomes)
                BestFitness = Math.Max(BestFitness, Chromosome.Fitness);

            IChromosome MeilleurChromosome = chromosomes.Find(delegate(IChromosome Chromosome)
            {
                return Chromosome.Fitness == BestFitness;
            });

            NouvelleGeneration.Add(MeilleurChromosome);
            double TotalDesFitness = 0.0;
            chromosomes.ForEach(delegate(IChromosome Chromosome)
            {
                TotalDesFitness += Chromosome.Fitness;
            });

            // Ensuite, on choisit au hasard le reste de la population, en donnant d'autant
            // plus de chance d'appartenir à la nouvelle génération que la fitness est élevée.
            Random Generateur = new Random(DateTime.Now.Second + DateTime.Now.Millisecond);
            while (--size > 0)
            {
                double PositionHasard = Generateur.NextDouble() * TotalDesFitness;
                double FitnessCumulee = 0.0;
                foreach (IChromosome Chromosome in chromosomes)
                {
                    FitnessCumulee += Chromosome.Fitness;
                    if (FitnessCumulee > PositionHasard)
                    {
                        NouvelleGeneration.Add(Chromosome);
                        break;
                    }
                }
            }

            chromosomes.Clear();
            chromosomes.AddRange(NouvelleGeneration);
        }
    }
}
