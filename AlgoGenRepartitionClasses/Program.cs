using AForge.Genetic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoGenRepartitionClasses
{
    class Program
    {
        static void Main(string[] args)
        {
            // 3 affectations de troisièmes + 7 profs à aligner sur 21 classes sauf 2 préaffectées, 
            // et moins 3 troisièmes qui doivent rester sans découpe, mais + 5 sixièmes "AP" (la demi-heure
            // d'accompagnement) + 3 demi-heures supplémentaires
            IChromosome IndividuRacine = new ShortArrayChromosome(3 + 7 + (21 - 2 - 3 + 5) + 3);

            //new AForge.Genetic.PermutationChromosome

            FitnessRepartitionClasses fitness = new FitnessRepartitionClasses();
            Population Population = new Population(100,
                IndividuRacine,
                fitness,
                new RouletteEliteSelection());

            int Iteration = 0;
            ShortArrayChromosome Meilleur = null;
            while (Iteration++ < 1000)
            {
                Population.RunEpoch();
                Meilleur = (ShortArrayChromosome)Population.BestChromosome;
            }
            double fitValue = fitness.Evaluate(Meilleur, true);
            Console.WriteLine("fitness = " + fitValue);
        }
    }
}
