using Microsoft.AspNetCore.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;
using MIMLESvtt.src.Domain.Persistence.Services.Import;
using System.Reflection;

namespace MIMLESvtt.Tests;

[TestClass]
public class HomeGameSelectorTests
{
    [TestMethod]
    public void OpenGameSelector_WhenNoKnownSessions_ShowsEmptyStatus()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestOpenGameSelector();

        Assert.IsTrue(home.IsGameSelectorOpen);
        Assert.AreEqual(0, home.SavedSessionFiles.Count);
        StringAssert.Contains(home.SavedSessionStatusMessage, "No subscribed/saved sessions");
    }

    [TestMethod]
    public void OpenSelectedSession_WithoutSelection_ShowsGuidanceMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestOpenGameSelector();
        home.TestOpenSelectedSession();

        Assert.AreEqual("Select a session first.", home.SavedSessionStatusMessage);
    }

    [TestMethod]
    public void UseSelectedSessionAsJoinCode_WithoutSelection_ShowsGuidanceMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestUseSelectedSessionAsJoinCode();

        Assert.AreEqual("Select a session first.", home.JoinStatusMessage);
    }

    [TestMethod]
    public void StartNewSession_WhenToggleDisablesPermission_ShowsAccessMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestSetCanCreateSession(false);
        home.TestStartNewSession();

        StringAssert.Contains(home.NewSessionStatusMessage, "Only admins can create new sessions");
    }

    [TestMethod]
    public void JoinExistingGame_WithFriendlyJoinCode_LoadsKnownSession()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        var path = CreateTempFilePath("friendly-join-session", SnapshotFileExtensions.VttSession);

        try
        {
            var serializer = new VttSessionSnapshotSerializer();
            var session = new VttSession
            {
                Id = "session-friendly-join",
                Title = "Friendly Join Test"
            };

            File.WriteAllText(path, serializer.Save(session));
            workspace.AddKnownSnapshotPath(path);

            home.TestSetJoinCode("FRIENDLY-JOIN-SESSION");
            home.TestJoinExistingGame();

            Assert.IsNotNull(workspace.CurrentVttSession);
            Assert.AreEqual("session-friendly-join", workspace.CurrentVttSession!.Id);
            Assert.AreEqual("Joined existing game session.", home.JoinStatusMessage);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void SampleSessionSnapshot_File_CanBeLoadedIntoWorkspace()
    {
        var workspace = new VttSessionWorkspaceService();
        var root = FindRepoRoot();
        var samplePath = Path.Combine(root, "MIMLESvtt", "docs", "03-persistence", "sample-session.vttsession.json");

        workspace.OpenVttSessionFromFile(samplePath);

        Assert.IsNotNull(workspace.CurrentVttSession);
        Assert.AreEqual("session-sample-001", workspace.CurrentVttSession!.Id);
        Assert.AreEqual("Sample Tabletop Session", workspace.CurrentVttSession.Title);
        Assert.AreEqual(1, workspace.CurrentVttSession.Surfaces.Count);
        Assert.AreEqual(1, workspace.CurrentVttSession.Pieces.Count);
    }

    [TestMethod]
    public void JoinExistingGame_WithSampleJoinCodeWithoutKnownList_LoadsDocsSampleSession()
    {
        var workspace = new VttSessionWorkspaceService();

        var joined = workspace.TryJoinExistingGame("SAMPLE-SESSION", out var message);

        Assert.IsTrue(joined);
        StringAssert.Contains(message, "sample snapshots");
        Assert.IsNotNull(workspace.CurrentVttSession);
        Assert.AreEqual("session-sample-001", workspace.CurrentVttSession!.Id);
    }

    [TestMethod]
    public void JoinExistingGame_WithSampleJoinCodeAndUppercaseRole_LoadsDocsSampleSession()
    {
        var workspace = new VttSessionWorkspaceService();

        var joined = workspace.TryJoinExistingGame("SAMPLE-SESSION", out _);

        Assert.IsTrue(joined);
        Assert.IsNotNull(workspace.CurrentVttSession);
        Assert.AreEqual(ParticipantRole.GM, workspace.CurrentVttSession!.Participants[0].Role);
    }

    [TestMethod]
    public void CreateAndSaveNewSession_WithoutPath_ShowsValidationMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestSetNewSessionSavePath(string.Empty);
        home.TestCreateAndSaveNewSession();

        Assert.AreEqual("Session save path is required for Create + Save.", home.NewSessionStatusMessage);
    }

    [TestMethod]
    public void VisibleSavedSessions_WithSearchFilter_ReturnsFilteredCount()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestSetSessionSearchQuery("not-a-real-session-name");

        Assert.AreEqual(0, home.TestVisibleSessionCount());
    }

    [TestMethod]
    public void JoinExistingGame_WithoutJoinCode_ShowsValidationMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestSetJoinCode(string.Empty);
        home.TestJoinExistingGame();

        Assert.AreEqual("Join code is required.", home.JoinStatusMessage);
    }

    [TestMethod]
    public void AddKnownSessionPath_EmptyInput_ShowsValidationMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestSetKnownSessionPath(string.Empty);
        home.TestAddKnownSessionPath();

        Assert.AreEqual("Enter a session path first.", home.SavedSessionStatusMessage);
    }

    [TestMethod]
    public void RemoveSelectedSession_WithoutSelection_ShowsGuidanceMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestRemoveSelectedSession();

        Assert.AreEqual("Select a session first.", home.SavedSessionStatusMessage);
    }

    [TestMethod]
    public void StartNewSession_WhenCreationDenied_ShowsAccessMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        workspace.SetCanCreateSession(false);
        var home = CreateHome(workspace);

        home.TestStartNewSession();

        StringAssert.Contains(home.NewSessionStatusMessage, "Only admins can create new sessions");
    }

    private static Home CreateHome(VttSessionWorkspaceService workspace)
    {
        var home = new Home
        {
            WorkspaceService = workspace,
            Navigation = new TestNavigationManager()
        };

        InvokeProtected(home, "OnInitialized");
        return home;
    }

    private static void InvokeProtected(object instance, string methodName)
    {
        var method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method, $"Unable to find method '{methodName}'.");
        method.Invoke(instance, null);
    }

    private static string FindRepoRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "MIMLESvtt.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Unable to locate repository root containing MIMLESvtt.sln.");
    }

    private static string CreateTempFilePath(string prefix, string extension)
    {
        var fileName = $"{prefix}-{Guid.NewGuid():N}{extension}";
        return Path.Combine(Path.GetTempPath(), fileName);
    }

    private static void DeleteFileIfExists(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private sealed class TestNavigationManager : NavigationManager
    {
        public TestNavigationManager()
        {
            Initialize("http://localhost/", "http://localhost/");
        }

        protected override void NavigateToCore(string uri, NavigationOptions options)
        {
            Uri = ToAbsoluteUri(uri).ToString();
        }
    }
}
