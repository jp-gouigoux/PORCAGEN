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
    public class Prof : INotifyPropertyChanged
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

        private double _Service = 0.0;

        [XmlAttribute]
        public double Service 
        {
            get { return _Service; }
            set { if (value != _Service) { _Service = value; NotifyPropertyChanged(); } }
        }

        private double _MaxHeuresSup = 0.0;

        [XmlAttribute]
        public double MaxHeuresSup
        {
            get { return _MaxHeuresSup; }
            set { if (value != _MaxHeuresSup) { _MaxHeuresSup = value; NotifyPropertyChanged(); } }
        }

        public override string ToString()
        {
            return string.Concat(Nom, " : ", Service.ToString("F1"), "h+", MaxHeuresSup.ToString("F1") + "h");
        }
    }
}
