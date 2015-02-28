using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AssistantRepartitionClassesCollege
{
    public enum Niveau : int
    {
        Troisième = 3,
        Quatrième = 4,
        Cinquième = 5,
        Sixième = 6
    }

    public enum Preference : int
    {
        PrefereAvoir,
        //DoitAvoir,
        NePreferePasAvoir,
        NePeutPasAvoir
    }

    public class ModeleProjet
    {
        public class Classe
        {
            [XmlAttribute]
            public string Nom { get; set; }
            [XmlAttribute]
            public Niveau Niveau { get; set; }
            [XmlAttribute]
            public double Duree { get; set; }
            [XmlAttribute]
            public double DureeSoutien { get; set; }
            [XmlAttribute]
            public bool AutoriserDecoupe { get; set; }

            public override string ToString()
            {
                return string.Concat(Nom, " : ", Duree.ToString("F1"), "h", DureeSoutien > 0 ? ("+" + DureeSoutien.ToString("F1") + "h") : string.Empty);
            }
        }

        public class Prof
        {
            [XmlAttribute]
            public string Nom { get; set; }
            [XmlAttribute]
            public double Service { get; set; }
            [XmlAttribute]
            public double MaxHeuresSup { get; set; }

            public override string ToString()
            {
                return string.Concat(Nom, " : ", Service.ToString("F1"), "h+", MaxHeuresSup.ToString("F1") + "h");
            }
        }

        public class Preaffectation
        {
            [XmlAttribute]
            public string Classe { get; set; }
            [XmlAttribute]
            public string Prof { get; set; }

            public override string ToString()
            {
                return string.Concat(Classe, " => ", Prof);
            }
        }

        public class PreferenceNiveau
        {
            [XmlAttribute]
            public string Prof { get; set; }
            [XmlAttribute]
            public Preference Mode { get; set; }
            [XmlAttribute]
            public Niveau Niveau { get; set; }

            public override string ToString()
            {
                return string.Concat(Prof, " ", Mode, " ", Niveau);
            }
        }

        public class CriteresCalcul
        {
            [XmlAttribute]
            public int CritereNonDecoupageClasses { get; set; }
            [XmlAttribute]
            public int CritereDecoupageSurHeurePleine { get; set; }
            [XmlAttribute]
            public int CritereLimiterNiveauxParProf { get; set; }
            [XmlAttribute]
            public int CriterePrivilegierMemeProfCoursEtSoutien { get; set; }
            [XmlAttribute]
            public int CriterePriseEnComptePreferencesNiveaux { get; set; }
        }

        [XmlIgnore]
        public bool isDirty = true;

        public List<Classe> Classes { get; set; }
        public List<Prof> Profs { get; set; }
        public List<Preaffectation> Preaffectations { get; set; }
        public List<PreferenceNiveau> PreferencesNiveaux { get; set; }
        public CriteresCalcul Criteres { get; set; }

        public int TaillePopulation { get; set; }
        public int NombreIterations { get; set; }

        private static XmlSerializer _serialiseur = null;

        [XmlIgnore]
        internal static XmlSerializer serialiseur
        {
            get { return _serialiseur != null ? _serialiseur : _serialiseur = new XmlSerializer(typeof(ModeleProjet)); }
        }

        private double HeuresServices { get { return Profs.Sum(p => p.Service); } }
        private double HeuresSupTotalMax { get { return Profs.Sum(p => p.MaxHeuresSup); } }
        private double HeuresClasses { get { return Classes.Sum(c => c.Duree); } }
        private double HeuresSoutien { get { return Classes.Sum(c => c.DureeSoutien); } }
        private double HeuresCoursTotal { get { return HeuresClasses + HeuresSoutien; } }
        private double HeuresProfsMaxVoulu { get { return HeuresServices + HeuresSupTotalMax; } }
        private double HeuresProfsMaxTheorique { get { return HeuresServices + 5 * Profs.Count; } } // Variabiliser le 5 si on le fait un jour dans le contrôle graphique


        public ModeleProjet()
        {
            Classes = new List<Classe>();
            Profs = new List<Prof>();
            Preaffectations = new List<Preaffectation>();
            PreferencesNiveaux = new List<PreferenceNiveau>();
            Criteres = new CriteresCalcul();

            TaillePopulation = 1000;
            NombreIterations = 1000;
        }

        internal void Save(string NomFichierModele)
        {
            StringWriter scribe = new StringWriter();
            serialiseur.Serialize(scribe, this);
            File.WriteAllText(NomFichierModele, scribe.ToString());
        }

        internal static ModeleProjet Load(string NomFichierModele)
        {
            string contenu = File.ReadAllText(NomFichierModele);
            StringReader lecteur = new StringReader(contenu);
            return serialiseur.Deserialize(lecteur) as ModeleProjet;
        }

        internal string AfficherEntetePreCalcul()
        {
            string entete = "Heures disponibles : " + HeuresServices + " (service) + " + HeuresSupTotalMax + " (heures sup max) = " + HeuresProfsMaxVoulu + " heures";
            entete += Environment.NewLine;
            entete += "Heures à servir : " + HeuresClasses + " (cours) + " + HeuresSoutien + " (soutien) = " + HeuresCoursTotal + " heures";
            return entete;
        }

        internal string RequeterAvertissementEventuel()
        {
            if (HeuresCoursTotal > HeuresProfsMaxTheorique)
                return "Le nombre total d'heures de cours est supérieur au nombre total d'heures de service disponibles, "
                    + "y compris en mettant les heures supplémentaires au maximum pour tous les professeurs. "
                    + "Il est donc inutile de lancer la simulation, car elle ne pourra pas trouver de solution.";

            if (HeuresCoursTotal > HeuresProfsMaxVoulu)
                return "Le nombre total d'heures de cours est supérieur au nombre total d'heures de service disponibles, "
                    + "y compris en comptant les heures supplémentaires dans les limites demandées. Pour que la simulation trouve "
                    + "une solution, il est donc nécessaire d'étendre ces limites. Il manque "
                    + Convert.ToString(HeuresCoursTotal - HeuresProfsMaxVoulu)
                    + " heures.";

            return null;
        }
    }
}
