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
    public class Preaffectation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _Classe = string.Empty;

        [XmlAttribute]
        public string Classe
        {
            get { return _Classe; }
            set { if (value != _Classe) { _Classe = value; NotifyPropertyChanged(); } }
        }

        private string _Prof = string.Empty;

        [XmlAttribute]
        public string Prof
        {
            get { return _Prof; }
            set { if (value != _Prof) { _Prof = value; NotifyPropertyChanged(); } }
        }

        public override string ToString()
        {
            return string.Concat(Classe, " => ", Prof);
        }
    }
}
