using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class SettingsShellClassProjectionServiceTests
{
    [TestMethod]
    public void Project_DefaultTheme_MapsToDefaultClass()
    {
        var snapshot = new SettingsPreferenceSnapshot(
            ThemeName: "Default",
            UseCompactLayout: false,
            Multiplayer: false,
            Chat: false,
            FogOfWar: false,
            RulesEngine: false,
            Persistence: true);

        var result = SettingsShellClassProjectionService.Project(snapshot);

        Assert.AreEqual("theme-default", result.ThemeClass);
        Assert.AreEqual(string.Empty, result.ContentClass);
    }

    [TestMethod]
    public void Project_DarkThemeAndCompactLayout_MapsToExpectedClasses()
    {
        var snapshot = new SettingsPreferenceSnapshot(
            ThemeName: "Dark",
            UseCompactLayout: true,
            Multiplayer: false,
            Chat: false,
            FogOfWar: false,
            RulesEngine: false,
            Persistence: true);

        var result = SettingsShellClassProjectionService.Project(snapshot);

        Assert.AreEqual("theme-dark", result.ThemeClass);
        Assert.AreEqual("compact-layout", result.ContentClass);
    }

    [TestMethod]
    public void Project_HighContrastTheme_MapsToExpectedClass()
    {
        var snapshot = new SettingsPreferenceSnapshot(
            ThemeName: "HighContrast",
            UseCompactLayout: false,
            Multiplayer: false,
            Chat: false,
            FogOfWar: false,
            RulesEngine: false,
            Persistence: true);

        var result = SettingsShellClassProjectionService.Project(snapshot);

        Assert.AreEqual("theme-high-contrast", result.ThemeClass);
    }

    [TestMethod]
    public void Project_UnknownTheme_MapsToDefaultClass()
    {
        var snapshot = new SettingsPreferenceSnapshot(
            ThemeName: "Neon",
            UseCompactLayout: false,
            Multiplayer: false,
            Chat: false,
            FogOfWar: false,
            RulesEngine: false,
            Persistence: true);

        var result = SettingsShellClassProjectionService.Project(snapshot);

        Assert.AreEqual("theme-default", result.ThemeClass);
    }
}
