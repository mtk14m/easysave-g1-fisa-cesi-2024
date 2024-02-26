using EasySave_v2._0.Models;
using EasySave_v2._0.ViewModels;
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

namespace EasySave_v2._0.Pages
{
    public partial class AddBackupJobPage : Page
    {
        private BackupListViewModel viewmodel;
        public AddBackupJobPage()
        {
            InitializeComponent();
            viewmodel = new BackupListViewModel();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Récupérer les valeurs des champs
            string name = Name.Text;
            string source = Source.Text;
            string destination = Destination.Text;
            string type = ((ComboBoxItem)Type.SelectedItem)?.Content.ToString();

            // Vérifier si tous les champs sont remplis
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(destination) || string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Veuillez remplir tous les champs avant de sauvegarder.");
                return;
            }

            // Appeler la méthode du ViewModel pour créer et sauvegarder le nouveau travail de sauvegarde
            viewmodel.CreateAndSaveBackupJob(name, source, destination, type);

            // Afficher un message de confirmation
            MessageBox.Show("Nouveau travail de sauvegarde enregistré avec succès.");
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new OpenFileDialog
            {
                Title = "Select a folder",
                CheckFileExists = false,
                FileName = "Select Folder",
                Filter = "Folders|no.files"
            };

            if (folderDialog.ShowDialog() == true)
            {
                string folderPath = System.IO.Path.GetDirectoryName(folderDialog.FileName);

                if (sender == SourceBrowserButton)
                {
                    Source.Text = folderPath;
                }
                else if (sender == TargetBrowserButton)
                {
                    Destination.Text = folderPath;
                }
            }
        }
    }
}

