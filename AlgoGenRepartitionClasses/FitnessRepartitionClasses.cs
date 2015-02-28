using AForge.Genetic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoGenRepartitionClasses
{
    class FitnessRepartitionClasses : IFitnessFunction
    {
        string[] profs = { "Elizabeth", "Anne", "Guénaëlle", "Julie", "BMP18", "BMP9", "Morgane" };
        double[] heuresProfs = { 10, 18, 4.5, 15, 18, 8, 9 }; // Soit 82,5 heures
        string[] classes = { "6B", "6C", "6D", "6E", "6AP", "6BP", "6CP", "6DP", "6EP", "5A", "5B", "5C", "5D", "5E", "5F", "4A", "4B", "4C", "4D", "4E", "4F" };
        double[] dureesClasses = { 5, 5, 5, 5, 0.5, 0.5, 0.5, 0.5, 0.5, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 }; // Soit 84 heures
        int[] niveauClasses = { 6, 6, 6, 6, 6, 6, 6, 6, 6, 5, 5, 5, 5, 5, 5, 4, 4, 4, 4, 4, 4 };
        public double Evaluate(IChromosome chromosome)
        {
            return Evaluate(chromosome, false);
        }

        public double Evaluate(IChromosome chromosome, bool afficherResultat)
        {
            ushort[] genes = ((ShortArrayChromosome)chromosome).Value;

            // Affectation des trois troisièmes (3A pré-affectée à Gwen)
            int NBPROFS = profs.Length;
            double[] heuresProfsRestantes = new double[NBPROFS];
            Array.Copy(heuresProfs, heuresProfsRestantes, NBPROFS);
            int indexProfTroisiemeB = genes[0] % (NBPROFS - 1); // NBPROFS - 1, pour que Morgane, en dernier sur la liste, n'aie pas de troisième
            heuresProfsRestantes[indexProfTroisiemeB] -= 4.5;
            int indexProfTroisiemeC = genes[1] % (NBPROFS - 1); // NBPROFS - 1, pour que Morgane, en dernier sur la liste, n'aie pas de troisième
            while (heuresProfsRestantes[indexProfTroisiemeC] < 4.5)
                indexProfTroisiemeC = (indexProfTroisiemeC + 1) % (NBPROFS - 1); // NBPROFS - 1, pour que Morgane, en dernier sur la liste, n'aie pas de troisième
            heuresProfsRestantes[indexProfTroisiemeC] -= 4.5;
            int indexProfTroisiemeD = genes[2] % (NBPROFS - 1); // NBPROFS - 1, pour que Morgane, en dernier sur la liste, n'aie pas de troisième
            while (heuresProfsRestantes[indexProfTroisiemeD] < 4.5)
                indexProfTroisiemeD = (indexProfTroisiemeD + 1) % (NBPROFS - 1); // NBPROFS - 1, pour que Morgane, en dernier sur la liste, n'aie pas de troisième
            heuresProfsRestantes[indexProfTroisiemeD] -= 4.5;

            // Affichage éventuel des profs de troisième
            if (afficherResultat)
            {
                Console.Write("3A: " + profs[2]);
                Console.Write(" | 3B: " + profs[indexProfTroisiemeB]);
                Console.Write(" | 3C: " + profs[indexProfTroisiemeC]);
                Console.WriteLine(" | 3D: " + profs[indexProfTroisiemeD]);
            }

            // Récupération de la liste ordonnée des profs
            int[] positionsProfs = new int[NBPROFS];
            List<int> positionsRestantes = Enumerable.Range(0, NBPROFS).ToList();
            for (int i = 0; i < NBPROFS; i++)
            {
                ushort gene = genes[3 + i]; // On décale de trois, car on a d'abord les gènes sur les troisièmes
                int offset = gene % (NBPROFS - i);
                positionsProfs[i] = positionsRestantes[offset];
                positionsRestantes.RemoveAt(offset);
            }

            // Récupération de la liste ordonnée des classes
            int NBCLASSES = classes.Length;
            int[] positionsClasses = new int[NBCLASSES];
            positionsRestantes = Enumerable.Range(0, NBCLASSES).ToList();
            for (int i = 0; i < NBCLASSES; i++)
            {
                ushort gene = genes[3 + NBPROFS + i]; // Pour provoquer le décalage, les gènes sur les classes étant à la suite de ceux sur les profs
                int offset = gene % (NBCLASSES - i);
                positionsClasses[i] = positionsRestantes[offset];
                positionsRestantes.RemoveAt(offset);
            }

            // On ajuste les heures des profs avec les heures supplémentaires
            // Elizabeth
            if (genes[3 + NBPROFS + NBCLASSES] % 3 == 0) heuresProfsRestantes[0] += 0.5;
            if (genes[3 + NBPROFS + NBCLASSES + 1] % 3 == 0) heuresProfsRestantes[0] += 0.5;
            if (genes[3 + NBPROFS + NBCLASSES + 2] % 3 == 0) heuresProfsRestantes[0] += 0.5;
            // Anne
            if (genes[3 + NBPROFS + NBCLASSES] % 3 == 1) heuresProfsRestantes[1] += 0.5;
            if (genes[3 + NBPROFS + NBCLASSES + 1] % 3 == 1) heuresProfsRestantes[1] += 0.5;
            if (genes[3 + NBPROFS + NBCLASSES + 2] % 3 == 1) heuresProfsRestantes[1] += 0.5;
            // BMP18
            if (genes[3 + NBPROFS + NBCLASSES] % 3 == 2) heuresProfsRestantes[5] += 0.5;
            if (genes[3 + NBPROFS + NBCLASSES + 1] % 3 == 2) heuresProfsRestantes[5] += 0.5;
            if (genes[3 + NBPROFS + NBCLASSES + 2] % 3 == 2) heuresProfsRestantes[5] += 0.5;

            // Parcours des frontières des classes
            List<double> frontieresClasses = new List<double>();
            double cumulClasses = 0;
            List<Tuple<double, double, int>> repartitionClasses = new List<Tuple<double, double, int>>();
            for (int i = 0; i < NBCLASSES; i++)
            {
                double backupCumul = cumulClasses;
                cumulClasses += dureesClasses[positionsClasses[i]];
                frontieresClasses.Add(cumulClasses);
                repartitionClasses.Add(new Tuple<double, double, int>(backupCumul, cumulClasses, positionsClasses[i]));
            }

            // Parcours des frontières des profs
            List<double> frontieresHeuresProfs = new List<double>();
            double cumulHeuresProfs = 0;
            List<Tuple<double, double, int>> repartitionProfs = new List<Tuple<double, double, int>>();
            for (int i = 0; i < NBPROFS; i++)
            {
                double backupCumul = cumulHeuresProfs;
                cumulHeuresProfs += heuresProfsRestantes[positionsProfs[i]];
                frontieresHeuresProfs.Add(cumulHeuresProfs);
                repartitionProfs.Add(new Tuple<double, double, int>(backupCumul, cumulHeuresProfs, positionsProfs[i]));
            }

            // On doit arriver au même cumul sinon problème
            if (cumulClasses != cumulHeuresProfs) throw new ApplicationException("Assert : Les cumuls doivent matcher");

            // Récupération de toutes les frontières (sans duplication)
            List<double> frontieres = new List<double>(frontieresClasses);
            foreach (double f in frontieresHeuresProfs)
                if (!frontieres.Contains(f))
                    frontieres.Add(f);
            frontieres.Sort();

            // On boucle ensuite pour remplir les affectations, potentiellement multiples
            List<Dictionary<int, double>> affectationsClasses = new List<Dictionary<int, double>>();
            for (int i = 0; i < NBCLASSES; i++)
                affectationsClasses.Add(new Dictionary<int, double>());
            for (int i = 0; i < frontieres.Count; i++)
            {
                // On avance jusqu'à la première répartition de classe concernée
                int indexRepartitionClasse = 0;
                while (repartitionClasses[indexRepartitionClasse].Item2 < frontieres[i])
                    indexRepartitionClasse++;

                // Même chose pour trouver le prof concerné
                int indexRepartitionProf = 0;
                while (repartitionProfs[indexRepartitionProf].Item2 < frontieres[i])
                    indexRepartitionProf++;

                // On a alors l'index de la classe, ce qui permet de se positionner dans le dico des affectations
                Dictionary<int, double> dico = affectationsClasses[repartitionClasses[indexRepartitionClasse].Item3];

                // On a également l'index du prof, ce qui permet de rajouter les bonnes heures
                double dureeCreneau = frontieres[i];
                if (i > 0) dureeCreneau -= frontieres[i - 1];
                dico.Add(repartitionProfs[indexRepartitionProf].Item3, dureeCreneau);
            }

            // Affichage éventuel des résultats
            if (afficherResultat)
            {
                for (int i = 0; i < affectationsClasses.Count; i++)
                {
                    Console.Write(classes[i]);
                    Console.Write(" : { ");
                    List<string> horaires = new List<string>();
                    foreach (KeyValuePair<int, double> couple in affectationsClasses[i])
                    {
                        string horaire = profs[couple.Key];
                        horaire += ": ";
                        horaire += couple.Value.ToString("F1");
                        horaire += " h";
                        horaires.Add(horaire);
                    }
                    Console.Write(string.Join(" / ", horaires.ToArray()));
                    Console.WriteLine(" }");
                }
            }

            // Préférence sur les 6P sur le même prof que le 6
            // "6B", "6C", "6D", "6E", "6AP", "6BP", "6CP", "6DP", "6EP"
            //   0,    1,    2,    3,     4,     5,     6,     7,     8
            double fitnessProfSixiemeIdentiqueAP = 0.0;
            int indexProf6AP = -1;
            foreach (KeyValuePair<int, double> couple in affectationsClasses[4])
                indexProf6AP = couple.Key;
            if (indexProf6AP == NBPROFS - 1) fitnessProfSixiemeIdentiqueAP += 0.2; // 6A préaffectée à Morgane, dernière de la liste pour éviter les troisièmes
            for (int i = 0; i < 4; i++)
            {
                int indexProf6P = -1;
                foreach (KeyValuePair<int, double> couple in affectationsClasses[5 + i])
                    indexProf6P = couple.Key;
                int indexProf6 = -1;
                if (affectationsClasses[i].Count == 1)
                    foreach (KeyValuePair<int, double> couple in affectationsClasses[i])
                        indexProf6 = couple.Key;
                if (indexProf6P == indexProf6) fitnessProfSixiemeIdentiqueAP += 0.2;
            }

            // Anne de préférence pas de quatrième
            double fitnessAnneSansQuatrieme = 1.0;
            for (int i = 0; i < affectationsClasses.Count; i++)
			{
                if (niveauClasses[i] == 4)
                {
                    foreach (KeyValuePair<int, double> couple in affectationsClasses[i])
                        if (couple.Key == 1)
                        {
                            fitnessAnneSansQuatrieme = 0.0;
                            break;
                        }
                }
            }

            // Le moins possible de niveaux différents par prof
            Dictionary<int, List<int>> dicoNiveauxParProf = new Dictionary<int, List<int>>();
            for (int i = 0; i < affectationsClasses.Count; i++)
            {
                int niveauClasse = niveauClasses[i];
                foreach (KeyValuePair<int, double> couple in affectationsClasses[i])
                {
                    int indexProf = couple.Key;
                    if (!dicoNiveauxParProf.ContainsKey(indexProf))
                        dicoNiveauxParProf.Add(indexProf, new List<int>());
                    if (!dicoNiveauxParProf[indexProf].Contains(niveauClasse))
                        dicoNiveauxParProf[indexProf].Add(niveauClasse);
                }
            }

            foreach (int indexProf in new int[] { indexProfTroisiemeB, indexProfTroisiemeC, indexProfTroisiemeD })
            {
                if (!dicoNiveauxParProf.ContainsKey(indexProf))
                    dicoNiveauxParProf.Add(indexProf, new List<int>());
                if (!dicoNiveauxParProf[indexProf].Contains(3))
                    dicoNiveauxParProf[indexProf].Add(3);
            }

            int totalNiveauxProfs = 0;
            foreach (KeyValuePair<int, List<int>> couple in dicoNiveauxParProf)
                totalNiveauxProfs += couple.Value.Count;
            // En théorie, la fitness parfaite serait de NBPROFS (tous les profs ont un seul niveau), et celle la plus pourrie
            // serait de NBCLASSES (tous les profs n'ont que des niveaux différents pour chacune de leurs classes)
            //double fitnessMinimumNiveauxDifferents = (0.0 - 1.0 / (NBCLASSES - NBPROFS)) * totalNiveauxProfs + 1.0 - NBPROFS * (0.0 - 1.0 / (NBCLASSES - NBPROFS));
            //double fitnessMinimumNiveauxDifferents = 1.0 + (((double)totalNiveauxProfs - NBCLASSES) / (NBCLASSES - NBPROFS));
            double fitnessMinimumNiveauxDifferents = 1.0 - (totalNiveauxProfs - 10.0) / 20.0;
            fitnessMinimumNiveauxDifferents = Math.Min(1.0, Math.Max(0.0, fitnessMinimumNiveauxDifferents));

            // Fitness sur les découpes de classes, pour l'instant le relatif entre la découpe parfaite et le nombre de créneaux
            int nbAffectations = affectationsClasses.Count;
            int nbCreneauxTotaux = 0;
            foreach (Dictionary<int, double> dico in affectationsClasses)
                nbCreneauxTotaux += dico.Count;
            int nbSurcreneaux = nbCreneauxTotaux - nbAffectations;
            double Valeur = 1.0 - nbSurcreneaux / 2.0; // A deux découpages de classes, on a une fitness de 0
            double fitnessMoinsDeDecoupePossible = Math.Max(0, Valeur); // Juste pour empêcher les fitness négatives

            // Pas de créneau de type demi-heure
            int nbCreneauxSemiHoraires = 0;
            foreach (Dictionary<int, double> dico in affectationsClasses)
            {
                if (dico.Count > 1)
                {
                    foreach (KeyValuePair<int, double> couple in dico)
                    {
                        double dureePremierCreneau = couple.Value;
                        if (dureePremierCreneau != Math.Floor(dureePremierCreneau))
                            nbCreneauxSemiHoraires++;
                        break; // On s'arrête au premier, vu qu'il ne peut y en avoir qu'un, et lui aussi sur la demi-heure si le premier creneau n'est lui-même pas calé sur l'heure
                    }
                }
            }
            double fitnessMoinsDeDecoupePossibleDemiHeure = 1.0 - nbCreneauxSemiHoraires / nbSurcreneaux;
            fitnessMoinsDeDecoupePossibleDemiHeure = Math.Max(0, fitnessMoinsDeDecoupePossibleDemiHeure);

            // Fitness totale
            if (afficherResultat)
            {
                Console.WriteLine("Moins de découpe possible (40%) = " + fitnessMoinsDeDecoupePossible);
                Console.WriteLine("Moins de découpe possible hors horaire (25%) = " + fitnessMoinsDeDecoupePossibleDemiHeure);
                Console.WriteLine("Moins de niveaux différents (20%) = " + fitnessMinimumNiveauxDifferents);
                Console.WriteLine("Prof sixième identique AP (14%) = " + fitnessProfSixiemeIdentiqueAP);
                Console.WriteLine("Anne sans quatrième (01%) = " + fitnessAnneSansQuatrieme);
            }
            return 0.40 * fitnessMoinsDeDecoupePossible
                + 0.25 * fitnessMoinsDeDecoupePossibleDemiHeure
                + 0.20 * fitnessMinimumNiveauxDifferents
                + 0.14 * fitnessProfSixiemeIdentiqueAP
                + 0.01 * fitnessAnneSansQuatrieme;
        }
    }
}
