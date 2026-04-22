namespace MIMLESvtt.src.Domain.Models.Visibility
{
    public class VisibilityState
    {
        public bool IsHidden { get; set; }

        public List<string> VisibleToParticipantIds { get; set; } = [];
    }
}
