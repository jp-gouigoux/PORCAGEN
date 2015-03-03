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
    public class PreferenceNiveau : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Prof = string.Empty;

        [XmlAttribute]
        public string Prof 
        {
            get { return _Prof; }
            set { if (value != _Prof) { _Prof = value; NotifyPropertyChanged(); } }
        }

        private Preference _Mode = Preference.NePeutPasAvoir;

        [XmlAttribute]
        public Preference Mode 
        {
            get { return _Mode; }
            set { if (value != _Mode) { _Mode = value; NotifyPropertyChanged(); } }
        }

        private Niveau _Niveau = Niveau.Sixième;

        [XmlAttribute]
        public Niveau Niveau 
        {
            get { return _Niveau; }
            set { if (value != _Niveau) { _Niveau = value; NotifyPropertyChanged(); } }
        }

        public override string ToString()
        {
            return string.Concat(Prof, " ", Mode, " ", Niveau);
        }
    }
}
