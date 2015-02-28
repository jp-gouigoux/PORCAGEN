using AForge.Genetic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
    /// Logique d'interaction pour Simuler.xaml
    /// </summary>
    public partial class Simuler : UserControl
    {
        public Simuler()
        {
            InitializeComponent();
            Annuler.IsEnabled = false; // Plutôt que de manipuler en direct, ça serait bien de faire un binding sur moteur.IsBusy - cf. http://stackoverflow.com/questions/14143683/disable-wpf-buttons-during-longer-running-process-the-mvvm-way
        }

        BackgroundWorker moteur = null;

        private void Lancer_Click(object sender, RoutedEventArgs e)
        {
            ModeleProjet modele = DataContext as ModeleProjet;

            Suivi.Inlines.Clear();
            Suivi.Inlines.Add(new Run(modele.AfficherEntetePreCalcul()));
            Suivi.Inlines.Add(new LineBreak());
            Suivi.Inlines.Add(new LineBreak());

            string avertissement = modele.RequeterAvertissementEventuel();
            if (avertissement != null)
            {
                MessageBox.Show(avertissement, (Application.Current.MainWindow as MainWindow).Title, MessageBoxButton.OK);
                return;
            }

            (Application.Current.MainWindow as MainWindow).Statut.Text = "Simulation en cours...";
            Lancer.IsEnabled = false;
            Annuler.IsEnabled = true;

            moteur = new BackgroundWorker();
            moteur.DoWork += moteur_DoWork;
            moteur.WorkerReportsProgress = true;
            moteur.WorkerSupportsCancellation = true;
            moteur.RunWorkerCompleted += moteur_RunWorkerCompleted;
            moteur.ProgressChanged += moteur_ProgressChanged;
            moteur.RunWorkerAsync(modele);
        }

        void moteur_DoWork(object sender, DoWorkEventArgs e)
        {
            ModeleProjet modele = e.Argument as ModeleProjet;
            BackgroundWorker moteur = sender as BackgroundWorker;
            FitnessRepartitionClasses fitness = new FitnessRepartitionClasses(modele);
            IChromosome IndividuRacine = new ShortArrayChromosome(fitness.NombreGenesNecessaires);
            Population Population = new Population(modele.TaillePopulation, IndividuRacine, fitness, new RouletteEliteSelection());

            int Iteration = 0;
            ShortArrayChromosome Meilleur = null;
            while (Iteration++ < modele.NombreIterations)
            {
                Population.RunEpoch();
                Meilleur = Population.BestChromosome as ShortArrayChromosome;
                if (moteur.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                moteur.ReportProgress(100 * Iteration / modele.NombreIterations, Meilleur.Fitness);
            }

            e.Result = Tuple.Create(Meilleur, fitness);
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            if (moteur != null)
                moteur.CancelAsync();
            Lancer.IsEnabled = true;
            Annuler.IsEnabled = false;
        }

        void moteur_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            double fitness = 100 * (double)e.UserState;
            (Application.Current.MainWindow as MainWindow).Statut.Text = "Valeur de la solution trouvée : " + fitness.ToString("F1") + "%";
            (Application.Current.MainWindow as MainWindow).Progres.Value = e.ProgressPercentage;
        }

        void moteur_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                (Application.Current.MainWindow as MainWindow).Statut.Text = "Simulation annulée";
            }
            else if (e.Error != null)
            {
                Suivi.Inlines.Add(e.Error.ToString());
                (Application.Current.MainWindow as MainWindow).Statut.Text = "Simulation en erreur";
            }
            else if (e.Result != null)
            {
                StringWriter scribe = new StringWriter();
                Tuple<ShortArrayChromosome, FitnessRepartitionClasses> resultat = e.Result as Tuple<ShortArrayChromosome, FitnessRepartitionClasses>;
                double fitValue = resultat.Item2.Evaluate(resultat.Item1, scribe);
                Suivi.Inlines.Add(scribe.ToString());
                Suivi.Inlines.Add("fitness = " + fitValue);
                (Application.Current.MainWindow as MainWindow).Statut.Text = "Simulation terminée";
            }
            Lancer.IsEnabled = true;
            Annuler.IsEnabled = false;
        }
    }
}
