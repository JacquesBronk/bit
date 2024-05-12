using Bit.Lib.Infra.Docker;
using Microsoft.Extensions.Configuration;

namespace Bit.Lib.Infra;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddDockerSecrets(this IConfigurationBuilder builder, string secretsPath = "/run/secrets")
    {
        return builder.Add(new DockerSecretsConfigurationSource { SecretsPath = secretsPath });
    }
}