using MIMLESvtt.Services;

namespace VttMvuView.Settings;

public static class SettingsUpdate
{
    public static SettingsState Reduce(SettingsState state, ISettingsAction action)
    {
        return ReduceWithEffects(state, action).State;
    }

    public static SettingsUpdateResult ReduceWithEffects(SettingsState state, ISettingsAction action)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(action);

        var nextState = action switch
        {
            SetThemeAction setTheme => state with
            {
                ThemeName = string.IsNullOrWhiteSpace(setTheme.ThemeName) ? state.ThemeName : setTheme.ThemeName.Trim(),
                StatusMessage = $"Theme set to {setTheme.ThemeName}."
            },
            SetCompactLayoutAction setCompact => state with
            {
                UseCompactLayout = setCompact.Enabled,
                StatusMessage = $"Compact layout {(setCompact.Enabled ? "enabled" : "disabled")}."
            },
            SetMultiplayerAction setValue => state with
            {
                Toggles = state.Toggles with { Multiplayer = setValue.Enabled },
                StatusMessage = $"Multiplayer {(setValue.Enabled ? "enabled" : "disabled")}."
            },
            SetChatAction setValue => state with
            {
                Toggles = state.Toggles with { Chat = setValue.Enabled },
                StatusMessage = $"Chat {(setValue.Enabled ? "enabled" : "disabled")}."
            },
            SetFogOfWarAction setValue => state with
            {
                Toggles = state.Toggles with { FogOfWar = setValue.Enabled },
                StatusMessage = $"Fog of War {(setValue.Enabled ? "enabled" : "disabled")}."
            },
            SetRulesEngineAction setValue => state with
            {
                Toggles = state.Toggles with { RulesEngine = setValue.Enabled },
                StatusMessage = $"Rules engine {(setValue.Enabled ? "enabled" : "disabled")}."
            },
            SetPersistenceAction setValue => state with
            {
                Toggles = state.Toggles with { Persistence = setValue.Enabled },
                StatusMessage = $"Persistence {(setValue.Enabled ? "enabled" : "disabled")}."
            },
            ClearStatusAction => state with { StatusMessage = null },
            PersistSettingsAction => state with { StatusMessage = "Settings saved." },
            _ => state
        };

        if (action is not PersistSettingsAction)
        {
            return new SettingsUpdateResult(nextState, []);
        }

        var snapshot = new SettingsPreferenceSnapshot(
            ThemeName: nextState.ThemeName,
            UseCompactLayout: nextState.UseCompactLayout,
            Multiplayer: nextState.Toggles.Multiplayer,
            Chat: nextState.Toggles.Chat,
            FogOfWar: nextState.Toggles.FogOfWar,
            RulesEngine: nextState.Toggles.RulesEngine,
            Persistence: nextState.Toggles.Persistence);

        return new SettingsUpdateResult(nextState, [new PersistSettingsEffect(snapshot)]);
    }
}
