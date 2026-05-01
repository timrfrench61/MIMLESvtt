using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Services;

namespace MIMLESvtt.Tests;

[TestClass]
public class SettingsPreferenceServiceTests
{
    [TestMethod]
    public void Load_WhenFileMissing_ReturnsDefaultSnapshot()
    {
        var path = BuildTempSettingsPath();

        try
        {
            var service = new SettingsPreferenceService(path);
            var snapshot = service.Load();

            Assert.AreEqual("Default", snapshot.ThemeName);
            Assert.IsFalse(snapshot.Multiplayer);
            Assert.IsTrue(snapshot.Persistence);
        }
        finally
        {
            DeleteIfExists(path);
        }
    }

    [TestMethod]
    public void Save_RaisesPreferencesChanged_EachTimeSaveIsCalled()
    {
        var path = BuildTempSettingsPath();

        try
        {
            var service = new SettingsPreferenceService(path);
            var eventCount = 0;
            service.PreferencesChanged += _ => eventCount++;

            service.Save(new SettingsPreferenceSnapshot("Default", false, false, false, false, false, true));
            service.Save(new SettingsPreferenceSnapshot("Dark", true, true, true, false, true, false));

            Assert.AreEqual(2, eventCount);
        }
        finally
        {
            DeleteIfExists(path);
        }
    }

    [TestMethod]
    public void Save_RaisesPreferencesChanged_WithSavedSnapshot()
    {
        var path = BuildTempSettingsPath();

        try
        {
            var service = new SettingsPreferenceService(path);
            SettingsPreferenceSnapshot? captured = null;
            service.PreferencesChanged += snapshot => captured = snapshot;

            var input = new SettingsPreferenceSnapshot(
                ThemeName: "HighContrast",
                UseCompactLayout: true,
                Multiplayer: true,
                Chat: false,
                FogOfWar: true,
                RulesEngine: false,
                Persistence: true);

            service.Save(input);

            Assert.IsNotNull(captured);
            Assert.AreEqual("HighContrast", captured!.ThemeName);
            Assert.IsTrue(captured.UseCompactLayout);
            Assert.IsTrue(captured.Multiplayer);
            Assert.IsTrue(captured.Persistence);
        }
        finally
        {
            DeleteIfExists(path);
        }
    }

    [TestMethod]
    public void Save_ThenLoad_RoundTripsSnapshot()
    {
        var path = BuildTempSettingsPath();

        try
        {
            var service = new SettingsPreferenceService(path);
            var input = new SettingsPreferenceSnapshot(
                ThemeName: "Dark",
                UseCompactLayout: true,
                Multiplayer: true,
                Chat: true,
                FogOfWar: true,
                RulesEngine: true,
                Persistence: false);

            service.Save(input);
            var output = service.Load();

            Assert.AreEqual("Dark", output.ThemeName);
            Assert.IsTrue(output.UseCompactLayout);
            Assert.IsTrue(output.Multiplayer);
            Assert.IsFalse(output.Persistence);
        }
        finally
        {
            DeleteIfExists(path);
        }
    }

    private static string BuildTempSettingsPath()
    {
        return Path.Combine(Path.GetTempPath(), $"mimlesvtt-settings-{Guid.NewGuid():N}.json");
    }

    private static void DeleteIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}
