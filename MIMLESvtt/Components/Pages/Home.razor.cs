using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using MIMLESvtt.src.Domain.Persistence.Models;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Components.Pages;

public partial class Home : ComponentBase
{
    private enum CampaignBrowserViewMode
    {
        DetailList,
        Card
    }

    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthenticationStateProvider { get; set; } = default!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = default!;
    [Inject] public VttSessionWorkspaceService WorkspaceService { get; set; } = default!;

    private bool isGameSelectorOpen;
    private readonly List<KnownGameSession> savedSessionFiles = [];
    private string? selectedSessionPath;
    private string? selectedCampaignId;
    private string newKnownSessionPath = string.Empty;
    private string customJoinCodeInput = string.Empty;
    private string joinCode = string.Empty;
    private string currentUserName = "(unknown)";
    private bool isCurrentUserAdmin;
    private string newSessionTitle = "New Session";
    private string newSessionSavePath = string.Empty;
    private string newCampaignGameboxId = string.Empty;
    private string newGameboxId = string.Empty;
    private readonly List<string> availableGameboxIds = [];
    private string sessionSearchQuery = string.Empty;
    private SavedSessionSort savedSessionSort = SavedSessionSort.LastUpdatedDesc;
    private string? savedSessionStatusMessage;
    private string? joinStatusMessage;
    private string? newSessionStatusMessage;
    private string? newGameboxStatusMessage;
    private string knownSessionRegistryPath = string.Empty;
    private bool knownSessionRegistryLoadedFromDisk;
    private string? knownSessionRegistryWarning;
    private CampaignBrowserViewMode campaignBrowserViewMode = CampaignBrowserViewMode.DetailList;

    internal bool IsGameSelectorOpen => isGameSelectorOpen;
    internal IReadOnlyList<KnownGameSession> SavedSessionFiles => savedSessionFiles;
    internal string? SelectedSessionPath => selectedSessionPath;
    internal string SavedSessionStatusMessage => savedSessionStatusMessage ?? string.Empty;
    internal string JoinStatusMessage => joinStatusMessage ?? string.Empty;
    internal string NewSessionStatusMessage => newSessionStatusMessage ?? string.Empty;
    internal string NewGameboxStatusMessage => newGameboxStatusMessage ?? string.Empty;

    private sealed class CampaignSelectionEntry
    {
        public string CampaignId { get; init; } = string.Empty;
        public string CampaignName { get; init; } = string.Empty;
        public string CurrentScenarioId { get; init; } = string.Empty;
        public string CurrentScenarioName { get; init; } = string.Empty;
        public string CurrentSnapshotPath { get; init; } = string.Empty;
        public bool IsReadonlySeed { get; init; }
        public bool IsKnownSessionEntry { get; init; }
        public string? SessionPath { get; init; }
    }

    private enum SavedSessionSort
    {
        LastUpdatedDesc,
        LastUpdatedAsc,
        LastJoinedDesc,
        LastJoinedAsc,
        JoinCodeUpdatedDesc,
        JoinCodeUpdatedAsc,
        FileNameAsc,
        FileNameDesc
    }

    protected override async Task OnInitializedAsync()
    {
        await ApplyAuthorizationStateAsync();
        campaignBrowserViewMode = ParseCampaignBrowserViewMode(WorkspaceService.GetCampaignBrowserViewMode());
        RefreshKnownSessionRegistryStatus();
        RefreshSavedSessions();
        RefreshGameboxOptions();
    }

    private void RefreshGameboxOptions()
    {
        availableGameboxIds.Clear();
        availableGameboxIds.AddRange(WorkspaceService.ListAvailableGameboxIds());

        if (string.IsNullOrWhiteSpace(newCampaignGameboxId) && availableGameboxIds.Count > 0)
        {
            newCampaignGameboxId = availableGameboxIds[0];
        }
    }

    private async Task DeleteSelectedCampaign()
    {
        var confirmed = await JsRuntime.InvokeAsync<bool>("confirm", "Are you sure you want to delete the selected game?");
        if (!confirmed)
        {
            return;
        }

        DeleteSelectedCampaignCore();
    }

