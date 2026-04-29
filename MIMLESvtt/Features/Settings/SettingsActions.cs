namespace VttMvuView.Settings;

public interface ISettingsAction;

public sealed record SetThemeAction(string ThemeName) : ISettingsAction;

public sealed record SetCompactLayoutAction(bool Enabled) : ISettingsAction;

public sealed record SetMultiplayerAction(bool Enabled) : ISettingsAction;

public sealed record SetChatAction(bool Enabled) : ISettingsAction;

public sealed record SetFogOfWarAction(bool Enabled) : ISettingsAction;

public sealed record SetRulesEngineAction(bool Enabled) : ISettingsAction;

public sealed record SetPersistenceAction(bool Enabled) : ISettingsAction;

public sealed record ClearStatusAction : ISettingsAction;

public sealed record PersistSettingsAction : ISettingsAction;
