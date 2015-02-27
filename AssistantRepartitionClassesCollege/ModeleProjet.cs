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
        }

        public class Prof
        {
            [XmlAttribute]
            public string Nom { get; set; }
            [XmlAttribute]
            public double Service { get; set; }
            [XmlAttribute]
            public double MaxHeuresSup { get; set; }
        }

        public class Preaffectation
        {
            [XmlAttribute]
            public string Classe { get; set; }
            [XmlAttribute]
            public string Prof { get; set; }
        }

        public class PreferenceNiveau
        {
            [XmlAttribute]
            public string Prof { get; set; }
            [XmlAttribute]
            public Preference Mode { get; set; }
            [XmlAttribute]
            public Niveau Niveau { get; set; }
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

        public ModeleProjet()
        {
            Classes = new List<Classe>();
            Profs = new List<Prof>();
            Preaffectations = new List<Preaffectation>();
            PreferencesNiveaux = new List<PreferenceNiveau>();
            Criteres = new CriteresCalcul();

            TaillePopulation = 1000;
            NombreIterations = 100;
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
    }
}
