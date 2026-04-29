using System.Text.Json;

namespace MIMLESvtt.Services;

public sealed record SettingsPreferenceSnapshot(
    string ThemeName,
    bool UseCompactLayout,
    bool Multiplayer,
    bool Chat,
    bool FogOfWar,
    bool RulesEngine,
    bool Persistence);

public class SettingsPreferenceService
{
    private static readonly SettingsPreferenceSnapshot DefaultSnapshot = new(
        ThemeName: "Default",
        UseCompactLayout: false,
        Multiplayer: false,
        Chat: false,
        FogOfWar: false,
        RulesEngine: false,
        Persistence: true);

    private readonly string _settingsFilePath;

    public SettingsPreferenceService()
        : this(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "App_Data", "mvu-settings.json")))
    {
    }

    public SettingsPreferenceService(string settingsFilePath)
    {
        _settingsFilePath = string.IsNullOrWhiteSpace(settingsFilePath)
            ? throw new ArgumentException("Settings file path is required.", nameof(settingsFilePath))
            : Path.GetFullPath(settingsFilePath);
    }

    public SettingsPreferenceSnapshot Load()
    {
        if (!File.Exists(_settingsFilePath))
        {
            return DefaultSnapshot;
        }

        try
        {
            var json = File.ReadAllText(_settingsFilePath);
            var snapshot = JsonSerializer.Deserialize<SettingsPreferenceSnapshot>(json);
            return snapshot ?? DefaultSnapshot;
        }
        catch (JsonException)
        {
            return DefaultSnapshot;
        }
    }

    public void Save(SettingsPreferenceSnapshot snapshot)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        var directory = Path.GetDirectoryName(_settingsFilePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_settingsFilePath, json);
    }
}
