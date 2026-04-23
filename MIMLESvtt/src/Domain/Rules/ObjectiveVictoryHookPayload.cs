using MIMLESvtt.src;

namespace MIMLESvtt.src.Domain.Rules;

public class ObjectiveVictoryHookPayload
{
    public ObjectiveVictoryTriggerKind TriggerKind { get; init; } = ObjectiveVictoryTriggerKind.StateChange;

    public List<ObjectiveEvaluationResult> ObjectiveResults { get; init; } = [];

    public VictoryEvaluationResult VictoryResult { get; init; } = new();

    public string Message { get; init; } = string.Empty;

    public ActionRecord? RelatedAction { get; init; }
}
