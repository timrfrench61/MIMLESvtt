using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MIMLESvtt.Components.Pages;
using MIMLESvtt.src.Domain.Models;
using MIMLESvtt.src.Domain.Persistence.Snapshot;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;
using MIMLESvtt.src.Domain.Persistence.Services.Import;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

namespace MIMLESvtt.Tests;

[TestClass]
public class HomeGameSelectorTests
{
    [TestInitialize]
    public void TestInitialize()
    {
        ResetWorkspaceArtifacts();
    }

    [TestMethod]
    public void SharedSetupOptionList_Wiring_IsPresentInSetupPages()
    {
        var root = FindRepoRoot();
        var pagesDirectory = Path.Combine(root, "MIMLESvtt", "Components", "Pages");

        var gameSetupSource = File.ReadAllText(Path.Combine(pagesDirectory, "GameSetupPage.razor"));
        var workspaceLaunchSource = File.ReadAllText(Path.Combine(pagesDirectory, "WorkspaceLaunchPage.razor"));

        StringAssert.Contains(gameSetupSource, "<SetupOptionButtonList");
        StringAssert.Contains(gameSetupSource, "<SetupStepNavigationRow");
        StringAssert.Contains(gameSetupSource, "DisableBack=\"DisableBack\"");
        StringAssert.Contains(gameSetupSource, "DisableContinue=\"DisableContinue\"");
        StringAssert.Contains(gameSetupSource, "Tone=\"SetupOptionButtonTone.Primary\"");
        StringAssert.Contains(gameSetupSource, "Tone=\"SetupOptionButtonTone.Success\"");
        StringAssert.Contains(gameSetupSource, "Tone=\"SetupOptionButtonTone.Secondary\"");
        StringAssert.Contains(gameSetupSource, "EmptyMessage=\"No session sources available.\"");
        StringAssert.Contains(gameSetupSource, "SetupModeOptionCatalogService.GameSetupSessionModes()");
        StringAssert.Contains(gameSetupSource, "SetupOptionButtonProjectionService.Project(");

        StringAssert.Contains(workspaceLaunchSource, "<SetupOptionButtonList");
        StringAssert.Contains(workspaceLaunchSource, "<SetupStepNavigationRow");
        StringAssert.Contains(workspaceLaunchSource, "DisableBack=\"DisableBack\"");
        StringAssert.Contains(workspaceLaunchSource, "DisableContinue=\"DisableContinue\"");
        StringAssert.Contains(workspaceLaunchSource, "Tone=\"SetupOptionButtonTone.Primary\"");
        StringAssert.Contains(workspaceLaunchSource, "Tone=\"SetupOptionButtonTone.Secondary\"");
        StringAssert.Contains(workspaceLaunchSource, "Tone=\"SetupOptionButtonTone.Success\"");
        StringAssert.Contains(workspaceLaunchSource, "EmptyMessage=\"No scenarios available for this mode.\"");
        StringAssert.Contains(workspaceLaunchSource, "SetupModeOptionCatalogService.WorkspaceScenarioModes()");
        StringAssert.Contains(workspaceLaunchSource, "SetupOptionButtonProjectionService.Project(");
    }

    [TestMethod]
    public void OpenGameSelector_WhenNoKnownSessions_ShowsEmptyStatus()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestOpenGameSelector();

