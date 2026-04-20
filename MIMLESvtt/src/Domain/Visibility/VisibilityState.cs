namespace MIMLESvtt.src
{
    public class VisibilityState
    {
        public bool IsHidden { get; set; }

        public List<string> VisibleToParticipantIds { get; set; } = [];
    }
}
