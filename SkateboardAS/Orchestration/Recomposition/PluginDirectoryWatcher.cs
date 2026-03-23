namespace Orchestration.Recomposition;

public class PluginDirectoryWatcher : IDisposable
{
    private readonly FileSystemWatcher _watcher;
    public event EventHandler<string>? PluginChanged;

    public PluginDirectoryWatcher(string pluginsDirectory)
    {
        _watcher = new FileSystemWatcher(pluginsDirectory, "*.dll")
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            IncludeSubdirectories = true,
            EnableRaisingEvents = true
        };
        _watcher.Created += (_, e) => PluginChanged?.Invoke(this, e.FullPath);
        _watcher.Changed += (_, e) => PluginChanged?.Invoke(this, e.FullPath);
    }

    public void Dispose() => _watcher.Dispose();
}
