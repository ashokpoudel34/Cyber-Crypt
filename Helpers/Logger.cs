using System;
using System.IO;

namespace CyberCrypt.Helpers
{
    public static class Logger
    {
        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "CyberCrypt_Errors.log");

        private static readonly string WarningLogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "CyberCrypt_Warnings.log");

        public static void LogError(Exception ex, string? context = null)
        {
            try
            {
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                                  $"Context: {context ?? "No context"}\n" +
                                  $"Error: {ex.Message}\n" +
                                  $"Stack Trace:\n{ex.StackTrace}\n" +
                                  "----------------------------------------\n";

                File.AppendAllText(LogPath, logMessage);
            }
            catch { /* Prevent logging failures from crashing app */ }
        }

        public static void LogWarning(string warningMessage, string? context = null)
        {
            try
            {
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                                  $"Context: {context ?? "No context"}\n" +
                                  $"Warning: {warningMessage}\n" +
                                  "----------------------------------------\n";

                File.AppendAllText(WarningLogPath, logMessage);
            }
            catch { /* Prevent logging failures from crashing app */ }
        }

        public static void LogInformation(string infoMessage, string? context = null)
        {
            try
            {
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                                  $"Context: {context ?? "No context"}\n" +
                                  $"Info: {infoMessage}\n" +
                                  "----------------------------------------\n";

                File.AppendAllText(LogPath, logMessage); // Or create separate info log
            }
            catch { /* Prevent logging failures from crashing app */ }
        }
    }
}