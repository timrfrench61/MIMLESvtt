namespace MIMLESvtt.Components.Pages;

public class TacticalScenarioSetupState
{
    public string SelectedSurfaceId { get; set; } = string.Empty;

    public string SideAssignment { get; set; } = string.Empty;

    public string FactionAssignment { get; set; } = string.Empty;

    public string PlacementDefinitionId { get; set; } = string.Empty;

    public string PlacementOwnerParticipantId { get; set; } = string.Empty;

    public float PlacementX { get; set; }

    public float PlacementY { get; set; }

    public string ScenarioTitle { get; set; } = string.Empty;

    public string ScenarioPath { get; set; } = string.Empty;
}
