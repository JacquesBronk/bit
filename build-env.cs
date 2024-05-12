using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

public class buildenv
{
    private static readonly string ServicesSecretsBasePath = "./config";
    private const string InfoColor = "\u001b[32m";
    private const string WarningColor = "\u001b[33m";
    private const string ErrorColor = "\u001b[31m";
    private const string ResetColor = "\u001b[0m";

    public static void Main()
    {
        try
        {
            List<string> services = GetDockerComposeServices();
            if (services.Count == 0)
            {
                WriteColoredMessage("[WARNING] No services found in the Docker Compose file.", WarningColor);
                return;
            }

            foreach (var service in services)
            {
                CreateServiceSecrets(service);
            }
        }
        catch (Exception ex)
        {
            WriteColoredMessage($"[ERROR] {ex.Message}", ErrorColor);
        }
    }

    private static List<string> GetDockerComposeServices()
    {
        string fileName;
        string arguments;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            fileName = "cmd.exe";
            arguments = "/C docker-compose config --services";
        }
        else
        {
            fileName = GetBashPath();
            arguments = "-c \"docker-compose config --services\"";
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return string.IsNullOrWhiteSpace(result)
            ? new List<string>()
            : new List<string>(result.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
    }

    private static void CreateServiceSecrets(string service)
    {
        var serviceSecretsDir = Path.Combine(ServicesSecretsBasePath, service, "secrets");
        Directory.CreateDirectory(serviceSecretsDir);
        var password = GenerateSecurePassword();
        var secretFilePath = Path.Combine(serviceSecretsDir, "default-password");
        File.WriteAllText(secretFilePath, password);
        WriteColoredMessage($"[INFO] Created secrets for service '{service}' at '{secretFilePath}'", InfoColor);
    }

    private static string GenerateSecurePassword(int length = 16)
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Substring(0, length);
        }
    }

    private static string GetBashPath()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = "-c \"command -v bash\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd().Trim();
        process.WaitForExit();
        return string.IsNullOrWhiteSpace(result) ? "/bin/bash" : result;
    }

    private static void WriteColoredMessage(string message, string color)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Console.WriteLine(message);
        }
        else
        {
            Console.WriteLine($"{color}{message}{ResetColor}");
        }
    }
}