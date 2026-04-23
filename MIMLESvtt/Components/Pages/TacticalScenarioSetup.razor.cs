using Microsoft.AspNetCore.Components;
using MIMLESvtt.src.Domain.Persistence.VttSessionNSPC;

namespace MIMLESvtt.Components.Pages;

public partial class TacticalScenarioSetup : ComponentBase
{
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public VttSessionWorkspaceService WorkspaceService { get; set; } = default!;

    private readonly TacticalScenarioSetupState setupState = new();
    private string setupStatusMessage = string.Empty;

    private TacticalScenarioSetupState SetupState => setupState;

    private IReadOnlyList<string> AvailableSurfaceIds => WorkspaceService.State.CurrentVttSession?.Surfaces
        .Select(s => s.Id)
        .OrderBy(id => id, StringComparer.Ordinal)
        .ToList()
        ?? [];

    private IReadOnlyList<string> AvailableParticipantIds => WorkspaceService.State.CurrentVttSession?.Participants
        .Select(p => p.Id)
        .OrderBy(id => id, StringComparer.Ordinal)
        .ToList()
        ?? [];

    private void ApplySideFactionAssignment()
    {
        if (WorkspaceService.State.CurrentVttSession is null)
        {
            setupStatusMessage = "Load or create a session first.";
            return;
        }

        if (string.IsNullOrWhiteSpace(SetupState.PlacementOwnerParticipantId))
        {
            setupStatusMessage = "Owner participant id is required for side/faction assignment.";
            return;
        }

        if (WorkspaceService.State.CurrentVttSession.Participants.All(p =>
                !string.Equals(p.Id, SetupState.PlacementOwnerParticipantId, StringComparison.Ordinal)))
        {
            setupStatusMessage = "Owner participant id was not found in current session.";
            return;
        }

        var normalizedOwner = SetupState.PlacementOwnerParticipantId.Trim();
        var existingSeat = WorkspaceService.State.CurrentVttSession.PlayerSeats
            .FirstOrDefault(s => string.Equals(s.ParticipantId, normalizedOwner, StringComparison.Ordinal));

        if (existingSeat is null)
        {
            WorkspaceService.State.CurrentVttSession.PlayerSeats.Add(new MIMLESvtt.src.Domain.Models.PlayerSeat
            {
                Id = $"seat-{normalizedOwner}",
                ParticipantId = normalizedOwner,
                Role = MIMLESvtt.src.Domain.Models.ParticipantRole.Player,
                Side = SetupState.SideAssignment?.Trim() ?? string.Empty,
                Faction = SetupState.FactionAssignment?.Trim() ?? string.Empty
            });
        }
        else
        {
            existingSeat.Side = SetupState.SideAssignment?.Trim() ?? string.Empty;
            existingSeat.Faction = SetupState.FactionAssignment?.Trim() ?? string.Empty;
        }

        setupStatusMessage = "Applied side/faction assignment for selected owner participant.";
    }

    private void SaveSetupAsScenario()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(SetupState.ScenarioTitle))
            {
                throw new InvalidOperationException("Scenario title is required.");
            }

            if (string.IsNullOrWhiteSpace(SetupState.ScenarioPath))
            {
                throw new InvalidOperationException("Scenario path is required.");
            }

            WorkspaceService.SaveCurrentLayoutAsScenario(SetupState.ScenarioTitle, SetupState.ScenarioPath);
            setupStatusMessage = "Saved tactical setup as scenario snapshot.";
        }
        catch (Exception ex)
        {
            setupStatusMessage = ex.Message;
        }
    }

    private void OpenWorkspace()
    {
        Navigation.NavigateTo("/workspace");
    }

    private void GoHome()
    {
        Navigation.NavigateTo("/");
    }

    internal void TestSetScenarioTitle(string value) => setupState.ScenarioTitle = value ?? string.Empty;

    internal void TestSetScenarioPath(string value) => setupState.ScenarioPath = value ?? string.Empty;

    internal void TestSaveSetupAsScenario() => SaveSetupAsScenario();

    internal string TestSetupStatusMessage => setupStatusMessage;
}
