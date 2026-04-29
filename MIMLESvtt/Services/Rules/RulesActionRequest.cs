namespace MIMLESvtt.src.Domain.Rules;

public class RulesActionRequest
{
    public string ActionType { get; init; } = string.Empty;

    public string ActorParticipantId { get; init; } = string.Empty;

    public object? Payload { get; init; }
}
