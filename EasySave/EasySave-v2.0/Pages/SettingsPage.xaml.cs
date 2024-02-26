using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using EasySave_v2._0.ViewModels;
using System.Windows.Controls;

namespace EasySave_v2._0.Pages
{
    public partial class SettingsPage : Page
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsPage()
        {
            InitializeComponent();
            _viewModel = new SettingsViewModel();
            DataContext = _viewModel;

            // Définir la langue par défaut
            LanguageComboBox.SelectedIndex = 0;
            // Définir le type de log par défaut
            LogTypeComboBox.SelectedIndex = 0;
        }

        private void RestoreButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ResetConfig();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
                _viewModel.SaveConfig();
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
                string folderPath = Path.GetDirectoryName(folderDialog.FileName);

                if (sender == DailyLogPathBrowseButton)
                {
                    _viewModel.DailyLogPath = folderPath;
                }
                if (sender == StateLogPathBrowseButton)
                {
                    _viewModel.StateLogPath = folderPath;
                }
                if (sender == ConfigFilePathBrowseButton)
                {
                    _viewModel.ConfigFilePath = folderPath;
                }
            }
        }
    }
}
