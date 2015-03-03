using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AssistantRepartitionClassesCollege.Controls
{
    /// <summary>
    /// Logique d'interaction pour Options.xaml
    /// </summary>
    public partial class Options : UserControl
    {
        public Options()
        {
            InitializeComponent();
        }

        // Penser à supprimer ce code quand on aura implémenté le isDirty de manière complète
        private void TaillePopulation_TextChanged(object sender, TextChangedEventArgs e)
        {
            //ModeleProjet modele = DataContext as ModeleProjet;
            //if (modele != null) modele.isDirty = true;
        }

        private void NombreIterations_TextChanged(object sender, TextChangedEventArgs e)
        {
            //ModeleProjet modele = DataContext as ModeleProjet;
            //if (modele != null) modele.isDirty = true;
        }
    }
}
