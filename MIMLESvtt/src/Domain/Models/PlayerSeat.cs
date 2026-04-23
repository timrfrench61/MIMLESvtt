namespace MIMLESvtt.src.Domain.Models
{
    public class PlayerSeat
    {
        public string Id { get; set; } = string.Empty;

        public string ParticipantId { get; set; } = string.Empty;

        public ParticipantRole Role { get; set; }

        public string Side { get; set; } = string.Empty;

        public string Faction { get; set; } = string.Empty;

        public Dictionary<string, object> Metadata { get; set; } = [];
    }
}
