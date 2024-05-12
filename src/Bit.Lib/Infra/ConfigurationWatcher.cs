using Microsoft.Extensions.Configuration;

namespace Bit.Lib.Infra;

public class ConfigurationWatcher
{
    public event EventHandler? ConfigurationChanged;

    public ConfigurationWatcher(IConfigurationRoot configurationRoot)
    {
        configurationRoot.GetReloadToken().RegisterChangeCallback(OnConfigurationChanged, null);
    }

    private void OnConfigurationChanged(object? state)
    {
        ConfigurationChanged?.Invoke(this, EventArgs.Empty);
    }
}