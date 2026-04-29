namespace VttMvuView.Settings;

public sealed record SettingsToggleState(
    bool Multiplayer,
    bool Chat,
    bool FogOfWar,
    bool RulesEngine,
    bool Persistence);

public sealed record SettingsState(
    SettingsToggleState Toggles,
    string ThemeName,
    bool UseCompactLayout,
    string? StatusMessage);
