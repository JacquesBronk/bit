using Microsoft.Extensions.Configuration;

namespace Bit.Lib.Infra.Docker;

public class DockerSecretsConfigurationProvider(string secretsPath) : ConfigurationProvider
{
    private readonly string _secretsPath = secretsPath;

    public override void Load()
    {
        if (!Directory.Exists(_secretsPath)) return;

        Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var file in Directory.GetFiles(_secretsPath))
        {
            var key = Path.GetFileName(file);
            var value = File.ReadAllText(file).Trim();
            Data.Add(key, value);
        }
    }
}