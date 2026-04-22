namespace MIMLESvtt.src.Domain.Models
{
    public class TabletopOptions
    {
        public bool EnableFog { get; set; }

        public bool EnableTurnTracker { get; set; }

        public Dictionary<string, object> Options { get; set; } = [];
    }
}
