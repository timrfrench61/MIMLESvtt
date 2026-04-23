using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Components.Pages;

public partial class Home : ComponentBase
{
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] public VttSessionWorkspaceService WorkspaceService { get; set; } = default!;

    private bool isGameSelectorOpen;
    private readonly List<KnownGameSession> savedSessionFiles = [];
    private string? selectedSessionPath;
    private string newKnownSessionPath = string.Empty;
    private string joinCode = string.Empty;
    private string currentUserName = "(unknown)";
    private bool isCurrentUserAdmin;
    private string newSessionTitle = "New Session";
    private string newSessionSavePath = string.Empty;
    private string sessionSearchQuery = string.Empty;
    private SavedSessionSort savedSessionSort = SavedSessionSort.LastUpdatedDesc;
    private string? savedSessionStatusMessage;
    private string? joinStatusMessage;
    private string? newSessionStatusMessage;
    private string knownSessionRegistryPath = string.Empty;
    private bool knownSessionRegistryLoadedFromDisk;

    internal bool IsGameSelectorOpen => isGameSelectorOpen;
    internal IReadOnlyList<KnownGameSession> SavedSessionFiles => savedSessionFiles;
    internal string? SelectedSessionPath => selectedSessionPath;
    internal string SavedSessionStatusMessage => savedSessionStatusMessage ?? string.Empty;
    internal string JoinStatusMessage => joinStatusMessage ?? string.Empty;
    internal string NewSessionStatusMessage => newSessionStatusMessage ?? string.Empty;

    private enum SavedSessionSort
    {
        LastUpdatedDesc,
        LastUpdatedAsc,
        FileNameAsc,
        FileNameDesc
    }

    protected override async Task OnInitializedAsync()
    {
        await ApplyAuthorizationStateAsync();
        RefreshKnownSessionRegistryStatus();
        RefreshSavedSessions();
    }

    private void RefreshKnownSessionRegistryStatus()
    {
        knownSessionRegistryPath = WorkspaceService.GetKnownGameSessionRegistryPath();
        knownSessionRegistryLoadedFromDisk = WorkspaceService.HasKnownGameSessionRegistry();
    }

    private async Task ApplyAuthorizationStateAsync()
    {
        var authenticationState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var user = authenticationState.User;

        currentUserName = string.IsNullOrWhiteSpace(user.Identity?.Name)
            ? "(unknown)"
            : user.Identity!.Name!;

        isCurrentUserAdmin = user.IsInRole("Admin");
        WorkspaceService.SetCanCreateSession(isCurrentUserAdmin);
    }

    private void RefreshKnownSessions()
    {
        RefreshKnownSessionRegistryStatus();
        RefreshSavedSessions();
        savedSessionStatusMessage = savedSessionFiles.Count == 0
            ? "No saved sessions are currently known."
            : $"Loaded {savedSessionFiles.Count} known session(s).";
    }

    private void SaveKnownSessionRegistryNow()
    {
        try
        {
            WorkspaceService.SaveKnownGameSessionRegistry();
            RefreshKnownSessionRegistryStatus();
            savedSessionStatusMessage = "Saved known game session registry.";
        }
        catch (Exception ex)
        {
            savedSessionStatusMessage = ex.Message;
        }
    }

    private void UseSelectedSessionAsJoinCode()
    {
        if (string.IsNullOrWhiteSpace(selectedSessionPath))
        {
            joinStatusMessage = "Select a session first.";
            return;
        }

        joinCode = selectedSessionPath;
        joinStatusMessage = "Join code prefilled from selected session.";
    }

    private void AddKnownSessionPath()
    {
        if (string.IsNullOrWhiteSpace(newKnownSessionPath))
        {
            savedSessionStatusMessage = "Enter a session path first.";
            return;
        }

        try
        {
            WorkspaceService.AddKnownSnapshotPath(newKnownSessionPath);
            newKnownSessionPath = string.Empty;
            RefreshKnownSessionRegistryStatus();
            RefreshSavedSessions();
            savedSessionStatusMessage = "Added known session path.";
        }
        catch (Exception ex)
        {
            savedSessionStatusMessage = ex.Message;
        }
    }

    private IReadOnlyList<KnownGameSession> GetVisibleSavedSessions()
    {
        IEnumerable<KnownGameSession> sessions = savedSessionFiles;

        if (!string.IsNullOrWhiteSpace(sessionSearchQuery))
        {
            var query = sessionSearchQuery.Trim();
            sessions = sessions.Where(s =>
                s.FileName.Contains(query, StringComparison.OrdinalIgnoreCase)
                || s.FilePath.Contains(query, StringComparison.OrdinalIgnoreCase)
                || s.JoinCode.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        sessions = savedSessionSort switch
        {
            SavedSessionSort.LastUpdatedAsc => sessions.OrderBy(s => s.LastWriteTimeUtc ?? DateTime.MinValue),
            SavedSessionSort.FileNameAsc => sessions.OrderBy(s => s.FileName, StringComparer.OrdinalIgnoreCase),
            SavedSessionSort.FileNameDesc => sessions.OrderByDescending(s => s.FileName, StringComparer.OrdinalIgnoreCase),
            _ => sessions.OrderByDescending(s => s.LastWriteTimeUtc ?? DateTime.MinValue)
        };

        return sessions.ToList();
    }

    private void RemoveSelectedSession()
    {
        if (string.IsNullOrWhiteSpace(selectedSessionPath))
        {
            savedSessionStatusMessage = "Select a session first.";
            return;
        }

        if (!WorkspaceService.RemoveKnownSnapshotPath(selectedSessionPath))
        {
            savedSessionStatusMessage = "Unable to remove selected session.";
            return;
        }

        selectedSessionPath = null;
        RefreshKnownSessionRegistryStatus();
        RefreshSavedSessions();
        savedSessionStatusMessage = "Removed selected session from known list.";
    }

    private void OpenGameSelector()
    {
        isGameSelectorOpen = true;
        RefreshKnownSessionRegistryStatus();
        RefreshSavedSessions();
        savedSessionStatusMessage = savedSessionFiles.Count == 0
            ? "No subscribed/saved sessions are available yet."
            : $"Loaded {savedSessionFiles.Count} saved session(s).";
    }

    private void CloseGameSelector()
    {
        isGameSelectorOpen = false;
    }

    private void RefreshSavedSessions()
    {
        savedSessionFiles.Clear();
        savedSessionFiles.AddRange(WorkspaceService.ListKnownGameSessions());

        if (!string.IsNullOrWhiteSpace(selectedSessionPath) &&
            savedSessionFiles.All(s => !string.Equals(s.FilePath, selectedSessionPath, StringComparison.OrdinalIgnoreCase)))
        {
            selectedSessionPath = null;
        }
    }

    private void SelectSession(string path)
    {
        selectedSessionPath = path;
    }

    private void OpenSelectedSession()
    {
        if (string.IsNullOrWhiteSpace(selectedSessionPath))
        {
            savedSessionStatusMessage = "Select a session first.";
            return;
        }

        try
        {
            WorkspaceService.OpenVttSessionFromFile(selectedSessionPath);
            savedSessionStatusMessage = "Opened selected session.";
            Navigation.NavigateTo("/workspace");
        }
        catch (Exception ex)
        {
            savedSessionStatusMessage = ex.Message;
        }
    }

    private void JoinExistingGame()
    {
        if (WorkspaceService.TryJoinExistingGame(joinCode, out var message))
        {
            joinStatusMessage = message;
            Navigation.NavigateTo("/workspace");
            return;
        }

        joinStatusMessage = message;
    }

    private void StartNewSession()
    {
        try
        {
            WorkspaceService.CreateNewSession();
            if (!string.IsNullOrWhiteSpace(newSessionTitle))
            {
                WorkspaceService.UpdateSessionTitle(newSessionTitle.Trim());
            }

            newSessionStatusMessage = "Created new session.";
            Navigation.NavigateTo("/workspace");
        }
        catch (Exception ex)
        {
            newSessionStatusMessage = ex.Message;
        }
    }

    private void CreateAndSaveNewSession()
    {
        if (string.IsNullOrWhiteSpace(newSessionSavePath))
        {
            newSessionStatusMessage = "Session save path is required for Create + Save.";
            return;
        }

        try
        {
            WorkspaceService.CreateNewSession();

            if (!string.IsNullOrWhiteSpace(newSessionTitle))
            {
                WorkspaceService.UpdateSessionTitle(newSessionTitle.Trim());
            }

            WorkspaceService.SaveCurrentSessionAs(newSessionSavePath);
            RefreshSavedSessions();
            newSessionStatusMessage = "Created and saved new session.";
            Navigation.NavigateTo("/workspace");
        }
        catch (Exception ex)
        {
            newSessionStatusMessage = ex.Message;
        }
    }

    internal void TestOpenGameSelector() => OpenGameSelector();
    internal void TestCloseGameSelector() => CloseGameSelector();
    internal void TestSelectSession(string path) => SelectSession(path);
    internal void TestOpenSelectedSession() => OpenSelectedSession();
    internal void TestJoinExistingGame() => JoinExistingGame();
    internal void TestStartNewSession() => StartNewSession();
    internal void TestSetJoinCode(string value) => joinCode = value ?? string.Empty;
    internal void TestSetKnownSessionPath(string value) => newKnownSessionPath = value ?? string.Empty;
    internal void TestAddKnownSessionPath() => AddKnownSessionPath();
    internal void TestRemoveSelectedSession() => RemoveSelectedSession();
    internal void TestRefreshKnownSessions() => RefreshKnownSessions();
    internal void TestSetSessionSearchQuery(string value) => sessionSearchQuery = value ?? string.Empty;
    internal int TestVisibleSessionCount() => GetVisibleSavedSessions().Count;
    internal void TestSetCanCreateSession(bool value)
    {
        isCurrentUserAdmin = value;
        WorkspaceService.SetCanCreateSession(value);
    }
    internal void TestUseSelectedSessionAsJoinCode() => UseSelectedSessionAsJoinCode();
    internal void TestSetNewSessionTitle(string value) => newSessionTitle = value ?? string.Empty;
    internal void TestSetNewSessionSavePath(string value) => newSessionSavePath = value ?? string.Empty;
    internal void TestCreateAndSaveNewSession() => CreateAndSaveNewSession();
}
