﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;

public class BuildEnv
{
    private static readonly string ServicesSecretsBasePath = "./config";
    private const string InfoColor = "\u001b[32m";
    private const string WarningColor = "\u001b[33m";
    private const string ErrorColor = "\u001b[31m";
    private const string ResetColor = "\u001b[0m";

    public static void Main(string[] args)
    {
        try
        {
            if (args.Length < 1)
            {
                WriteColoredMessage("[ERROR] Usage: dotnet run <path-to-appsettings.json> [--env-renew]", ErrorColor);
                return;
            }

            string appSettingsPath = args[0];
            bool envRenew = args.Length > 1 && args[1] == "--env-renew";

            if (envRenew)
            {
                GenerateEnvFile(appSettingsPath);
                return;
            }

            GenerateEnvFile(appSettingsPath);

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
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes).Substring(0, length);
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

    private static void GenerateEnvFile(string appSettingsPath)
    {
        string envFilePath = Path.Combine(Directory.GetCurrentDirectory(), ".env");

        try
        {
            if (!File.Exists(appSettingsPath))
            {
                WriteColoredMessage($"[ERROR] File not found: {appSettingsPath}", ErrorColor);
                return;
            }

            var jsonContent = File.ReadAllText(appSettingsPath);
            var jsonObject = JsonDocument.Parse(jsonContent).RootElement;

            var envContent = GenerateEnvContent(jsonObject, string.Empty);
            File.WriteAllText(envFilePath, envContent);

            WriteColoredMessage($"[INFO] Environment variables written to: {envFilePath}", InfoColor);
        }
        catch (Exception ex)
        {
            WriteColoredMessage($"[ERROR] An error occurred while generating the .env file: {ex.Message}", ErrorColor);
        }
    }

    private static string GenerateEnvContent(JsonElement jsonObject, string prefix)
    {
        var envVariables = new List<string>();

        foreach (var property in jsonObject.EnumerateObject())
        {
            envVariables.AddRange(ProcessProperty(property, prefix));
        }

        return string.Join(Environment.NewLine, envVariables);
    }

    private static IEnumerable<string> ProcessProperty(JsonProperty prop, string prefix)
    {
        string key = string.IsNullOrEmpty(prefix) ? prop.Name : $"{prefix}__{prop.Name}";

        if (prop.Value.ValueKind == JsonValueKind.Object)
        {
            return GenerateEnvContent(prop.Value, key);
        }
        else if (prop.Value.ValueKind == JsonValueKind.Array)
        {
            var arrayItems = new List<string>();
            int index = 0;
            foreach (var item in prop.Value.EnumerateArray())
            {
                arrayItems.AddRange(ProcessArrayItem(item, $"{key}__{index}"));
                index++;
            }
            return arrayItems;
        }
        else
        {
            return new List<string> { $"{key}={prop.Value.ToString()}" };
        }
    }

    private static IEnumerable<string> ProcessArrayItem(JsonElement item, string key)
    {
        if (item.ValueKind == JsonValueKind.Object)
        {
            return GenerateEnvContent(item, key);
        }
        else
        {
            return new List<string> { $"{key}={item.ToString()}" };
        }
    }
}
