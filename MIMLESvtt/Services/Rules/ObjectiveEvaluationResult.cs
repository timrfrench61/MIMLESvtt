namespace MIMLESvtt.src.Domain.Rules;

public class ObjectiveEvaluationResult
{
    public string ObjectiveId { get; init; } = string.Empty;

    public ObjectiveEvaluationStatus Status { get; init; } = ObjectiveEvaluationStatus.NotMet;

    public string SummaryMessage { get; init; } = string.Empty;

    public Dictionary<string, object> Diagnostics { get; init; } = [];
}
