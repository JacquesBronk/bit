using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Bit.Lib.Infra.Os;

public static class OsInterop
{
    public static string ExecuteCommand(string command)
    {
        Console.WriteLine($"Executing command: {command}");
        var processStartInfo = new ProcessStartInfo();

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.Arguments = $"/c {command}";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                 RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            processStartInfo.FileName = "/bin/bash";
            processStartInfo.Arguments = $"-c \"{command}\"";
        }

        processStartInfo.RedirectStandardOutput = true;
        processStartInfo.RedirectStandardError = true;

        var process = Process.Start(processStartInfo);
        var output = new StringBuilder();

        if (process == null) return output.ToString();
        
        process.OutputDataReceived += (_, args) => output.AppendLine(args.Data);
        process.ErrorDataReceived += (_, args) => output.AppendLine(args.Data);

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception(output.ToString());
        }

        return output.ToString();
    }
}