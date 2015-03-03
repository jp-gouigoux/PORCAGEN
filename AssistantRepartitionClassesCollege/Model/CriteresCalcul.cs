using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AssistantRepartitionClassesCollege
{
    public class CriteresCalcul : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _CritereNonDecoupageClasses = 40;

        [XmlAttribute]
        public int CritereNonDecoupageClasses
        {
            get { return _CritereNonDecoupageClasses; }
            set { if (value != _CritereNonDecoupageClasses) { value = _CritereNonDecoupageClasses; NotifyPropertyChanged(); } }
        }

        private int _CritereDecoupageSurHeurePleine = 25;

        [XmlAttribute]
        public int CritereDecoupageSurHeurePleine
        {
            get { return _CritereDecoupageSurHeurePleine; }
            set { if (value != _CritereDecoupageSurHeurePleine) { value = _CritereDecoupageSurHeurePleine; NotifyPropertyChanged(); } }
        }

        private int _CritereLimiterNiveauxParProf = 20;

        [XmlAttribute]
        public int CritereLimiterNiveauxParProf
        {
            get { return _CritereLimiterNiveauxParProf; }
            set { if (value != _CritereLimiterNiveauxParProf) { value = _CritereLimiterNiveauxParProf; NotifyPropertyChanged(); } }
        }

        private int _CriterePrivilegierMemeProfCoursEtSoutien = 14;

        [XmlAttribute]
        public int CriterePrivilegierMemeProfCoursEtSoutien
        {
            get { return _CriterePrivilegierMemeProfCoursEtSoutien; }
            set { if (value != _CriterePrivilegierMemeProfCoursEtSoutien) { value = _CriterePrivilegierMemeProfCoursEtSoutien; NotifyPropertyChanged(); } }
        }

        private int _CriterePriseEnComptePreferencesNiveaux = 1;

        [XmlAttribute]
        public int CriterePriseEnComptePreferencesNiveaux
        {
            get { return _CriterePriseEnComptePreferencesNiveaux; }
            set { if (value != _CriterePriseEnComptePreferencesNiveaux) { value = _CriterePriseEnComptePreferencesNiveaux; NotifyPropertyChanged(); } }
        }
    }
}
