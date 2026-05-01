namespace MIMLESvtt.src
{
    public enum ActionEventCategory
    {
        Move,
        Gameplay,
        Admin,
        System
    }

    public class ActionRecord
    {
        public string Id { get; set; } = string.Empty;

        public string ActionType { get; set; } = string.Empty;

        public string ActorParticipantId { get; set; } = string.Empty;

        public DateTime TimestampUtc { get; set; }

        public ActionEventCategory EventCategory { get; set; } = ActionEventCategory.Gameplay;

        public string ReferencedPieceId { get; set; } = string.Empty;

        public string ReferencedZoneId { get; set; } = string.Empty;

        public int? ReferencedTurnNumber { get; set; }

        public object? Payload { get; set; }
    }
}
