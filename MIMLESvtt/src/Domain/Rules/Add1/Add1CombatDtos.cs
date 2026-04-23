using MIMLESvtt.src.Domain.Models;

namespace MIMLESvtt.src.Domain.Rules.Add1;

public enum Add1InitiativeMode
{
    Individual,
    Side
}

public enum Add1InitiativeTieBreakPolicy
{
    StableOrder,
    Alphabetical
}

public enum Add1InitiativeRerollPolicy
{
    EachRound,
    KeepOrderUntilCombatEnds
}

public class Add1InitiativeRequest
{
    public List<string> ParticipantIds { get; init; } = [];

    public Add1InitiativeMode InitiativeMode { get; init; } = Add1InitiativeMode.Individual;

    public Add1InitiativeTieBreakPolicy TieBreakPolicy { get; init; } = Add1InitiativeTieBreakPolicy.StableOrder;

    public Add1InitiativeRerollPolicy RerollPolicy { get; init; } = Add1InitiativeRerollPolicy.EachRound;

    public int RoundNumber { get; init; } = 1;

    public string PhaseContext { get; init; } = string.Empty;

    public Dictionary<string, string> SideByParticipantId { get; init; } = new(StringComparer.Ordinal);

    public Add1InitiativeResult? PreviousRoundResult { get; init; }
}

public class Add1InitiativeResult
{
    public List<string> OrderedParticipantIds { get; init; } = [];

    public Dictionary<string, int> InitiativeByParticipantId { get; init; } = new(StringComparer.Ordinal);

    public Dictionary<string, int> InitiativeBySide { get; init; } = new(StringComparer.Ordinal);

    public Add1InitiativeMode InitiativeMode { get; init; } = Add1InitiativeMode.Individual;

    public Add1InitiativeTieBreakPolicy TieBreakPolicy { get; init; } = Add1InitiativeTieBreakPolicy.StableOrder;

    public Add1InitiativeRerollPolicy RerollPolicy { get; init; } = Add1InitiativeRerollPolicy.EachRound;

    public int RoundNumber { get; init; }

    public string PhaseContext { get; init; } = string.Empty;

    public bool WasRerolledThisRound { get; init; }

    public Dictionary<string, object> Metadata { get; init; } = [];
}

public class Add1AttackRequest
{
    public string AttackerParticipantId { get; init; } = string.Empty;

    public string AttackerPieceId { get; init; } = string.Empty;

    public string TargetPieceId { get; init; } = string.Empty;

    public string AttackType { get; init; } = string.Empty;

    public int AttackModifier { get; init; }

    public string WeaponProfileId { get; init; } = string.Empty;

    public int ToHitThreshold { get; init; }

    public int DamageAmountOnHit { get; init; }

    public string? StatusOnHit { get; init; }
}

public class Add1AttackResult
{
    public bool IsSuccess { get; init; }

    public int AttackRoll { get; init; }

    public bool IsHit { get; init; }

    public DiceRollResult RollDetails { get; init; } = new();

    public int AppliedModifier { get; init; }

    public Add1DamageEffect DamageEffect { get; init; } = new();

    public Add1StatusConditionEffect StatusEffect { get; init; } = new();

    public string Summary { get; init; } = string.Empty;
}

public class Add1SavingThrowRequest
{
    public string ActorParticipantId { get; init; } = string.Empty;

    public string SaveType { get; init; } = string.Empty;

    public int SaveTarget { get; init; }
}

public class Add1SavingThrowResult
{
    public bool IsSuccess { get; init; }

    public bool SavePassed { get; init; }

    public int SaveRoll { get; init; }

    public int SaveTarget { get; init; }

    public DiceRollResult RollDetails { get; init; } = new();

    public int AppliedModifier { get; init; }

    public string Summary { get; init; } = string.Empty;
}

public class Add1DamageEffect
{
    public string TargetPieceId { get; init; } = string.Empty;

    public int DamageAmount { get; init; }

    public bool ShouldApply { get; init; }
}

public class Add1StatusConditionEffect
{
    public string TargetPieceId { get; init; } = string.Empty;

    public string ConditionMarkerId { get; init; } = string.Empty;

    public bool ShouldApply { get; init; }
}
