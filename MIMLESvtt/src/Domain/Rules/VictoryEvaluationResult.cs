namespace MIMLESvtt.src.Domain.Rules;

public class VictoryEvaluationResult
{
    public bool IsTerminal { get; init; }

    public string WinnerParticipantId { get; init; } = string.Empty;

    public string WinnerSide { get; init; } = string.Empty;

    public string VictoryType { get; init; } = string.Empty;

    public List<string> SatisfiedConditions { get; init; } = [];

    public string Message { get; init; } = string.Empty;
}
