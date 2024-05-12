using Microsoft.Extensions.Configuration;

namespace Bit.Lib.Infra.Docker;

public class DockerSecretsConfigurationSource : IConfigurationSource
{
    public string SecretsPath { get; init; } = "/run/secrets";

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DockerSecretsConfigurationProvider(SecretsPath);
    }
}