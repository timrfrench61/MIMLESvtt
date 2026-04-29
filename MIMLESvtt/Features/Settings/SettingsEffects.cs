using MIMLESvtt.Services;

namespace VttMvuView.Settings;

public interface ISettingsEffect;

public sealed record PersistSettingsEffect(SettingsPreferenceSnapshot Snapshot) : ISettingsEffect;

public sealed record SettingsUpdateResult(SettingsState State, IReadOnlyList<ISettingsEffect> Effects);
