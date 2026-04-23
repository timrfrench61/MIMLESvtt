using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules.Brp;

public class BrpPercentileCheckRequest
{
    public string ActorParticipantId { get; init; } = string.Empty;

    public string SkillOrAttributeId { get; init; } = string.Empty;

    public int TargetValue { get; init; }

    public int Modifier { get; init; }
}

public class BrpSkillResolutionRequest
{
    public string ActorParticipantId { get; init; } = string.Empty;

    public string SkillId { get; init; } = string.Empty;

    public int TargetValue { get; init; }

    public int Modifier { get; init; }

    public string ContextTag { get; init; } = string.Empty;
}

public class BrpPercentileCheckResult
{
    public bool IsSuccess { get; init; }

    public int RolledValue { get; init; }

    public int AppliedModifier { get; init; }

    public int EffectiveTargetValue { get; init; }

    public bool Passed { get; init; }

    public DiceRollResult RollDetails { get; init; } = new();

    public string Summary { get; init; } = string.Empty;

    public Dictionary<string, object> EffectHints { get; init; } = [];
}

public class BrpOpposedCheckRequest
{
    public string AttackerParticipantId { get; init; } = string.Empty;

    public int AttackerTargetValue { get; init; }

    public int AttackerModifier { get; init; }

    public string DefenderParticipantId { get; init; } = string.Empty;

    public int DefenderTargetValue { get; init; }

    public int DefenderModifier { get; init; }
}

public class BrpOpposedCheckResult
{
    public bool IsSuccess { get; init; }

    public string WinnerParticipantId { get; init; } = string.Empty;

    public bool WasTie { get; init; }

    public string TieBreakPolicy { get; init; } = string.Empty;

    public BrpPercentileCheckResult AttackerResult { get; init; } = new();

    public BrpPercentileCheckResult DefenderResult { get; init; } = new();

    public string Summary { get; init; } = string.Empty;
}