    private void DeleteSelectedCampaignCore()
    {
        if (string.IsNullOrWhiteSpace(selectedCampaignId))
        {
            savedSessionStatusMessage = "Select a campaign first.";
            return;
        }

        var selected = GetCampaignSelections()
            .FirstOrDefault(s => string.Equals(s.CampaignId, selectedCampaignId, StringComparison.OrdinalIgnoreCase));

        if (selected is null)
        {
            savedSessionStatusMessage = "Selected campaign could not be found.";
            return;
        }

        if (selected.IsReadonlySeed)
        {
            WorkspaceService.SetReadonlyCampaignHidden(selected.CampaignId, true);
            selectedCampaignId = null;
            RefreshSavedSessions();
            savedSessionStatusMessage = "Deleted selected game from games list.";
            return;
        }

        if (selected.IsKnownSessionEntry && !string.IsNullOrWhiteSpace(selected.SessionPath))
        {
            if (!WorkspaceService.DeleteKnownSnapshotPath(selected.SessionPath))
            {
                savedSessionStatusMessage = "Unable to delete selected game.";
                return;
            }

            selectedCampaignId = null;
            RefreshKnownSessionRegistryStatus();
            RefreshSavedSessions();
            savedSessionStatusMessage = "Deleted selected game from games list.";
            return;
        }

        savedSessionStatusMessage = "Delete is only available for saved or readonly games.";
    }

    private void CreateNewGamebox()
    {
        CreateNewGameboxInternal(navigateToEdit: false);
    }

    private void CreateAndEditNewGamebox()
    {
        CreateNewGameboxInternal(navigateToEdit: true);
    }

    private void CreateNewGameboxInternal(bool navigateToEdit)
    {
        if (string.IsNullOrWhiteSpace(newGameboxId))
        {
            newGameboxStatusMessage = "Gamebox id is required.";
            return;
        }

        try
        {
            var createdGameboxId = WorkspaceService.CreateNewGamebox(newGameboxId);
            newGameboxId = createdGameboxId;
            RefreshGameboxOptions();
            newGameboxStatusMessage = "Created new Gamebox.";

            if (string.IsNullOrWhiteSpace(newCampaignGameboxId))
            {
                newCampaignGameboxId = createdGameboxId;
            }

            if (navigateToEdit)
            {
                Navigation.NavigateTo($"/content?tab=gamebox&gameboxId={Uri.EscapeDataString(createdGameboxId)}");
            }
        }
        catch (Exception ex)
        {
            newGameboxStatusMessage = ex.Message;
        }
    }

    private void EditSelectedGamebox()
    {
        if (string.IsNullOrWhiteSpace(newCampaignGameboxId))
        {
            newSessionStatusMessage = "Select a Gamebox to edit.";
            return;
        }

        Navigation.NavigateTo($"/content?tab=gamebox&gameboxId={Uri.EscapeDataString(newCampaignGameboxId)}");
    }

