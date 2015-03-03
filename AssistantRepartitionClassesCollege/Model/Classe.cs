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
    public class Classe : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Nom = string.Empty;

        [XmlAttribute]
        public string Nom
        {
            get { return _Nom; }
            set { if (value != _Nom) { _Nom = value; NotifyPropertyChanged(); } }
        }

        private Niveau _Niveau = Niveau.Sixième;

        [XmlAttribute]
        public Niveau Niveau
        {
            get { return _Niveau; }
            set { if (value != _Niveau) { _Niveau = value; NotifyPropertyChanged(); } } 
        }

        private double _Duree = 0.0;

        [XmlAttribute]
        public double Duree
        {
            get { return _Duree; }
            set { if (value != _Duree) { _Duree = value; NotifyPropertyChanged(); } }
        }

        private double _DureeSoutien = 0.0;

        [XmlAttribute]
        public double DureeSoutien
        {
            get { return _DureeSoutien; }
            set { if (value != _DureeSoutien) { _DureeSoutien = value; NotifyPropertyChanged(); } } 
        }

        private bool _AutoriserDecoupe = true;

        [XmlAttribute]
        public bool AutoriserDecoupe
        {
            get { return _AutoriserDecoupe; }
            set { if (value != _AutoriserDecoupe) { _AutoriserDecoupe = value; NotifyPropertyChanged(); } } 
        }

        public override string ToString()
        {
            return string.Concat(Nom, " : ", Duree.ToString("F1"), "h", DureeSoutien > 0 ? ("+" + DureeSoutien.ToString("F1") + "h") : string.Empty);
        }
    }
}
