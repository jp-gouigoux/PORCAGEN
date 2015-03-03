using Microsoft.Win32;
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

namespace AssistantRepartitionClassesCollege
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Controls.Welcome ControleBienvenu = null;
        public Controls.Projet ControleProjet = null;
        public Controls.Classes ControleClasses = null;
        public Controls.Profs ControleProfs = null;
        public Controls.Preaffectations ControlePreaffectations = null;
        public Controls.PreferencesNiveaux ControlePreferencesNiveaux = null;
        public Controls.Criteres ControleCriteres = null;
        public Controls.Simuler ControleSimuler = null;
        public Controls.Options ControleOptions = null;

        ModeleProjet modele = null;

        string NomFichierModele = null;

        public MainWindow()
        {
            // Initialisation des contrôles graphiques du programme
            InitializeComponent();
            Contenu.Children.Add(ControleBienvenu = new Controls.Welcome());
            Contenu.Children.Add(ControleProjet = new Controls.Projet());
            Contenu.Children.Add(ControleClasses = new Controls.Classes());
            Contenu.Children.Add(ControleProfs = new Controls.Profs());
            Contenu.Children.Add(ControlePreaffectations = new Controls.Preaffectations());
            Contenu.Children.Add(ControlePreferencesNiveaux = new Controls.PreferencesNiveaux());
            Contenu.Children.Add(ControleCriteres = new Controls.Criteres());
            Contenu.Children.Add(ControleSimuler = new Controls.Simuler());
            Contenu.Children.Add(ControleOptions = new Controls.Options());

            // Lorsqu'on démarre le programme, le contrôle visible est celui qui affiche le message de bienvenue
            CacherControlesEnfants(Contenu);
            ControleBienvenu.Visibility = Visibility.Visible;

            // Sur la machine de développement, chargement automatique d'un projet pour tests
            if (Environment.MachineName == "ANTARES")
            {
                modele = new ModeleProjet();
                modele.Classes.Add(new Classe() { Nom = "6A", Niveau = Niveau.Sixième, Duree = 5, DureeSoutien = 0.5, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "6B", Niveau = Niveau.Sixième, Duree = 5, DureeSoutien = 0.5, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "6C", Niveau = Niveau.Sixième, Duree = 5, DureeSoutien = 0.5, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "6D", Niveau = Niveau.Sixième, Duree = 5, DureeSoutien = 0.5, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "6E", Niveau = Niveau.Sixième, Duree = 5, DureeSoutien = 0.5, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "5A", Niveau = Niveau.Cinquième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "5B", Niveau = Niveau.Cinquième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "5C", Niveau = Niveau.Cinquième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "5D", Niveau = Niveau.Cinquième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "5E", Niveau = Niveau.Cinquième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "5F", Niveau = Niveau.Cinquième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "4A", Niveau = Niveau.Quatrième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "4B", Niveau = Niveau.Quatrième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "4C", Niveau = Niveau.Quatrième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "4D", Niveau = Niveau.Quatrième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "4E", Niveau = Niveau.Quatrième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "4F", Niveau = Niveau.Quatrième, Duree = 4, AutoriserDecoupe = true });
                modele.Classes.Add(new Classe() { Nom = "3A", Niveau = Niveau.Troisième, Duree = 4.5, AutoriserDecoupe = false });
                modele.Classes.Add(new Classe() { Nom = "3B", Niveau = Niveau.Troisième, Duree = 4.5, AutoriserDecoupe = false });
                modele.Classes.Add(new Classe() { Nom = "3C", Niveau = Niveau.Troisième, Duree = 4.5, AutoriserDecoupe = false });
                modele.Classes.Add(new Classe() { Nom = "3D", Niveau = Niveau.Troisième, Duree = 4.5, AutoriserDecoupe = false });
                modele.Profs.Add(new Prof() { Nom = "Elizabeth", Service = 10, MaxHeuresSup = 1.5 });
                modele.Profs.Add(new Prof() { Nom = "Anne", Service = 18, MaxHeuresSup = 1.5 });
                modele.Profs.Add(new Prof() { Nom = "Guénaëlle", Service = 9, MaxHeuresSup = 0 });
                modele.Profs.Add(new Prof() { Nom = "Julie", Service = 15, MaxHeuresSup = 0 });
                modele.Profs.Add(new Prof() { Nom = "BMP18", Service = 18, MaxHeuresSup = 1.5 });
                modele.Profs.Add(new Prof() { Nom = "BMP8", Service = 8, MaxHeuresSup = 0 });
                modele.Profs.Add(new Prof() { Nom = "Morgane", Service = 14, MaxHeuresSup = 0 });
                modele.Preaffectations.Add(new Preaffectation() { Prof = "Morgane", Classe = "6A" });
                modele.Preaffectations.Add(new Preaffectation() { Prof = "Guénaëlle", Classe = "3A" });
                modele.PreferencesNiveaux.Add(new PreferenceNiveau() { Prof = "Morgane", Mode = Preference.NePeutPasAvoir, Niveau = Niveau.Troisième });
                modele.PreferencesNiveaux.Add(new PreferenceNiveau() { Prof = "Anne", Mode = Preference.NePreferePasAvoir, Niveau = Niveau.Quatrième });
                modele.Criteres.CritereNonDecoupageClasses = 40;
                modele.Criteres.CritereDecoupageSurHeurePleine = 25;
                modele.Criteres.CritereLimiterNiveauxParProf = 20;
                modele.Criteres.CriterePrivilegierMemeProfCoursEtSoutien = 14;
                modele.Criteres.CriterePriseEnComptePreferencesNiveaux = 1;
                modele.isDirty = false;
                BindControls();
            }
        }

        private void BindControls()
        {
            ControleClasses.DataContext = modele;
            ControleProfs.DataContext = modele;
            ControlePreaffectations.DataContext = modele;
            ControlePreferencesNiveaux.DataContext = modele;
            ControleCriteres.DataContext = modele;
            ControleSimuler.DataContext = modele;
            ControleOptions.DataContext = modele;
        }

        private void NewCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (modele != null)
            {
                CloseCommand_Executed(sender, e);
                if (!e.Handled) return;
            }

            modele = new ModeleProjet();
            modele.isDirty = true;
            BindControls();

            CacherControlesEnfants(Contenu);
            ControleProjet.Visibility = Visibility.Visible;
        }

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (modele != null && modele.isDirty)
            {
                MessageBoxResult resultat = MessageBox.Show("Le projet courant a été modifié. Voulez-vous sauvegarder ces changements avant d'en ouvrir un autre ? Cliquez sur Oui pour enregistrer le projet, Non pour créer le nouveau sans enregistrer le précédent, ou Annuler pour revenir au projet courant.", this.Title, MessageBoxButton.YesNoCancel);
                if (resultat == MessageBoxResult.Cancel) { e.Handled = true; return; }
                if (resultat == MessageBoxResult.Yes)
                {
                    SaveCommand_Executed(sender, e);
                    if (!e.Handled) return;
                }
            }

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Fichier Porcagen (*.porcagen)|*.porcagen"
            };

            if (dialog.ShowDialog() == true)
            {
                NomFichierModele = dialog.FileName;
                modele = ModeleProjet.Load(NomFichierModele);
                modele.isDirty = false;
                BindControls();

                CacherControlesEnfants(Contenu);
                ControleProjet.Visibility = Visibility.Visible;
            }
        }

        private void SaveCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null && modele.isDirty;
        }

        private void SaveCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (NomFichierModele == null) DisplaySaveAsDialog();
            if (NomFichierModele == null) return;
            modele.Save(NomFichierModele);
            modele.isDirty = false;
            Statut.Text = "Fichier enregistré";
            e.Handled = true;
        }

        private void SaveAsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SaveAsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            DisplaySaveAsDialog();
            if (NomFichierModele == null) return;
            modele.Save(NomFichierModele);
            modele.isDirty = false;
            Statut.Text = "Fichier enregistré";
            e.Handled = true;
        }

        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null;
        }

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (modele == null) return;
            if (modele.isDirty)
            {
                MessageBoxResult resultat = MessageBox.Show("Le projet courant a été modifié. Voulez-vous sauvegarder ces changements avant de le fermer ? Cliquez sur Oui pour enregistrer le projet, Non pour fermer sans enregistrer, ou Annuler pour revenir au projet.", this.Title, MessageBoxButton.YesNoCancel);
                if (resultat == MessageBoxResult.Cancel) return;
                if (resultat == MessageBoxResult.Yes)
                {
                    SaveCommand_Executed(sender, e);
                    if (!e.Handled) return;
                }
            }

            NomFichierModele = null;
            modele = null;
            Statut.Text = "Projet fermé";
            e.Handled = true;

            BindControls();

            CacherControlesEnfants(Contenu);
            ControleBienvenu.Visibility = Visibility.Visible;
        }

        private void OptionsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null;
        }

        private void OptionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CacherControlesEnfants(Contenu);
            ControleOptions.Visibility = Visibility.Visible;
        }

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (modele != null && modele.isDirty)
            {
                MessageBoxResult resultat = MessageBox.Show("Le projet courant a été modifié. Voulez-vous sauvegarder ces changements avant de quitter l'application ? Cliquez sur Oui pour enregistrer le projet, Non pour sortir sans enregistrer, ou Annuler pour revenir au projet.", this.Title, MessageBoxButton.YesNoCancel);
                if (resultat == MessageBoxResult.Cancel) return;
                if (resultat == MessageBoxResult.Yes)
                {
                    SaveCommand_Executed(sender, e);
                    if (!e.Handled) return;
                }
            }

            Close();
        }

        private void ClassesCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null;
        }

        private void ClassesCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CacherControlesEnfants(Contenu);
            ControleClasses.Visibility = Visibility.Visible;
            Statut.Text = "ENTREE pour ajouter une ligne, SUPPR pour effacer la ligne sélectionnée";
        }

        private void ProfsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null;
        }

        private void ProfsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CacherControlesEnfants(Contenu);
            ControleProfs.Visibility = Visibility.Visible;
            Statut.Text = "ENTREE pour ajouter une ligne, SUPPR pour effacer la ligne sélectionnée";
        }

        private void PreaffectationsCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null;
        }

        private void PreaffectationsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CacherControlesEnfants(Contenu);
            ControlePreaffectations.Visibility = Visibility.Visible;
            Statut.Text = "ENTREE pour ajouter une ligne, SUPPR pour effacer la ligne sélectionnée";
        }

        private void PreferencesNiveauxCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null;
        }

        private void PreferencesNiveauxCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CacherControlesEnfants(Contenu);
            ControlePreferencesNiveaux.Visibility = Visibility.Visible;
            Statut.Text = "ENTREE pour ajouter une ligne, SUPPR pour effacer la ligne sélectionnée";
        }

        private void CriteresCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null;
        }

        private void CriteresCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CacherControlesEnfants(Contenu);
            ControleCriteres.Visibility = Visibility.Visible;
            Statut.Text = "ENTREE pour ajouter une ligne, SUPPR pour effacer la ligne sélectionnée";
        }

        private void SimulerCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = modele != null;
        }

        private void SimulerCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            CacherControlesEnfants(Contenu);
            ControleSimuler.Visibility = Visibility.Visible;
            Statut.Text = "Une fois l'opération d'optimisation terminée, vous pourrez enregistrer les résultats";
        }

        public void DisplaySaveAsDialog()
        {
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Fichier Porcagen (*.porcagen)|*.porcagen"
            };

            if (dialog.ShowDialog() == true)
                NomFichierModele = dialog.FileName;
        }

        private void CacherControlesEnfants(Panel parent)
        {
            foreach (Control c in parent.Children)
                c.Visibility = Visibility.Hidden;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (modele != null && modele.isDirty)
            {
                MessageBoxResult resultat = MessageBox.Show("Le projet courant a été modifié. Voulez-vous sauvegarder ces changements avant de quitter l'application ? Cliquez sur Oui pour enregistrer le projet, Non pour sortir sans enregistrer, ou Annuler pour revenir au projet.", this.Title, MessageBoxButton.YesNoCancel);
                if (resultat == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (resultat == MessageBoxResult.Yes)
                {
                    DisplaySaveAsDialog();
                    if (NomFichierModele == null)
                    {
                        e.Cancel = true;
                        return;
                    }
                    modele.Save(NomFichierModele);
                }
            }
        }
    }

    public static class CustomCommands
    {
        public static readonly RoutedUICommand Classes = new RoutedUICommand(
            "Classes",
            "Classes",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.D1, ModifierKeys.Alt) });

        public static readonly RoutedUICommand Profs = new RoutedUICommand(
            "Profs",
            "Profs",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.D2, ModifierKeys.Alt) });

        public static readonly RoutedUICommand Preaffectations = new RoutedUICommand(
            "Preaffectations",
            "Preaffectations",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.D3, ModifierKeys.Alt) });

        public static readonly RoutedUICommand PreferencesNiveaux = new RoutedUICommand(
            "PreferencesNiveaux",
            "PreferencesNiveaux",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.D4, ModifierKeys.Alt) });

        public static readonly RoutedUICommand Criteres = new RoutedUICommand(
            "Criteres",
            "Criteres",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.D5, ModifierKeys.Alt) });

        public static readonly RoutedUICommand Simuler = new RoutedUICommand(
            "Simuler",
            "Simuler",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.S, ModifierKeys.Alt) });

        public static readonly RoutedUICommand Options = new RoutedUICommand(
            "Options",
            "Options",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.O, ModifierKeys.Alt) });

        public static readonly RoutedUICommand Exit = new RoutedUICommand(
            "Exit",
            "Exit",
            typeof(CustomCommands),
            new InputGestureCollection() { new KeyGesture(Key.F4, ModifierKeys.Alt) });
    }
}
