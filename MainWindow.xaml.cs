using System;
using System.Windows;
using System.IO;
using System.Threading.Tasks;
using CyberCrypt.ViewModels;

namespace CyberCrypt
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e) => _viewModel.BrowseFile();

        private void BrowseSaveButton_Click(object sender, RoutedEventArgs e) => _viewModel.ChooseSaveLocation();

        private async void EncryptButton_Click(object sender, RoutedEventArgs e) => await _viewModel.EncryptFileAsync();


        private void BrowseEncryptedFile_Click(object sender, RoutedEventArgs e) => _viewModel.BrowseEncryptedFile();

        private void BrowseKeyFile_Click(object sender, RoutedEventArgs e) => _viewModel.BrowseKeyFile();

        private async void DecryptButton_Click(object sender, RoutedEventArgs e) => await _viewModel.DecryptFileAsync();
        

        private async void InjectButton_Click(object sender, RoutedEventArgs e) => await _viewModel.BuildInjectorAsync();

    }
}
