using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules;

public class RulesContext
{
    public VttSession CurrentSession { get; init; } = new();

    public int CurrentTurnNumber { get; init; }

    public int CurrentTurnIndex { get; init; }

    public string CurrentPhase { get; init; } = string.Empty;

    public string ActorParticipantId { get; init; } = string.Empty;

    public string SelectedRulesModuleId { get; init; } = string.Empty;

    public string SelectedRulesModuleVersion { get; init; } = string.Empty;

    public string ScenarioMetadata { get; init; } = string.Empty;

    public bool EnableStrictValidation { get; init; }
}
