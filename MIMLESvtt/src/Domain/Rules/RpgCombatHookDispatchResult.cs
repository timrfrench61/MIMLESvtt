namespace MIMLESvtt.src.Domain.Rules;

public class RpgCombatHookDispatchResult
{
    public bool IsAllowed { get; init; }

    public string Message { get; init; } = string.Empty;

    public ObjectiveVictoryHookPayload ObjectiveVictoryPayload { get; init; } = new();

    public static RpgCombatHookDispatchResult Allow()
    {
        return new RpgCombatHookDispatchResult
        {
            IsAllowed = true,
            Message = string.Empty,
            ObjectiveVictoryPayload = new ObjectiveVictoryHookPayload()
        };
    }

    public static RpgCombatHookDispatchResult AllowWithEvaluation(ObjectiveVictoryHookPayload payload)
    {
        return new RpgCombatHookDispatchResult
        {
            IsAllowed = true,
            Message = string.Empty,
            ObjectiveVictoryPayload = payload ?? new ObjectiveVictoryHookPayload()
        };
    }

    public static RpgCombatHookDispatchResult Reject(string message)
    {
        return new RpgCombatHookDispatchResult
        {
            IsAllowed = false,
            Message = message ?? string.Empty,
            ObjectiveVictoryPayload = new ObjectiveVictoryHookPayload()
        };
    }
}
