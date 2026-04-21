namespace MIMLESvtt.src
{
    public class TableSession
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public List<Participant> Participants { get; set; } = [];

        public List<SurfaceInstance> Surfaces { get; set; } = [];

        public List<PieceInstance> Pieces { get; set; } = [];

        public List<string> TurnOrder { get; set; } = [];

        public int CurrentTurnIndex { get; set; }

        public string CurrentPhase { get; set; } = string.Empty;

        public TableOptions Options { get; set; } = new();

        public VisibilityState Visibility { get; set; } = new();

        public List<ActionRecord> ActionLog { get; set; } = [];

        public Dictionary<string, object> ModuleState { get; set; } = [];
    }
}