        Assert.IsTrue(home.IsGameSelectorOpen);
        Assert.AreEqual(0, home.SavedSessionFiles.Count);
        StringAssert.Contains(home.SavedSessionStatusMessage, "No subscribed or saved game sessions");
    }

    [TestMethod]
    public void OpenSelectedSession_WithKnownSession_LoadsSessionAndNavigatesToWorkspace()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        var path = CreateTempFilePath("open-selected-session", SnapshotFileExtensions.VttSession);

        try
        {
            var serializer = new VttSessionSnapshotSerializer();
            var session = new VttSession
            {
                Id = "session-open-selected",
                Title = "Open Selected Session"
            };

            File.WriteAllText(path, serializer.Save(session));
            workspace.AddKnownSnapshotPath(path);

            home.TestOpenGameSelector();
            home.TestSelectSession(Path.GetFullPath(path));
            home.TestOpenSelectedSession();

            Assert.IsNotNull(workspace.CurrentVttSession);
            Assert.AreEqual("session-open-selected", workspace.CurrentVttSession!.Id);
            Assert.AreEqual("Opened selected session.", home.SavedSessionStatusMessage);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void OpenSelectedSession_WithMissingSessionFile_ShowsErrorMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        var missingPath = CreateTempFilePath("open-missing-session", SnapshotFileExtensions.VttSession);
        workspace.AddKnownSnapshotPath(missingPath);

        home.TestOpenGameSelector();
        home.TestSelectSession(Path.GetFullPath(missingPath));
        home.TestOpenSelectedSession();

        StringAssert.Contains(home.SavedSessionStatusMessage, "not found");
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
    public void StartNewSession_AsAdmin_SeedsReadonlyEmptyCampaign()
    {
        var workspace = new VttSessionWorkspaceService();
        workspace.SetCanCreateSession(true);
        var home = CreateHome(workspace);

        home.TestStartNewSession();

        Assert.IsNotNull(workspace.CurrentVttSession);
        Assert.AreEqual("New Session", workspace.CurrentVttSession!.Campaign.Name);
        Assert.IsFalse(workspace.CurrentVttSession.Campaign.IsReadOnly);
        Assert.IsFalse(string.IsNullOrWhiteSpace(workspace.CurrentVttSession.Campaign.GameboxId));
        Assert.IsNotNull(workspace.CurrentVttSession.Campaign.CurrentScenarioSnapshot);
    }

    [TestMethod]
    public void ToggleReadonlyCampaignHidden_HidesAndUnhidesCheckersCampaignFromSeedList()
    {
        var workspace = new VttSessionWorkspaceService();
        workspace.SetCanCreateSession(true);
        var home = CreateHome(workspace);

        workspace.SetReadonlyCampaignHidden("CHECKERS-CAMPAIGN", true);
        Assert.IsFalse(workspace.ListReadonlyScenarios().Any(s => s.CampaignId == "CHECKERS-CAMPAIGN"));

        workspace.SetReadonlyCampaignHidden("CHECKERS-CAMPAIGN", false);
        Assert.IsTrue(workspace.ListReadonlyScenarios().Any(s => s.CampaignId == "CHECKERS-CAMPAIGN"));
    }

    [TestMethod]
    public void DeleteReadonlyCampaign_PersistsAcrossAppSessions()
    {
        var dataRootPath = Path.Combine(Path.GetTempPath(), $"mimlesvtt-hidden-campaign-{Guid.NewGuid():N}");

        try
        {
            var firstWorkspace = new VttSessionWorkspaceService(dataRootPath);
            firstWorkspace.SetCanCreateSession(true);
            firstWorkspace.SetReadonlyCampaignHidden("CHECKERS-CAMPAIGN", true);

            var secondWorkspace = new VttSessionWorkspaceService(dataRootPath);
            Assert.IsFalse(secondWorkspace.ListReadonlyScenarios().Any(s => s.CampaignId == "CHECKERS-CAMPAIGN"));
        }
        finally
        {
            if (Directory.Exists(dataRootPath))
            {
                Directory.Delete(dataRootPath, recursive: true);
            }
        }
    }

    [TestMethod]
    public void OpenSelectedCampaign_CheckersCampaign_ActivatesCurrentScenarioSnapshot()
    {
        var workspace = new VttSessionWorkspaceService();
        workspace.SetCanCreateSession(true);
        var home = CreateHome(workspace);
        home.TestSetCanCreateSession(true);

        home.TestSelectCampaign("CHECKERS-CAMPAIGN");
        home.TestOpenSelectedCampaign();

        Assert.IsNotNull(workspace.CurrentVttSession, home.SavedSessionStatusMessage);
        Assert.AreEqual("CHECKERS-CAMPAIGN", workspace.CurrentVttSession!.Campaign.Id);
        Assert.IsTrue(workspace.CurrentVttSession.Pieces.Count > 0);
    }

    [TestMethod]
    public void DeleteSelectedCampaign_KnownSession_RemovesFromCampaignList()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        var path = CreateTempFilePath("delete-known-campaign", SnapshotFileExtensions.VttSession);

        try
        {
            var serializer = new VttSessionSnapshotSerializer();
            var session = new VttSession
            {
                Id = "session-delete-known",
                Title = "Delete Known Campaign"
            };

            File.WriteAllText(path, serializer.Save(session));
            workspace.AddKnownSnapshotPath(path);

            home.TestOpenGameSelector();
            home.TestSelectCampaign($"KNOWN:{Path.GetFullPath(path)}");
            home.TestDeleteSelectedCampaign();

            Assert.IsFalse(home.SavedSessionFiles.Any(s => string.Equals(s.FilePath, Path.GetFullPath(path), StringComparison.OrdinalIgnoreCase)));
            Assert.IsFalse(File.Exists(Path.GetFullPath(path)));
            Assert.AreEqual("Deleted selected game from games list.", home.SavedSessionStatusMessage);
        }
        finally
        {
            DeleteFileIfExists(path);
        }
    }

    [TestMethod]
    public void DeleteSelectedCampaign_ReadonlyCampaign_HidesCampaign()
    {
        var workspace = new VttSessionWorkspaceService();
        workspace.SetCanCreateSession(true);
        var home = CreateHome(workspace);

        home.TestOpenGameSelector();
        home.TestSelectCampaign("CHECKERS-CAMPAIGN");
        home.TestDeleteSelectedCampaign();

        Assert.IsFalse(workspace.ListReadonlyScenarios().Any(s => s.CampaignId == "CHECKERS-CAMPAIGN"));
        Assert.AreEqual("Deleted selected game from games list.", home.SavedSessionStatusMessage);
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

            var joinCode = Path
                .GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(Path.GetFileName(path)))
                .ToUpperInvariant();

            home.TestSetJoinCode(joinCode);
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
        Assert.IsNotNull(workspace.CurrentVttSession.Campaign.CurrentScenarioSnapshot);
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
    public void JoinExistingGame_WithSampleJoinCode_PersistsLastJoinedUtcInKnownSessionRegistry()
    {
        var workspace = new VttSessionWorkspaceService();
        var joined = workspace.TryJoinExistingGame("SAMPLE-SESSION", out _);

        Assert.IsTrue(joined);

        var registryPath = workspace.GetKnownGameSessionRegistryPath();
        Assert.IsTrue(File.Exists(registryPath));

        using var document = JsonDocument.Parse(File.ReadAllText(registryPath));
        var entries = document.RootElement.GetProperty("Entries").EnumerateArray();

        JsonElement? matchedEntry = null;
        foreach (var entry in entries)
        {
            if (!entry.TryGetProperty("JoinCode", out var joinCodeElement))
            {
                continue;
            }

            if (!string.Equals(joinCodeElement.GetString(), "SAMPLE-SESSION", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            matchedEntry = entry;
            break;
        }

        Assert.IsTrue(matchedEntry.HasValue, "Expected SAMPLE-SESSION to be persisted in known-game-sessions registry entries.");
        Assert.IsTrue(matchedEntry.Value.TryGetProperty("LastJoinedUtc", out var lastJoinedElement));
        Assert.AreEqual(JsonValueKind.String, lastJoinedElement.ValueKind);
        Assert.IsTrue(DateTime.TryParse(lastJoinedElement.GetString(), out _));
    }

    [TestMethod]
    public void CreateAndSaveNewSession_WithoutPath_ShowsValidationMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        var home = CreateHome(workspace);

        home.TestSetNewSessionSavePath(string.Empty);
        home.TestCreateAndSaveNewSession();

        Assert.AreEqual("Campaign save path is required for Create + Save.", home.NewSessionStatusMessage);
    }

    [TestMethod]
    public void StartNewSession_WithoutGameboxSelection_ShowsValidationMessage()
    {
        var workspace = new VttSessionWorkspaceService();
        workspace.SetCanCreateSession(true);
        var home = CreateHome(workspace);

        home.TestSetNewCampaignGameboxId(string.Empty);
        home.TestStartNewSession();

        Assert.AreEqual("Select a valid Gamebox before creating a campaign.", home.NewSessionStatusMessage);
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
            Navigation = new TestNavigationManager(),
            AuthenticationStateProvider = new TestAuthenticationStateProvider(isAdmin: workspace.State.CanCreateSession)
        };

        InvokeProtectedAsync(home, "OnInitializedAsync").GetAwaiter().GetResult();
        return home;
    }

    private static async Task InvokeProtectedAsync(object instance, string methodName)
    {
        var method = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
        Assert.IsNotNull(method, $"Unable to find method '{methodName}'.");

        if (method.Invoke(instance, null) is Task task)
        {
            await task;
        }
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

            if (File.Exists(Path.Combine(directory.FullName, "MIMLESvtt", "MIMLESvtt.csproj")))
            {
                return directory.FullName;
            }

            if (File.Exists(Path.Combine(directory.FullName, "MIMLESvtt.csproj")))
            {
                return directory.Parent?.FullName ?? directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Unable to locate repository root containing MIMLESvtt.sln.");
    }

    private static void ResetWorkspaceArtifacts()
    {
        var appDataPath = Path.Combine(AppContext.BaseDirectory, "App_Data");
        if (Directory.Exists(appDataPath))
        {
            Directory.Delete(appDataPath, recursive: true);
        }
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

    private sealed class TestAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly AuthenticationState _state;

        public TestAuthenticationStateProvider(bool isAdmin)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, isAdmin ? "admin-user" : "player-user")
            };

            if (isAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType: "Test"));
            _state = new AuthenticationState(principal);
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(_state);
        }
    }
}
