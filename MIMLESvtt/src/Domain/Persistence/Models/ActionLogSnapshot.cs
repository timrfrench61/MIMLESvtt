namespace MIMLESvtt.src.Domain.Persistence.Models
{
    public class ActionLogSnapshot
    {
        public int Version { get; set; }

        public string? SessionId { get; set; }

        public List<ActionRecord>? Actions { get; set; }
    }
}
