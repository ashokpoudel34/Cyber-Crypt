using Microsoft.Win32;
using System;
using System.IO;

namespace CyberCrypt.Helpers
{
    public static class FileHelper
    {
        public static string BrowseForFile()
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select File to Encrypt",
                Filter = "All files (*.*)|*.*"
            };

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : string.Empty;
        }

        public static string ChooseSaveLocation( )
        {
            var saveFileDialog = new SaveFileDialog
            {
                Title = "Select Save Location",
                Filter = "Encrypted Files (*.bin)|*.bin",
                FileName = $"encrypted_{DateTime.Now:yyyyMMdd}.bin"
            };

            return saveFileDialog.ShowDialog() == true ? Path.GetDirectoryName(saveFileDialog.FileName) ?? string.Empty : string.Empty;

        }
    }
}
