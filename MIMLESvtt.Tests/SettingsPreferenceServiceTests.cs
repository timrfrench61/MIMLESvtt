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
