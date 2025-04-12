using System;
using System.Diagnostics;

namespace CyberCrypt.Models
{
    public static class ConvertToShellcode
    {
        public static void GenerateShellcode(string exePath, string outputShellcodePath)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c donut -i \"{exePath}\" -o \"{outputShellcodePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process? process = Process.Start(psi);  // Use nullable Process type

            if (process == null)
            {
                throw new Exception("Failed to start the process.");
            }

            process.WaitForExit();

            string output = process.StandardOutput?.ReadToEnd() ?? "";  
            string error = process.StandardError?.ReadToEnd() ?? "";

            if (process.ExitCode != 0)
            {
                throw new Exception($"Donut failed or file missing. Error: {error}");
            }
        }
    }
}
