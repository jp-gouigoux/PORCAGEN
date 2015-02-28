using AForge.Genetic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssistantRepartitionClassesCollege
{
    class FitnessRepartitionClasses : IFitnessFunction
    {
        private ModeleProjet modele = null;
        int N1, N2, N3, N4;

        private Dictionary<ModeleProjet.Prof, double> dicoHeuresServiceProfHorsPreaffectations = new Dictionary<ModeleProjet.Prof, double>();
        private List<ModeleProjet.Classe> classesARepartirEnEntier = null;
        private List<ModeleProjet.Classe> classesARepartir = null;
        private List<ModeleProjet.Classe> classesDeSoutien = null;

        public FitnessRepartitionClasses(ModeleProjet modele)
        {
            // Penser à mettre des validation avant de lancer

            this.modele = modele;

            dicoHeuresServiceProfHorsPreaffectations = modele.Profs.ToDictionary(p => p,
                p => p.Service - modele.Preaffectations
                    .Where(pr => pr.Prof == p.Nom)
                    .Sum(pr => modele.Classes.First(c => c.Nom == pr.Classe).Duree));

            classesARepartirEnEntier = modele.Classes
                .Where(c => !c.AutoriserDecoupe && !modele.Preaffectations.Any(p => p.Classe == c.Nom)).ToList();

            classesDeSoutien = new List<ModeleProjet.Classe>();
            foreach (ModeleProjet.Classe c in modele.Classes.Where(c => c.DureeSoutien > 0))
                classesDeSoutien.Add(new ModeleProjet.Classe() { Nom = c.Nom + "P", AutoriserDecoupe = false, Duree = c.DureeSoutien, Niveau = c.Niveau });

            classesARepartir = modele.Classes.Where(c => c.AutoriserDecoupe && !modele.Preaffectations.Any(p => p.Classe == c.Nom)).Concat(classesDeSoutien).ToList();

            double heuresSupNecessairesPourEquilibre = modele.Classes.Sum(c => c.Duree + c.DureeSoutien) - modele.Profs.Sum(p => p.Service);

            // On rappelle le mode de construction du chromosome :
            // - N1 gènes pour les classes qu'on ne peut pas découper (un prof affecté et un seul)
            // - N2 gènes pour les professeurs à aligner
            // - N3 gènes pour les classes à aligner, c'est-à-dire celles qui ne sont pas marquées "sans découpe", et auquelles on a aussi enlevé celles pré-affectées, plus les simili-classes reprenant les classes avec du soutien
            // - N4 gènes pour le nombre de demi-heures supplémentaires nécessaires pour équilibrer
            N1 = classesARepartirEnEntier.Count();
            N2 = modele.Profs.Count;
            N3 = classesARepartir.Count;
            N4 = (int)(2 * heuresSupNecessairesPourEquilibre);
        }

        public int NombreGenesNecessaires { get { return N1 + N2 + N3 + N4; } }

        public double Evaluate(IChromosome chromosome)
        {
            return Evaluate(chromosome, null);
        }

        public double Evaluate(IChromosome chromosome, StringWriter scribe)
        {
            // A noter qu'on pourrait peut-être utiliser un PermutationChromosome.

            ushort[] genes = ((ShortArrayChromosome)chromosome).Value;

            // Factoriser N2 avec NBPROFS

            // C'est ce dictionnaire qu'on va remplir progressivement, avec les différents cas
            Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf = new Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>>();

            // Au début, on affecte toutes les heures de service des professeurs dans un dictionnaire, qu'on va diminuer au fur et à mesure des affectations
            Dictionary<ModeleProjet.Prof, double> dicoHeuresServiceProfRestantes = new Dictionary<ModeleProjet.Prof, double>(dicoHeuresServiceProfHorsPreaffectations);

            // Affectation des classes sans découpe
            TraiterClassesSansDecoupe(genes, dicoAffectationClasseVersProf, dicoHeuresServiceProfRestantes);

            // Affectation des classes à répartir
            TraiterClassesARepartir(genes, dicoAffectationClasseVersProf, dicoHeuresServiceProfRestantes);

            // Préférence sur le même prof en classe et en soutien pour les classes qui ont du soutien
            double fitnessProfSixiemeIdentiqueAP = CalculerFitnessMemeProfSoutienQueClasse(dicoAffectationClasseVersProf);

            // Prise en compte des préférences de niveaux des profs
            double fitnessPreferenceNiveau = CalculerFitnessPreferenceNiveau(dicoAffectationClasseVersProf);

            // Le moins possible de niveaux différents par prof
            double fitnessMinimumNiveauxDifferents = CalculerFitnessMinimiserNombreNiveauxDifferentsParProf(dicoAffectationClasseVersProf);

            // Fitness sur les découpes de classes, pour l'instant le relatif entre la découpe parfaite et le nombre de créneaux
            int nbSurcreneaux;
            double fitnessMoinsDeDecoupePossible = CalculerFitnessDecoupeMinimaleClasses(dicoAffectationClasseVersProf, out nbSurcreneaux);

            // Pas de créneau de type demi-heure
            double fitnessMoinsDeDecoupePossibleDemiHeure = CalculerFitnessLimiterDecoupageSemiHoraires(dicoAffectationClasseVersProf, nbSurcreneaux);

            // Affichage éventuel des résultats (en général, pour le dernier chromosome, optimal de la simulation)
            if (scribe != null)
            {
                AfficherResultats(dicoAffectationClasseVersProf, scribe);
                scribe.WriteLine("Moins de découpe possible (40%) = " + fitnessMoinsDeDecoupePossible);
                scribe.WriteLine("Moins de découpe possible hors horaire (25%) = " + fitnessMoinsDeDecoupePossibleDemiHeure);
                scribe.WriteLine("Moins de niveaux différents (20%) = " + fitnessMinimumNiveauxDifferents);
                scribe.WriteLine("Prof sixième identique AP (14%) = " + fitnessProfSixiemeIdentiqueAP);
                scribe.WriteLine("Anne sans quatrième (01%) = " + fitnessPreferenceNiveau);
            }

            // Fitness totale
            return 0.40 * fitnessMoinsDeDecoupePossible
                + 0.25 * fitnessMoinsDeDecoupePossibleDemiHeure
                + 0.20 * fitnessMinimumNiveauxDifferents
                + 0.14 * fitnessProfSixiemeIdentiqueAP
                + 0.01 * fitnessPreferenceNiveau;
        }

        private void TraiterClassesSansDecoupe(
            ushort[] genes,
            Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf,
            Dictionary<ModeleProjet.Prof, double> dicoHeuresServiceProfRestantes)
        {
            // Il y a un gène par classe sans découpe, et chacun peut être traité à peu près indépendamment
            for (int i = 0; i < N1; i++)
            {
                // On fait un modulo sur le nombre de prof pour que le gène désigne un prof en particulier
                int indexProfChoisi = genes[i] % modele.Profs.Count;

                // Par contre, si ce prof n'a plus assez d'heures restantes, ou bien s'il ne peut pas avoir ce niveau de classe, on va passer au suivant dans la liste
                while (dicoHeuresServiceProfRestantes[modele.Profs[indexProfChoisi]] < classesARepartirEnEntier[i].Duree
                    || modele.PreferencesNiveaux.Any(
                        pr => pr.Niveau == classesARepartirEnEntier[i].Niveau
                        && pr.Prof == modele.Profs[indexProfChoisi].Nom
                        && pr.Mode == Preference.NePeutPasAvoir))
                    indexProfChoisi = (indexProfChoisi + 1) % modele.Profs.Count;

                // Quand on a trouvé le prof qui prendra cette classe, on diminue d'autant ses heures restant à affecter
                dicoHeuresServiceProfRestantes[modele.Profs[indexProfChoisi]] -= classesARepartirEnEntier[i].Duree;

                // Ajout du résultat dans le dictionnaire des affectations de classes
                Dictionary<ModeleProjet.Prof, double> presenceProf = new Dictionary<ModeleProjet.Prof, double>();
                presenceProf.Add(modele.Profs[indexProfChoisi], classesARepartirEnEntier[i].Duree);
                dicoAffectationClasseVersProf.Add(classesARepartirEnEntier[i], presenceProf);
            }
        }

        private void TraiterClassesARepartir(
            ushort[] genes,
            Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf,
            Dictionary<ModeleProjet.Prof, double> dicoHeuresServiceProfRestantes)
        {
            // Récupération de la liste ordonnée des profs
            int[] positionsProfs = new int[modele.Profs.Count];
            List<int> positionsRestantes = Enumerable.Range(0, modele.Profs.Count).ToList();
            for (int i = 0; i < modele.Profs.Count; i++)
            {
                ushort gene = genes[N1 + i];
                int offset = gene % (modele.Profs.Count - i);
                positionsProfs[i] = positionsRestantes[offset];
                positionsRestantes.RemoveAt(offset);
            }

            // Récupération de la liste ordonnée des classes
            int[] positionsClasses = new int[classesARepartir.Count];
            positionsRestantes = Enumerable.Range(0, classesARepartir.Count).ToList();
            for (int i = 0; i < classesARepartir.Count; i++)
            {
                ushort gene = genes[N1 + N2 + i];
                int offset = gene % (classesARepartir.Count - i);
                positionsClasses[i] = positionsRestantes[offset];
                positionsRestantes.RemoveAt(offset);
            }

            // On ajuste les heures des profs avec les demi-heures supplémentaires
            for (int i = 0; i < N4; i++)
            {
                int indexProfChoisi = genes[N1 + N2 + N3 + i] % modele.Profs.Count;
                dicoHeuresServiceProfRestantes[modele.Profs[indexProfChoisi]] += 0.5;
            }

            // Parcours des frontières des classes
            List<double> frontieresClasses = new List<double>();
            double cumulClasses = 0;
            List<Tuple<double, double, int>> repartitionClasses = new List<Tuple<double, double, int>>();
            for (int i = 0; i < classesARepartir.Count; i++)
            {
                double backupCumul = cumulClasses;
                cumulClasses += classesARepartir[positionsClasses[i]].Duree;
                frontieresClasses.Add(cumulClasses);
                repartitionClasses.Add(new Tuple<double, double, int>(backupCumul, cumulClasses, positionsClasses[i]));
            }

            // Parcours des frontières des profs
            List<double> frontieresHeuresProfs = new List<double>();
            double cumulHeuresProfs = 0;
            List<Tuple<double, double, int>> repartitionProfs = new List<Tuple<double, double, int>>();
            for (int i = 0; i < modele.Profs.Count; i++)
            {
                double backupCumul = cumulHeuresProfs;
                cumulHeuresProfs += dicoHeuresServiceProfRestantes[modele.Profs[positionsProfs[i]]];
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
            foreach (ModeleProjet.Classe classeARepartir in classesARepartir)
                dicoAffectationClasseVersProf.Add(classeARepartir, new Dictionary<ModeleProjet.Prof,double>());
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
                Dictionary<ModeleProjet.Prof, double> dico = dicoAffectationClasseVersProf[classesARepartir[positionsClasses[indexRepartitionClasse]]];

                // On a également l'index du prof, ce qui permet de rajouter les bonnes heures
                double dureeCreneau = frontieres[i];
                if (i > 0) dureeCreneau -= frontieres[i - 1];
                dico.Add(modele.Profs[positionsProfs[indexRepartitionProf]], dureeCreneau);
            }
        }

        private double CalculerFitnessMemeProfSoutienQueClasse(Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf)
        {
            double fitnessProfSixiemeIdentiqueAP = 0.0;
            foreach (ModeleProjet.Classe classeDeSoutien in classesDeSoutien)
            {
                // On retrouve d'abord quel prof a la classe en soutien
                Dictionary<ModeleProjet.Prof, double> affectationSoutien = dicoAffectationClasseVersProf[classeDeSoutien];
                ModeleProjet.Prof profAffecteAuSoutien = affectationSoutien.First().Key;

                // Ensuite, on regarde d'abord si le prof n'a pas été préaffecté à la classe
                string nomClasseCoursCorrespondante = classeDeSoutien.Nom.Substring(0, 2);
                ModeleProjet.Prof profAffecteAuCours = null;
                ModeleProjet.Preaffectation preaff = modele.Preaffectations.FirstOrDefault(pr => pr.Classe == nomClasseCoursCorrespondante);
                if (preaff != null)
                    profAffecteAuCours = modele.Profs.Find(p => p.Nom == preaff.Prof);

                // S'il n'a pas été préaffecté, on le cherche dans le dictionnaire d'affectation issu de la simulation
                if (profAffecteAuCours == null)
                {
                    ModeleProjet.Classe classeDeCours = modele.Classes.Find(c => c.Nom == nomClasseCoursCorrespondante);
                    Dictionary<ModeleProjet.Prof, double> affectationCours = dicoAffectationClasseVersProf[classeDeCours];
                    profAffecteAuCours = affectationCours.First().Key;
                }

                // Si les deux profs correspondent, ça augmente la fitness
                if (profAffecteAuCours == profAffecteAuSoutien)
                    fitnessProfSixiemeIdentiqueAP += 1.0 / classesDeSoutien.Count();
            }

            return fitnessProfSixiemeIdentiqueAP;
        }

        private double CalculerFitnessPreferenceNiveau(
            Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf)
        {
            double fitnessPreferenceNiveau = 0.0;
            IEnumerable<ModeleProjet.PreferenceNiveau> preferencesNonObligatoires = modele.PreferencesNiveaux.Where(pr => pr.Mode != Preference.NePeutPasAvoir);
            foreach (ModeleProjet.PreferenceNiveau pref in preferencesNonObligatoires)
            {
                ModeleProjet.Prof profConcerne = modele.Profs.Find(p => p.Nom == pref.Prof);
                bool preferenceRespectee = pref.Mode == Preference.NePreferePasAvoir;
                foreach (KeyValuePair<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> couple in dicoAffectationClasseVersProf)
                    if (couple.Key.Niveau == pref.Niveau && couple.Value.ContainsKey(profConcerne))
                    {
                        preferenceRespectee = pref.Mode != Preference.NePreferePasAvoir;
                        break;
                    }
                if (preferenceRespectee) fitnessPreferenceNiveau += 1.0 / preferencesNonObligatoires.Count();
            }
            return fitnessPreferenceNiveau;
        }

        private double CalculerFitnessMinimiserNombreNiveauxDifferentsParProf(
            Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf)
        {
            Dictionary<ModeleProjet.Prof, List<Niveau>> dicoNiveauxParProf = new Dictionary<ModeleProjet.Prof, List<Niveau>>();
            foreach (KeyValuePair<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> affectation in dicoAffectationClasseVersProf)
            {
                ModeleProjet.Classe curClasse = affectation.Key;
                foreach (KeyValuePair<ModeleProjet.Prof, double> couple in affectation.Value)
                {
                    ModeleProjet.Prof curProf = couple.Key;
                    if (!dicoNiveauxParProf.ContainsKey(curProf))
                        dicoNiveauxParProf.Add(curProf, new List<Niveau>());
                    if (!dicoNiveauxParProf[curProf].Contains(curClasse.Niveau))
                        dicoNiveauxParProf[curProf].Add(curClasse.Niveau);
                }
            }

            // En théorie, la fitness parfaite serait de NBPROFS (tous les profs ont un seul niveau), et celle la plus pourrie
            // serait de NBCLASSES (tous les profs n'ont que des niveaux différents pour chacune de leurs classes)
            //double fitnessMinimumNiveauxDifferents = (0.0 - 1.0 / (NBCLASSES - NBPROFS)) * totalNiveauxProfs + 1.0 - NBPROFS * (0.0 - 1.0 / (NBCLASSES - NBPROFS));
            //double fitnessMinimumNiveauxDifferents = 1.0 + (((double)totalNiveauxProfs - NBCLASSES) / (NBCLASSES - NBPROFS));
            int totalNiveauxProfs = dicoNiveauxParProf.Sum(couple => couple.Value.Count);
            double fitnessMinimumNiveauxDifferents = 1.0 - (totalNiveauxProfs - 10.0) / 20.0;
            fitnessMinimumNiveauxDifferents = Math.Min(1.0, Math.Max(0.0, fitnessMinimumNiveauxDifferents));
            return fitnessMinimumNiveauxDifferents;
        }

        private double CalculerFitnessDecoupeMinimaleClasses(
            Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf,
            out int nbSurcreneaux)
        {
            int nbAffectations = dicoAffectationClasseVersProf.Count;
            int nbCreneauxTotaux = 0;
            foreach (KeyValuePair<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> couple in dicoAffectationClasseVersProf)
                nbCreneauxTotaux += couple.Value.Count;
            nbSurcreneaux = nbCreneauxTotaux - nbAffectations;
            double Valeur = 1.0 - nbSurcreneaux / 2.0; // A deux découpages de classes, on a une fitness de 0
            return Math.Max(0, Valeur); // Juste pour empêcher les fitness négatives
        }

        private static double CalculerFitnessLimiterDecoupageSemiHoraires(
            Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf,
            int nbSurcreneaux)
        {
            int nbCreneauxSemiHoraires = 0;
            foreach (KeyValuePair<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> affectation in dicoAffectationClasseVersProf)
            {
                if (affectation.Value.Count > 1)
                {
                    foreach (KeyValuePair<ModeleProjet.Prof, double> couple in affectation.Value)
                    {
                        double dureePremierCreneau = couple.Value;
                        if (dureePremierCreneau != Math.Floor(dureePremierCreneau))
                            nbCreneauxSemiHoraires++;
                        break; // On s'arrête au premier, vu qu'il ne peut y en avoir qu'un, et lui aussi sur la demi-heure si le premier creneau n'est lui-même pas calé sur l'heure
                    }
                }
            }
            double fitnessMoinsDeDecoupePossibleDemiHeure =
                nbSurcreneaux == 0
                ? 1.0
                : 1.0 - nbCreneauxSemiHoraires / nbSurcreneaux;
            fitnessMoinsDeDecoupePossibleDemiHeure = Math.Max(0, fitnessMoinsDeDecoupePossibleDemiHeure);
            return fitnessMoinsDeDecoupePossibleDemiHeure;
        }

        private void AfficherResultats(Dictionary<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> dicoAffectationClasseVersProf, StringWriter scribe)
        {
            // Trier le dico par ordre alphabétique de classes
            //dicoAffectationClasseVersProf

            foreach(KeyValuePair<ModeleProjet.Classe, Dictionary<ModeleProjet.Prof, double>> affectation in dicoAffectationClasseVersProf)
            {
                scribe.Write(affectation.Key.Nom);
                scribe.Write(" : { ");
                List<string> horaires = new List<string>();
                foreach (KeyValuePair<ModeleProjet.Prof, double> couple in affectation.Value)
                {
                    string horaire = couple.Key.Nom;
                    horaire += ": ";
                    horaire += couple.Value.ToString("F1");
                    horaire += " h";
                    horaires.Add(horaire);
                }
                scribe.Write(string.Join(" / ", horaires.ToArray()));
                scribe.WriteLine(" }");
            }
        }
    }
}
