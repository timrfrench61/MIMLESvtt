using Microsoft.VisualStudio.TestTools.UnitTesting;
using VttMvuView.Settings;

namespace MIMLESvtt.Tests;

[TestClass]
public class SettingsUpdateTests
{
    [TestMethod]
    public void SetTheme_UpdatesThemeName()
    {
        var state = BuildInitialState();

        var result = SettingsUpdate.Reduce(state, new SetThemeAction("Dark"));

        Assert.AreEqual("Dark", result.ThemeName);
        Assert.AreEqual("Theme set to Dark.", result.StatusMessage);
    }

    [TestMethod]
    public void SetMultiplayer_UpdatesToggle()
    {
        var state = BuildInitialState();

        var result = SettingsUpdate.Reduce(state, new SetMultiplayerAction(true));

        Assert.IsTrue(result.Toggles.Multiplayer);
    }

    [TestMethod]
    public void SetPersistence_UpdatesToggle()
    {
        var state = BuildInitialState();

        var result = SettingsUpdate.Reduce(state, new SetPersistenceAction(false));

        Assert.IsFalse(result.Toggles.Persistence);
        Assert.AreEqual("Persistence disabled.", result.StatusMessage);
    }

    [TestMethod]
    public void ClearStatus_RemovesStatusMessage()
    {
        var state = BuildInitialState() with { StatusMessage = "Some status" };

        var result = SettingsUpdate.Reduce(state, new ClearStatusAction());

        Assert.IsNull(result.StatusMessage);
    }

    [TestMethod]
    public void PersistSettingsAction_ProducesPersistEffect()
    {
        var state = BuildInitialState() with
        {
            ThemeName = "HighContrast",
            UseCompactLayout = true,
            Toggles = new SettingsToggleState(
                Multiplayer: true,
                Chat: false,
                FogOfWar: true,
                RulesEngine: false,
                Persistence: true)
        };

        var result = SettingsUpdate.ReduceWithEffects(state, new PersistSettingsAction());

        Assert.AreEqual("Settings saved.", result.State.StatusMessage);
        Assert.AreEqual(1, result.Effects.Count);
        Assert.IsInstanceOfType<PersistSettingsEffect>(result.Effects[0]);

        var effect = (PersistSettingsEffect)result.Effects[0];
        Assert.AreEqual("HighContrast", effect.Snapshot.ThemeName);
        Assert.IsTrue(effect.Snapshot.UseCompactLayout);
        Assert.IsTrue(effect.Snapshot.Multiplayer);
        Assert.IsTrue(effect.Snapshot.FogOfWar);
    }

    private static SettingsState BuildInitialState()
    {
        return new SettingsState(
            new SettingsToggleState(
                Multiplayer: false,
                Chat: false,
                FogOfWar: false,
                RulesEngine: false,
                Persistence: true),
            ThemeName: "Default",
            UseCompactLayout: false,
            StatusMessage: null);
    }
}
