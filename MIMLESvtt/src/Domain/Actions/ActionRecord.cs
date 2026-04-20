namespace MIMLESvtt.src
{
    public class ActionRecord
    {
        public string Id { get; set; } = string.Empty;

        public string ActionType { get; set; } = string.Empty;

        public string ActorParticipantId { get; set; } = string.Empty;

        public DateTime TimestampUtc { get; set; }

        public object? Payload { get; set; }
    }
}
