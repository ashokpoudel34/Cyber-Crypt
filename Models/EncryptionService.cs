using System;
using System.IO;
using System.Security.Cryptography;
using CyberCrypt.Models;

namespace CyberCrypt.Models
{
    public static class EncryptionService
    {
        public static void EncryptFile(string inputPath, string outputPath, string keyPath)
        {
                // Step 1: Convert user-given .exe to raw shellcode
            string shellcodePath = Path.Combine(Path.GetDirectoryName(outputPath) ?? ".", "temp_shellcode.bin");
            ConvertToShellcode.GenerateShellcode(inputPath, shellcodePath);
            byte[] plaintextBytes = File.ReadAllBytes(shellcodePath);
            byte[] key = new byte[16];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(key);
            }

            // Add PKCS7 padding
            int paddingSize = 16 - (plaintextBytes.Length % 16);
            byte[] paddedData = new byte[plaintextBytes.Length + paddingSize];
            Array.Copy(plaintextBytes, paddedData, plaintextBytes.Length);
            for (int i = 0; i < paddingSize; i++)
            {
                paddedData[plaintextBytes.Length + i] = (byte)paddingSize;
            }

            // Encrypt with AES
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.None;

                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    byte[] encryptedBytes = encryptor.TransformFinalBlock(paddedData, 0, paddedData.Length);
                    File.WriteAllBytes(outputPath, encryptedBytes);
                }
            }

            File.WriteAllBytes(keyPath, key);
            File.Delete(shellcodePath);
        }

    public static void DecryptFile(string encryptedFilePath, string keyFilePath, string outputFile)
{
    // Read encrypted file and key
    byte[] encryptedBytes = File.ReadAllBytes(encryptedFilePath);
    byte[] key = File.ReadAllBytes(keyFilePath);

    // Validate key length
    if (key.Length != 16)
    {
        throw new ArgumentException("Invalid key length. Key must be 16 bytes (128-bit) for AES-128.");
    }

    // Decrypt with AES
    byte[] decryptedPaddedData;
    using (Aes aes = Aes.Create())
    {
        aes.Key = key;
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        using (ICryptoTransform decryptor = aes.CreateDecryptor())
        {
            decryptedPaddedData = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
        }
    }

    // Remove PKCS7 padding
    int paddingSize = decryptedPaddedData[decryptedPaddedData.Length - 1];
    if (paddingSize > 16 || paddingSize < 1)
    {
        throw new CryptographicException("Invalid padding detected. The file may be corrupted or the key is incorrect.");
    }

    // Verify padding
    for (int i = decryptedPaddedData.Length - paddingSize; i < decryptedPaddedData.Length; i++)
    {
        if (decryptedPaddedData[i] != paddingSize)
        {
            throw new CryptographicException("Invalid padding detected. The file may be corrupted or the key is incorrect.");
        }
    }

    // Extract original data
    byte[] originalData = new byte[decryptedPaddedData.Length - paddingSize];
    Array.Copy(decryptedPaddedData, originalData, originalData.Length);

    // Write decrypted file
    File.WriteAllBytes(outputFile, originalData);
}
    }
}