    private void RefreshKnownSessionRegistryStatus()
    {
        knownSessionRegistryPath = WorkspaceService.GetKnownGameSessionRegistryPath();
        knownSessionRegistryLoadedFromDisk = WorkspaceService.HasKnownGameSessionRegistry();
        knownSessionRegistryWarning = WorkspaceService.GetKnownGameSessionRegistryWarning();
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
            ? "No saved game sessions are currently known."
            : $"Loaded {savedSessionFiles.Count} known game session(s).";
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

    private void ReloadKnownSessionRegistryNow()
    {
        try
        {
            WorkspaceService.ReloadKnownGameSessionRegistry();
            RefreshKnownSessionRegistryStatus();
            RefreshSavedSessions();
            savedSessionStatusMessage = "Reloaded known game session registry from disk.";
        }
        catch (Exception ex)
        {
            savedSessionStatusMessage = ex.Message;
        }
    }

    private void ResetSelectedSessionJoinCodeToDefault()
    {
        if (string.IsNullOrWhiteSpace(selectedSessionPath))
        {
            savedSessionStatusMessage = "Select a session first.";
            return;
        }

        try
        {
            customJoinCodeInput = WorkspaceService.ResetKnownGameSessionJoinCodeToDefault(selectedSessionPath);
            RefreshSavedSessions();
            savedSessionStatusMessage = "Reset selected join code to default.";
        }
        catch (Exception ex)
        {
            savedSessionStatusMessage = ex.Message;
        }
    }

    private void GenerateSelectedSessionJoinCode()
    {
        if (string.IsNullOrWhiteSpace(selectedSessionPath))
        {
            savedSessionStatusMessage = "Select a session first.";
            return;
        }

        try
        {
            customJoinCodeInput = WorkspaceService.GenerateKnownGameSessionJoinCode(selectedSessionPath);
            RefreshSavedSessions();
            savedSessionStatusMessage = "Generated a new join code for selected session.";
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

        joinCode = WorkspaceService.GetKnownGameSessionJoinCode(selectedSessionPath);
        joinStatusMessage = "Join code prefilled from selected session.";
    }

    private void SaveSelectedSessionJoinCode()
    {
        if (string.IsNullOrWhiteSpace(selectedSessionPath))
        {
            savedSessionStatusMessage = "Select a session first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(customJoinCodeInput))
        {
            savedSessionStatusMessage = "Enter a join code first.";
            return;
        }

        try
        {
            WorkspaceService.SetKnownGameSessionJoinCode(selectedSessionPath, customJoinCodeInput);
            RefreshSavedSessions();
            savedSessionStatusMessage = "Saved custom join code for selected session.";
        }
        catch (Exception ex)
        {
            savedSessionStatusMessage = ex.Message;
        }
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
            SavedSessionSort.LastJoinedDesc => sessions.OrderByDescending(s => s.LastJoinedUtc ?? DateTime.MinValue),
            SavedSessionSort.LastJoinedAsc => sessions.OrderBy(s => s.LastJoinedUtc ?? DateTime.MinValue),
            SavedSessionSort.JoinCodeUpdatedDesc => sessions.OrderByDescending(s => s.LastJoinCodeUpdatedUtc ?? DateTime.MinValue),
            SavedSessionSort.JoinCodeUpdatedAsc => sessions.OrderBy(s => s.LastJoinCodeUpdatedUtc ?? DateTime.MinValue),
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
        customJoinCodeInput = string.Empty;
        RefreshKnownSessionRegistryStatus();
        RefreshSavedSessions();
        savedSessionStatusMessage = "Removed selected session from known list.";
    }

    private void OpenGameSelector()
    {
        isGameSelectorOpen = true;
        RefreshKnownSessionRegistryStatus();
        RefreshSavedSessions();
        RefreshGameboxOptions();
        savedSessionStatusMessage = savedSessionFiles.Count == 0
            ? "No subscribed or saved game sessions are available yet."
            : $"Loaded {savedSessionFiles.Count} saved game session(s).";
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
            customJoinCodeInput = string.Empty;
            return;
        }

        if (!string.IsNullOrWhiteSpace(selectedSessionPath))
        {
            var selected = savedSessionFiles.FirstOrDefault(s => string.Equals(s.FilePath, selectedSessionPath, StringComparison.OrdinalIgnoreCase));
            customJoinCodeInput = selected?.JoinCode ?? string.Empty;
        }
    }

    private void SelectSession(string path)
    {
        selectedSessionPath = path;
        customJoinCodeInput = savedSessionFiles
            .FirstOrDefault(s => string.Equals(s.FilePath, path, StringComparison.OrdinalIgnoreCase))?.JoinCode
            ?? string.Empty;
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
            RefreshSavedSessions();
            if (WorkspaceService.CurrentVttSession?.Campaign is not null)
            {
                selectedCampaignId = WorkspaceService.CurrentVttSession.Campaign.Id;
            }
            else
            {
                var matchedKnownSession = savedSessionFiles.FirstOrDefault(s =>
                    string.Equals(s.JoinCode, joinCode, StringComparison.OrdinalIgnoreCase));
                if (matchedKnownSession is not null)
                {
                    selectedCampaignId = BuildKnownSessionCampaignId(matchedKnownSession.FilePath);
                }
            }

            joinStatusMessage = message;
            return;
        }

        joinStatusMessage = message;
    }

    private IReadOnlyList<CampaignSelectionEntry> GetCampaignSelections()
    {
        var selections = new List<CampaignSelectionEntry>();

        selections.AddRange(
            savedSessionFiles
                .Select(s => new CampaignSelectionEntry
                {
                    CampaignId = BuildKnownSessionCampaignId(s.FilePath),
                    CampaignName = s.FileName,
                    CurrentScenarioId = s.JoinCode,
                    CurrentScenarioName = "Known session",
                    CurrentSnapshotPath = s.FilePath,
                    IsReadonlySeed = false,
                    IsKnownSessionEntry = true,
                    SessionPath = s.FilePath
                }));

        if (WorkspaceService.CurrentVttSession is not null)
        {
            var campaign = WorkspaceService.CurrentVttSession.Campaign;
            if (!campaign.IsHidden)
            {
                selections.Add(new CampaignSelectionEntry
                {
                    CampaignId = campaign.Id,
                    CampaignName = campaign.Name,
                    CurrentScenarioId = campaign.ScenarioIds.FirstOrDefault() ?? $"{campaign.Id}-CURRENT-SCENARIO",
                    CurrentScenarioName = WorkspaceService.CurrentVttSession.CurrentVttScenario?.Title ?? "Current Scenario",
                    CurrentSnapshotPath = "Current session snapshot",
                    IsReadonlySeed = false,
                    IsKnownSessionEntry = false,
                    SessionPath = WorkspaceService.State.CurrentFilePath
                });
            }
        }

        selections.AddRange(
            WorkspaceService.ListReadonlyScenarios()
            .Select(s => new CampaignSelectionEntry
            {
                CampaignId = s.CampaignId,
                CampaignName = WorkspaceService.ListReadonlyCampaigns().FirstOrDefault(c => string.Equals(c.CampaignId, s.CampaignId, StringComparison.OrdinalIgnoreCase))?.Name ?? s.CampaignId,
                CurrentScenarioId = s.ScenarioId,
                CurrentScenarioName = s.Name,
                CurrentSnapshotPath = s.FilePath,
                IsReadonlySeed = true,
                IsKnownSessionEntry = false,
                SessionPath = s.FilePath
            }));

        return selections
            .GroupBy(s => s.CampaignId, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .OrderBy(s => s.CampaignName, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void SelectCampaign(string campaignId)
    {
        selectedCampaignId = campaignId;
    }

    private void SetCampaignBrowserViewMode(CampaignBrowserViewMode mode)
    {
        campaignBrowserViewMode = mode;
        WorkspaceService.SetCampaignBrowserViewMode(mode == CampaignBrowserViewMode.Card ? "Card" : "DetailList");
    }

    private static CampaignBrowserViewMode ParseCampaignBrowserViewMode(string? mode)
    {
        return string.Equals(mode, "Card", StringComparison.OrdinalIgnoreCase)
            ? CampaignBrowserViewMode.Card
            : CampaignBrowserViewMode.DetailList;
    }

    private static string BuildKnownSessionCampaignId(string sessionPath)
    {
        return $"KNOWN:{Path.GetFullPath(sessionPath)}";
    }

    private void OpenSelectedCampaignForPlay()
    {
        OpenSelectedCampaign("play");
    }

    private void OpenSelectedCampaignForDesign()
    {
        OpenSelectedCampaign("edit");
    }

    private void OpenSelectedCampaign(string mode)
    {
        if (string.IsNullOrWhiteSpace(selectedCampaignId))
        {
            savedSessionStatusMessage = "Select a campaign first.";
            return;
        }

        var selected = GetCampaignSelections()
            .FirstOrDefault(s => string.Equals(s.CampaignId, selectedCampaignId, StringComparison.OrdinalIgnoreCase));

        if (selected is null)
        {
            savedSessionStatusMessage = "Selected campaign did not resolve to a current scenario snapshot.";
            return;
        }

        try
        {
            if (selected.IsReadonlySeed)
            {
                WorkspaceService.ActivateReadonlyScenario(selected.CurrentScenarioId);
                savedSessionStatusMessage = $"Opened campaign {selectedCampaignId} using {selected.CurrentScenarioId}.";
            }
            else if (selected.IsKnownSessionEntry && !string.IsNullOrWhiteSpace(selected.SessionPath))
            {
                WorkspaceService.OpenVttSessionFromFile(selected.SessionPath);
                savedSessionStatusMessage = $"Opened known session for campaign {selectedCampaignId}.";
            }
            else
            {
                savedSessionStatusMessage = $"Opened campaign {selectedCampaignId}.";
            }

            Navigation.NavigateTo($"/workspace?mode={mode}");
        }
        catch (Exception ex)
        {
            savedSessionStatusMessage = ex.Message;
        }
    }

    private void StartNewSessionForPlay()
    {
        StartNewSessionAndNavigate("play");
    }

    private void StartNewSessionForDesign()
    {
        StartNewSessionAndNavigate("edit");
    }

    private void StartNewSessionAndNavigate(string mode)
    {
        StartNewSession();
        if (!string.IsNullOrWhiteSpace(newSessionStatusMessage) && !newSessionStatusMessage.Contains("Created", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Navigation.NavigateTo($"/workspace?mode={mode}");
    }

    private void StartNewSession()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(newCampaignGameboxId))
            {
                newSessionStatusMessage = "Select a valid Gamebox before creating a campaign.";
                return;
            }

            var campaignName = string.IsNullOrWhiteSpace(newSessionTitle) ? "New Campaign" : newSessionTitle.Trim();
            WorkspaceService.CreateNewCampaignSession(campaignName, newCampaignGameboxId);

            RefreshSavedSessions();
            newSessionStatusMessage = "Created new campaign.";
        }
        catch (Exception ex)
        {
            newSessionStatusMessage = ex.Message;
        }
    }

    private void CreateAndSaveNewSession()
    {
        if (string.IsNullOrWhiteSpace(newCampaignGameboxId))
        {
                newSessionStatusMessage = "Select a valid Gamebox before creating a campaign.";
            return;
        }

        if (string.IsNullOrWhiteSpace(newSessionSavePath))
        {
            newSessionStatusMessage = "Campaign save path is required for Create + Save.";
            return;
        }

        try
        {
            var campaignName = string.IsNullOrWhiteSpace(newSessionTitle) ? "New Campaign" : newSessionTitle.Trim();
            WorkspaceService.CreateNewCampaignSession(campaignName, newCampaignGameboxId);

            WorkspaceService.SaveCurrentSessionAs(newSessionSavePath);
            RefreshSavedSessions();
            newSessionStatusMessage = "Created and saved new campaign.";
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
    internal void TestSetNewCampaignGameboxId(string value) => newCampaignGameboxId = value ?? string.Empty;
    internal void TestCreateAndSaveNewSession() => CreateAndSaveNewSession();
    internal void TestSetNewGameboxId(string value) => newGameboxId = value ?? string.Empty;
    internal void TestCreateNewGamebox() => CreateNewGamebox();
    internal void TestCreateAndEditNewGamebox() => CreateAndEditNewGamebox();
    internal void TestSelectCampaign(string campaignId) => SelectCampaign(campaignId);
    internal void TestOpenSelectedCampaign() => OpenSelectedCampaignForPlay();
    internal void TestDeleteSelectedCampaign() => DeleteSelectedCampaignCore();
    internal string TestCampaignBrowserViewMode() => campaignBrowserViewMode.ToString();
    internal void TestSetCampaignBrowserViewMode(string value) => SetCampaignBrowserViewMode(ParseCampaignBrowserViewMode(value));
}
