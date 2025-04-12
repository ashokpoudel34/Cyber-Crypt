using CyberCrypt.Helpers;
using CyberCrypt.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace CyberCrypt.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _filePath = string.Empty;
        private string _savePath = string.Empty;
        private string _encryptedFilePath = string.Empty;
        private string _keyFilePath = string.Empty;
        private string _status = "Ready"; 

        public event PropertyChangedEventHandler? PropertyChanged;

        private string _processName = string.Empty;
        public string ProcessName
        {
            get => _processName;
            set
            {
                _processName = value;
                OnPropertyChanged();
            }
        }

        // Bindable Properties
        public string FilePath
        {
            get => _filePath;
            set { _filePath = value; OnPropertyChanged(); }
        }

        public string SavePath
        {
            get => _savePath;
            set { _savePath = value; OnPropertyChanged(); }
        }

        public string EncryptedFilePath
        {
            get => _encryptedFilePath;
            set { _encryptedFilePath = value; OnPropertyChanged(); }
        }

        public string KeyFilePath
        {
            get => _keyFilePath;
            set { _keyFilePath = value; OnPropertyChanged(); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        // File Selection Methods
        public void BrowseFile() => FilePath = FileHelper.BrowseForFile();
        public void ChooseSaveLocation() => SavePath = FileHelper.ChooseSaveLocation();
        public void BrowseEncryptedFile() => EncryptedFilePath = FileHelper.BrowseForFile();
        public void BrowseKeyFile() => KeyFilePath = FileHelper.BrowseForFile();


        // Encryption Method
        public async Task EncryptFileAsync()
        {
            if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
            {
                Status = "Error: No valid file selected!";
                return;
            }

            if (string.IsNullOrWhiteSpace(SavePath))
            {
                SavePath = Environment.CurrentDirectory;
            }

            string originalFileName = Path.GetFileNameWithoutExtension(FilePath);
            string outputFile = Path.Combine(SavePath, $"encrypted_{originalFileName}.bin");
            string keyFile = Path.Combine(SavePath, $"key_{originalFileName}.txt");

            try
            {
                Status = "Encrypting...";
                await Task.Run(() => EncryptionService.EncryptFile(FilePath, outputFile, keyFile));
                Status = $"Encryption complete! Files saved to {outputFile}";
                MessageBox.Show($"File encrypted successfully!\nEncrypted file: {outputFile}\nKey file: {keyFile}", 
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Status = "Encryption failed!";
                MessageBox.Show($"Encryption failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Decryption Method
        public async Task DecryptFileAsync()
        {
            if (string.IsNullOrWhiteSpace(EncryptedFilePath) || !File.Exists(EncryptedFilePath) ||
                string.IsNullOrWhiteSpace(KeyFilePath) || !File.Exists(KeyFilePath))
            {
                Status = "Error: Select encrypted file and key!";
                return;
            }

            if (string.IsNullOrWhiteSpace(SavePath))
            {
                SavePath = Environment.CurrentDirectory;
            }

            string originalFileName = Path.GetFileNameWithoutExtension(EncryptedFilePath).Replace("encrypted_", "");
            string outputFile = Path.Combine(SavePath, $"decrypted_{originalFileName}.exe");

            try
            {
                Status = "Decrypting...";
                await Task.Run(() => EncryptionService.DecryptFile(EncryptedFilePath, KeyFilePath, outputFile));
                Status = $"Decryption complete! Files saved to {outputFile}";
                MessageBox.Show($"File decrypted successfully!\nDecrypted file: {outputFile}",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Status = "Decryption failed!";
                MessageBox.Show($"Decryption failed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
private readonly InjectorBuilderService _builderService = new InjectorBuilderService();

public async Task BuildInjectorAsync()
{
    if (string.IsNullOrWhiteSpace(ProcessName) || 
        string.IsNullOrWhiteSpace(SavePath) ||
        !File.Exists(EncryptedFilePath) || 
        !File.Exists(KeyFilePath))
    {
        Status = "Error: Missing required files/parameters";
        return;
    }
string originalFileName = Path.GetFileNameWithoutExtension(EncryptedFilePath).Replace("encrypted_", "");
    try
    {
        Status = "Building injector...";
        await Task.Run(() => 
        {
            string outputExe = Path.Combine(SavePath, $"crypted_{originalFileName}.exe");
            _builderService.BuildInjectorExe(
                EncryptedFilePath,
                KeyFilePath,
                ProcessName,
                outputExe);
        });
        
        Status = $"Injector built successfully at {SavePath}";
    }
       catch (Exception ex)
    {
        string errorContext = $"Failed to build injector. " +
                            $"Process: {ProcessName}, " +
                            $"SavePath: {SavePath}";
        
        // Update UI
        Status = "Build failed - See desktop log file";
        
        // Log detailed error
        Logger.LogError(ex, errorContext);
        
        // Optional: Show brief notification
        MessageBox.Show("An error occurred. Details saved to desktop log.",
                      "Error",
                      MessageBoxButton.OK,
                      MessageBoxImage.Error);
    }
}



        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
