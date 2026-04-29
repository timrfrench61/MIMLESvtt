namespace MIMLESvtt.src
{
    public class ActionRequest
    {
        public string ActionType { get; set; } = string.Empty;

        public string ActorParticipantId { get; set; } = string.Empty;

        public object? Payload { get; set; }
    }
}
